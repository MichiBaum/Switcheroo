using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace ManagedWinapi.Hooks
{
    /// <summary>
    ///     A hook that intercepts local window messages.
    /// </summary>
    public class LocalMessageHook : Hook {
        /// <summary>
        ///     Represents a method that handles a message from a message hook.
        /// </summary>
        /// <param name="msg"></param>
        public delegate void MessageCallback(Message msg);

        /// <summary>
        ///     Creates a local message hook and hooks it.
        /// </summary>
        /// <param name="callback"></param>
        public LocalMessageHook(MessageCallback callback)
            : this() {
            MessageOccurred = callback;
            StartHook();
        }

        /// <summary>
        ///     Creates a local message hook.
        /// </summary>
        public LocalMessageHook()
            : base(HookType.WH_GETMESSAGE, false, false) {
            Callback += MessageHookCallback;
        }

        /// <summary>
        ///     Called when a message has been intercepted.
        /// </summary>
        public event MessageCallback MessageOccurred;

        private int MessageHookCallback(int code, IntPtr lParam, IntPtr wParam, ref bool callNext) {
            if (code == HC_ACTION) {
                Message msg = (Message)Marshal.PtrToStructure(wParam, typeof(Message));
                if (MessageOccurred != null) MessageOccurred(msg);
            }

            return 0;
        }
    }
}