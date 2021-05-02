using System;

namespace ManagedWinapi.Hooks {
    /// <summary>
    ///     A message that has been intercepted by a low-level hook
    /// </summary>
    public abstract class LowLevelMessage {
        internal LowLevelMessage(int msg, int flags, int time, IntPtr dwExtraInfo) {
            Message = msg;
            Flags = flags;
            Time = time;
            ExtraInfo = dwExtraInfo;
        }

        /// <summary>
        ///     The time this message happened.
        /// </summary>
        public int Time { get; set; }

        /// <summary>
        ///     Flags of the message. Its contents depend on the message.
        /// </summary>
        public int Flags { get; }

        /// <summary>
        ///     The message identifier.
        /// </summary>
        public int Message { get; }

        /// <summary>
        ///     Extra information. Its contents depend on the message.
        /// </summary>
        public IntPtr ExtraInfo { get; }

        /// <summary>
        ///     Replays this event as if the user did it again.
        /// </summary>
        public abstract void ReplayEvent();
    }
}