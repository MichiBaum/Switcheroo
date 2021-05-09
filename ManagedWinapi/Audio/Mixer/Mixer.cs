using ManagedWinapi.Windows;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace ManagedWinapi.Audio.Mixer {
    /// <summary>
    ///     Represents a mixer provided by a sound card. Each mixer has
    ///     multiple destination lines (e. g. Record and Playback) of which
    ///     each has multiple source lines (Wave, MIDI, Mic, etc.).
    /// </summary>
    public class Mixer : IDisposable {
        private readonly MIXERCAPS mc;

        /// <summary>
        ///     Occurs when a control of this mixer changes value.
        /// </summary>
        public MixerEventHandler ControlChanged;

        private IList<DestinationLine>? destLines;

        private IntPtr hMixer;

        /// <summary>
        ///     Occurs when a line of this mixer changes.
        /// </summary>
        public MixerEventHandler LineChanged;

        private Mixer(IntPtr hMixer) {
            this.hMixer = hMixer;
            EventDispatchingNativeWindow.Instance.EventHandler += ednw_EventHandler;
            mixerGetDevCapsA(hMixer, ref mc, Marshal.SizeOf(mc));
        }

        /// <summary>
        ///     Gets the number of available mixers in this system.
        /// </summary>
        public static uint MixerCount => mixerGetNumDevs();

        /// <summary>
        ///     Whether to create change events.
        ///     Enabling this may create a slight performance impact, so only
        ///     enable it if you handle these events.
        /// </summary>
        public bool CreateEvents { get; set; }

        internal IntPtr Handle => hMixer;

        /// <summary>
        ///     Gets the name of this mixer's sound card.
        /// </summary>
        public string Name => mc.szPname;

        /// <summary>
        ///     Gets the number of destination lines of this mixer.
        /// </summary>
        public int DestinationLineCount => mc.cDestinations;

        /// <summary>
        ///     Gets all destination lines of this mixer
        /// </summary>
        public IList<DestinationLine> DestinationLines {
            get {
                if (destLines == null) {
                    int dlc = DestinationLineCount;
                    List<DestinationLine> l = new(dlc);
                    for (int i = 0; i < dlc; i++) l.Add(DestinationLine.GetLine(this, i));
                    destLines = l.AsReadOnly();
                }

                return destLines;
            }
        }

        /// <summary>
        ///     Disposes this mixer.
        /// </summary>
        public void Dispose() {
            if (destLines != null) {
                foreach (DestinationLine dl in destLines) dl.Dispose();
                destLines = null;
            }

            if (hMixer.ToInt32() != 0) {
                mixerClose(hMixer);
                EventDispatchingNativeWindow.Instance.EventHandler -= ednw_EventHandler;
                hMixer = IntPtr.Zero;
            }
        }

        /// <summary>
        ///     Opens a mixer.
        /// </summary>
        /// <param name="index">The zero-based index of this mixer.</param>
        /// <returns>A reference to this mixer.</returns>
        public static Mixer OpenMixer(uint index) {
            if (index < 0 || index > MixerCount)
                throw new ArgumentException();
            IntPtr hMixer = IntPtr.Zero;
            EventDispatchingNativeWindow ednw = EventDispatchingNativeWindow.Instance;
            int error = mixerOpen(ref hMixer, index, ednw.Handle, IntPtr.Zero, CALLBACK_WINDOW);
            if (error != 0) throw new Win32Exception("Could not load mixer: " + error);
            return new Mixer(hMixer);
        }

        private void ednw_EventHandler(ref Message m, ref bool handled) {
            if (!CreateEvents)
                return;
            if (m.Msg == MM_MIXM_CONTROL_CHANGE && m.WParam == hMixer) {
                int ctrlID = m.LParam.ToInt32();
                MixerControl? c = FindControl(ctrlID);
                if (c != null) {
                    ControlChanged?.Invoke(this, new MixerEventArgs(this, c.Line, c));
                    c.OnChanged();
                }
            } else if (m.Msg == MM_MIXM_LINE_CHANGE && m.WParam == hMixer) {
                int lineID = m.LParam.ToInt32();
                MixerLine l = FindLine(lineID);
                if (l != null) {
                    if (ControlChanged != null) LineChanged(this, new MixerEventArgs(this, l, null));
                    l.OnChanged();
                }
            }
        }

        /// <summary>
        ///     Find a line of this mixer by ID.
        /// </summary>
        /// <param name="lineId">ID of the line to find</param>
        /// <returns>The line, or <code>null</code> if no line was found.</returns>
        public MixerLine FindLine(int lineId) {
            foreach (DestinationLine dl in DestinationLines) {
                MixerLine found = dl.FindLine(lineId);
                if (found != null)
                    return found;
            }

            return null;
        }

        /// <summary>
        ///     Find a control of this mixer by ID.
        /// </summary>
        /// <param name="ctrlId">ID of the control to find.</param>
        /// <returns>The control, or <code>null</code> if no control was found.</returns>
        public MixerControl? FindControl(int ctrlId) {
            foreach (DestinationLine dl in DestinationLines) {
                MixerControl? found = dl.FindControl(ctrlId);
                if (found != null)
                    return found;
            }

            return null;
        }

        [DllImport("winmm.dll", SetLastError = true)]
        private static extern uint mixerGetNumDevs();

        [DllImport("winmm.dll")]
        private static extern Int32 mixerOpen(ref IntPtr phmx, uint pMxId,
            IntPtr dwCallback, IntPtr dwInstance, UInt32 fdwOpen);

        [DllImport("winmm.dll")]
        private static extern Int32 mixerClose(IntPtr hmx);

        [DllImport("winmm.dll", CharSet = CharSet.Ansi)]
        private static extern int mixerGetDevCapsA(IntPtr uMxId, ref MIXERCAPS
            pmxcaps, int cbmxcaps);

        private struct MIXERCAPS {
            public short wMid;
            public short wPid;
            public int vDriverVersion;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string szPname;

            public int fdwSupport;
            public int cDestinations;
        }

        private static readonly uint CALLBACK_WINDOW = 0x00010000;
        private static readonly int MM_MIXM_LINE_CHANGE = 0x3D0;
        private static readonly int MM_MIXM_CONTROL_CHANGE = 0x3D1;
    }
}