using ManagedWinapi.Windows;
using System;
using System.Runtime.InteropServices;

namespace ManagedWinapi.Hooks {
    /// <summary>
    ///     A hook that intercepts mouse events
    /// </summary>
    public class LowLevelMouseHook : Hook {
        /// <summary>
        ///     Represents a method that handles an intercepted mouse action.
        /// </summary>
        public delegate void MouseCallback(int msg, POINT pt, int mouseData, int flags, int time, IntPtr dwExtraInfo,
            ref bool handled);

        /// <summary>
        ///     Creates a low-level mouse hook and hooks it.
        /// </summary>
        public LowLevelMouseHook(MouseCallback callback)
            : this() {
            MouseIntercepted = callback;
            StartHook();
        }

        /// <summary>
        ///     Creates a low-level mouse hook.
        /// </summary>
        public LowLevelMouseHook()
            : base(HookType.WH_MOUSE_LL, false, true) {
            Callback += LowLevelMouseHook_Callback;
        }

        /// <summary>
        ///     Called when a mouse action has been intercepted.
        /// </summary>
        public event MouseCallback? MouseIntercepted;

        /// <summary>
        ///     Called when a mouse message has been intercepted.
        /// </summary>
        public event LowLevelMessageCallback? MessageIntercepted;

        private int LowLevelMouseHook_Callback(int code, IntPtr wParam, IntPtr lParam, ref bool callNext) {
            if (code != HC_ACTION) return 0;
            MSLLHOOKSTRUCT? llh = (MSLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(MSLLHOOKSTRUCT));
            bool handled = false;
            MouseIntercepted?.Invoke((int)wParam, llh.pt, llh.mouseData, llh.flags, llh.time, llh.dwExtraInfo, ref handled);
            MessageIntercepted?.Invoke(new LowLevelMouseMessage((int)wParam, llh.pt, llh.mouseData, llh.flags, llh.time, llh.dwExtraInfo), ref handled);
            if (!handled) return 0;
            callNext = false;
            return 1;

        }

        [StructLayout(LayoutKind.Sequential)]
        private class MSLLHOOKSTRUCT {
            public IntPtr dwExtraInfo;
            public int flags;
            public int mouseData;
            public POINT pt;
            public int time;
        }
    }
}