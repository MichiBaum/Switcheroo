using System;
using System.Runtime.InteropServices;

namespace ManagedWinapi.Hooks {
    /// <summary>
    ///     A hook that can be used to create a log of keyboard and mouse events.
    /// </summary>
    public class JournalRecordHook : JournalHook {
        /// <summary>
        ///     Creates a new journal record hook.
        /// </summary>
        public JournalRecordHook() : base(HookType.WH_JOURNALRECORD) {
            Callback += JournalRecordHook_Callback;
        }

        /// <summary>
        ///     Occurs when a system modal dialog appears. This may be used
        ///     to stop recording.
        /// </summary>
        public event EventHandler? SystemModalDialogAppeared;

        /// <summary>
        ///     Occurs when a system modal dialog disappears. This may be used
        ///     to continue recording.
        /// </summary>
        public event EventHandler? SystemModalDialogDisappeared;

        /// <summary>
        ///     Occurs when an event can be recorded.
        /// </summary>
        public event EventHandler<JournalRecordEventArgs>? RecordEvent;

        private int JournalRecordHook_Callback(int code, IntPtr wParam, IntPtr lParam, ref bool callNext) {
            if (code == HC_ACTION) {
                EVENTMSG em = (EVENTMSG)(Marshal.PtrToStructure(lParam, typeof(EVENTMSG)) ?? throw new InvalidOperationException());
                JournalMessage jm = JournalMessage.Create(em);
                RecordEvent?.Invoke(this, new JournalRecordEventArgs(jm));
            } else if (code == HC_SYSMODALON) {
                SystemModalDialogAppeared?.Invoke(this, EventArgs.Empty);
            } else if (code == HC_SYSMODALOFF) {
                SystemModalDialogDisappeared?.Invoke(this, EventArgs.Empty);
            }

            return 0;
        }
    }
}