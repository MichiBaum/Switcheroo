using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace ManagedWinapi.Audio.Mixer {
    /// <summary>
    ///     Represents a mixer line, either a source line or a destination line.
    /// </summary>
    public abstract class MixerLine : IDisposable {
        private static readonly IList<MixerLine> EMPTY_LIST =
            new List<MixerLine>().AsReadOnly();

        /// <summary>
        ///     Occurs when this line changes.
        /// </summary>
        public EventHandler Changed;

        private MixerControl[] controls;

        internal MIXERLINE line;
        internal Mixer mixer;

        internal MixerLine(Mixer mixer, MIXERLINE line) {
            this.mixer = mixer;
            this.line = line;
        }

        /// <summary>
        ///     All controls of this line.
        /// </summary>
        public MixerControl[] Controls => controls ?? (controls = MixerControl.GetControls(mixer, this, ControlCount));

        /// <summary>
        ///     The volume control of this line, if it has one.
        /// </summary>
        public FaderMixerControl VolumeControl {
            get {
                foreach (MixerControl c in Controls)
                    if (c.ControlType == MixerControlType.MIXERCONTROL_CONTROLTYPE_VOLUME)
                        return (FaderMixerControl)c;
                return null;
            }
        }

        /// <summary>
        ///     The mute switch of this control, if it has one.
        /// </summary>
        public BooleanMixerControl MuteSwitch {
            get {
                foreach (MixerControl c in Controls)
                    if (c.ControlType == MixerControlType.MIXERCONTROL_CONTROLTYPE_MUTE)
                        return (BooleanMixerControl)c;
                return null;
            }
        }

        /// <summary>
        ///     Gets the ID of this line.
        /// </summary>
        public int Id => line.dwLineID;

        /// <summary>
        ///     Gets the number of channels of this line.
        /// </summary>
        public int ChannelCount => line.cChannels;

        /// <summary>
        ///     Gets the number of controls of this line.
        /// </summary>
        public int ControlCount => line.cControls;

        /// <summary>
        ///     Gets the short name of this line;
        /// </summary>
        public string ShortName => line.szShortName;

        /// <summary>
        ///     Gets the full name of this line.
        /// </summary>
        public string Name => line.szName;

        /// <summary>
        ///     Gets the component type of this line;
        /// </summary>
        public MixerLineComponentType ComponentType => (MixerLineComponentType)line.dwComponentType;

        /// <summary>
        ///     The mixer that owns this line.
        /// </summary>
        public Mixer Mixer => mixer;

        internal virtual IList<MixerLine> ChildLines => EMPTY_LIST;

        public virtual void Dispose() {
        }

        internal MixerLine? FindLine(int lineId) {
            if (Id == lineId) return this;
            foreach (MixerLine ml in ChildLines) {
                MixerLine? found = ml.FindLine(lineId);
                if (found != null)
                    return found;
            }

            return null;
        }

        internal MixerControl? FindControl(int ctrlId) {
            foreach (MixerControl c in Controls)
                if (c.Id == ctrlId)
                    return c;
            foreach (MixerLine l in ChildLines) {
                MixerControl? found = l.FindControl(ctrlId);
                if (found != null)
                    return found;
            }

            return null;
        }

        internal void OnChanged() {
            Changed?.Invoke(this, EventArgs.Empty);
        }

        #region PInvoke Declarations

        internal struct MIXERLINE {
            public int cbStruct;
            public int dwDestination;
            public int dwSource;
            public int dwLineID;
            public int fdwLine;
            public int dwUser;
            public int dwComponentType;
            public int cChannels;
            public int cConnections;
            public int cControls;

            [MarshalAs(UnmanagedType.ByValTStr,
                SizeConst = 16)]
            public string szShortName;

            [MarshalAs(UnmanagedType.ByValTStr,
                SizeConst = 64)]
            public string szName;

            public int dwType;
            public int dwDeviceID;
            public int wMid;
            public int wPid;
            public int vDriverVersion;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string szPname;
        }

        [DllImport("winmm.dll", CharSet = CharSet.Ansi)]
        internal static extern int mixerGetLineInfoA(IntPtr hmxobj, ref
            MIXERLINE pmxl, int fdwInfo);

        internal static int MIXER_GETLINEINFOF_DESTINATION = 0;
        internal static int MIXER_GETLINEINFOF_SOURCE = 1;

        #endregion
    }
}