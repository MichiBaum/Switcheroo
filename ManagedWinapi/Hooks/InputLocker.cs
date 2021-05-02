using System;

namespace ManagedWinapi.Hooks {
    /// <summary>
    ///     Convenience class that uses a journal playback hook to block keyboard
    ///     and mouse input for some time.
    /// </summary>
    public class InputLocker : IDisposable {
        private readonly JournalPlaybackHook hook;
        private readonly int interval;
        private int count;

        /// <summary>
        ///     Locks the input for <code>interval * count</code> milliseconds. The
        ///     lock can be canceled every <code>interval</code> milliseconds. If count is
        ///     negative, the lock will be active until cancelled.
        /// </summary>
        /// <param name="interval">The interval to lock the input.</param>
        /// <param name="count">How often to lock the input.</param>
        /// <param name="force">
        ///     If <code>true</code>, the lock cannot be canceled
        ///     by pressing Control+Alt+Delete
        /// </param>
        public InputLocker(int interval, int count, bool force) {
            this.interval = interval;
            this.count = count;
            hook = new JournalPlaybackHook();
            hook.GetNextJournalMessage += Hook_GetNextJournalMessage;
            if (force)
                hook.JournalCancelled += Hook_JournalCancelled;
            hook.StartHook();
        }

        /// <summary>
        ///     Unlocks the input.
        /// </summary>
        public void Dispose() {
            Unlock();
            hook.Dispose();
        }

        private void Hook_JournalCancelled(object sender, EventArgs e) {
            if (count >= 0)
                count++;
            hook.StartHook();
        }

        private JournalMessage Hook_GetNextJournalMessage(ref int timestamp) {
            if (count == 0)
                return null;
            timestamp = Environment.TickCount + interval;
            if (count > 0)
                count--;
            return null;
        }

        /// <summary>
        ///     Unlocks the input.
        /// </summary>
        public void Unlock() {
            count = 0;
        }

        /// <summary>
        ///     Lock input for given number of milliseconds
        /// </summary>
        /// <param name="millis">Number of milliseconds to lock</param>
        /// <param name="force">
        ///     If <code>true</code>, the lock cannot be canceled
        ///     by pressing Control+Alt+Delete
        /// </param>
        public static void LockInputFor(int millis, bool force) {
            new InputLocker(millis, 1, force);
        }
    }
}