using System;

namespace ManagedWinapi.Hooks
{
    /// <summary>
    ///     Event data for a journal record event.
    /// </summary>
    public class JournalRecordEventArgs : EventArgs {
        internal JournalRecordEventArgs(JournalMessage msg) {
            RecordedMessage = msg;
        }

        /// <summary>
        ///     The recorded message.
        /// </summary>
        public JournalMessage RecordedMessage { get; }
    }
}