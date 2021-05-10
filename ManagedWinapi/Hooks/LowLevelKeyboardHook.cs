using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace ManagedWinapi.Hooks {
    /// <summary>
    ///     A hook that intercepts keyboard events.
    /// </summary>
    public class LowLevelKeyboardHook : Hook {
        /// <summary>
        ///     Represents a method that handles an intercepted character.
        /// </summary>
        /// <param name="msg">The message that caused the character. Usually Either WM_KEYDOWN or WM_SYSKEYDOWN.</param>
        /// <param name="characters">
        ///     The character(s) that have been typed, or an empty string if a non-character key (like a
        ///     cursor key) has been pressed.
        /// </param>
        /// <param name="deadKeyPending">
        ///     Whether a dead key is pending. If a dead key is pending, you may not call the ToUnicode
        ///     method or similar methods, because they will destroy the deadkey state.
        /// </param>
        /// <param name="vkCode">The virtual key code of the message that caused the character.</param>
        /// <param name="scancode">The scancode of the message that caused the character.</param>
        /// <param name="flags">The flags of the message that caused the character.</param>
        /// <param name="time">The timestamp of the message that caused the character.</param>
        /// <param name="dwExtraInfo">The extra info of the message that caused the character.</param>
        public delegate void CharCallback(int msg, string characters, bool deadKeyPending, int vkCode, int scancode,
            int flags, int time, IntPtr dwExtraInfo);

        /// <summary>
        ///     Represents a method that handles an intercepted key.
        /// </summary>
        public delegate void KeyCallback(int msg, int vkCode, int scanCode, int flags, int time, IntPtr dwExtraInfo,
            ref bool handled);

        private char currentDeadChar = '\0';

        /// <summary>
        ///     Creates a low-level keyboard hook and hooks it.
        /// </summary>
        /// <param name="callback"></param>
        public LowLevelKeyboardHook(KeyCallback callback)
            : this() {
            KeyIntercepted = callback;
            StartHook();
        }

        /// <summary>
        ///     Creates a low-level keyboard hook.
        /// </summary>
        public LowLevelKeyboardHook()
            : base(HookType.WH_KEYBOARD_LL, false, true) {
            Callback += LowLevelKeyboardHook_Callback;
        }

        /// <summary>
        ///     Called when a key has been intercepted.
        /// </summary>
        public event KeyCallback? KeyIntercepted;

        /// <summary>
        ///     Called when a character has been intercepted.
        /// </summary>
        public event CharCallback? CharIntercepted;

        /// <summary>
        ///     Called when a key message has been intercepted.
        /// </summary>
        public event LowLevelMessageCallback? MessageIntercepted;

        private int LowLevelKeyboardHook_Callback(int code, IntPtr wParam, IntPtr lParam, ref bool callNext) {
            if (code != HC_ACTION) return 0;
            KBDLLHOOKSTRUCT? llh = (KBDLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(KBDLLHOOKSTRUCT));
            bool handled = false;
            int msg = (int)wParam;
            KeyIntercepted?.Invoke(msg, llh.vkCode, llh.scanCode, llh.flags, llh.time, llh.dwExtraInfo, ref handled);
            MessageIntercepted?.Invoke(new LowLevelKeyboardMessage((int)wParam, llh.vkCode, llh.scanCode, llh.flags, llh.time, llh.dwExtraInfo), ref handled);
            if (handled) {
                callNext = false;
                return 1;
            }

            if (CharIntercepted == null || (msg != 256 && msg != 260)) return 0;
            // Note that dead keys are somehow tricky, since ToUnicode changes their state
            // in the keyboard driver. So, if we catch a dead key and call ToUnicode on it,
            // we will have to stop the hook; otherwise the deadkey appears twice on the screen.
            // On the other hand, we try to avoid calling ToUnicode on the key pressed after
            // the dead key (the one which is modified by the deadkey), because that would
            // drop the deadkey altogether. Resynthesizing the deadkey event is hard since
            // some deadkeys are unshifted but used on shifted characters or vice versa.
            // This solution will not lose any dead keys; its only drawback is that dead
            // keys are not properly translated. Better implementations are welcome.
            if (llh.vkCode is (int)Keys.ShiftKey or (int)Keys.LShiftKey or (int)Keys.RShiftKey or (int)Keys.LControlKey or (int)Keys.RControlKey or (int)Keys.ControlKey or (int)Keys.Menu or (int)Keys.LMenu or (int)Keys.RMenu) {
                // ignore shift keys, they do not get modified by dead keys.
            } else if (currentDeadChar != '\0') {
                CharIntercepted(msg, "" + (llh.vkCode == (int)Keys.Space ? currentDeadChar : '\x01'), true,
                    llh.vkCode, llh.scanCode, llh.flags, llh.time, llh.dwExtraInfo);
                currentDeadChar = '\0';
            } else {
                short dummy = new KeyboardKey(Keys.Capital)
                    .State; // will refresh CAPS LOCK state for current thread
                byte[] kbdState = new byte[256];
                ApiHelper.FailIfZero(GetKeyboardState(kbdState));
                StringBuilder buff = new(64);
                int length = ToUnicode(llh.vkCode, llh.scanCode, kbdState, buff, 64, 0);
                if (length == -1) {
                    currentDeadChar = buff[0];
                    callNext = false;
                    return 1;
                }

                if (buff.Length != length)
                    buff.Remove(length, buff.Length - length);
                CharIntercepted(msg, buff.ToString(), false,
                    llh.vkCode, llh.scanCode, llh.flags, llh.time, llh.dwExtraInfo);
            }

            return 0;
        }

        [StructLayout(LayoutKind.Sequential)]
        private class KBDLLHOOKSTRUCT {
            public IntPtr dwExtraInfo;
            public int flags;
            public int scanCode;
            public int time;
            public int vkCode;
        }

        [DllImport("user32.dll")]
        private static extern int GetKeyboardState(byte[] lpKeyState);

        [DllImport("user32.dll")]
        private static extern int ToUnicode(int wVirtKey, int wScanCode, byte[] lpKeyState,
            [Out] [MarshalAs(UnmanagedType.LPWStr, SizeConst = 64)]
            StringBuilder pwszBuff, int cchBuff,
            uint wFlags);
    }
}