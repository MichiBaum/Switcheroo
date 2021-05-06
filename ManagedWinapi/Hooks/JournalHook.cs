using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace ManagedWinapi.Hooks {
    /// <summary>
    ///     Abstract base class for hooks that can be used to create or playback
    ///     a log of keyboard and mouse events.
    /// </summary>
    public abstract class JournalHook : Hook {
        private static readonly int WM_CANCELJOURNAL = 0x4B;
        private readonly LocalMessageHook lmh;

        /// <summary>
        ///     Creates a new journal hook.
        /// </summary>
        public JournalHook(HookType type) : base(type, true, false) {
            lmh = new LocalMessageHook();
            lmh.MessageOccurred += Lmh_Callback;
        }

        /// <summary>
        ///     Occurs when the journal activity has been cancelled by
        ///     CTRL+ALT+DEL or CTRL+ESC.
        /// </summary>
        public event EventHandler JournalCancelled;

        private void Lmh_Callback(Message msg) {
            if (msg.Msg == WM_CANCELJOURNAL) {
                hooked = false;
                lmh.Unhook();
                JournalCancelled?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        ///     Hooks the hook.
        /// </summary>
        public override void StartHook() {
            if (Hooked)
                return;
            lmh.StartHook();
            base.StartHook();
        }

        /// <summary>
        ///     Unhooks the hook.
        /// </summary>
        public override void Unhook() {
            if (!Hooked)
                return;
            base.Unhook();
            lmh.Unhook();
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct EVENTMSG {
            public uint message;
            public uint paramL;
            public uint paramH;
            public int time;
            public IntPtr hWnd;
        }
    }
}