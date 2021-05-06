// Copyright by Switcheroo

#region

using System;
using System.Windows.Forms;

#endregion

namespace ManagedWinapi.Hooks {
    /// <summary>
    ///     A message that has been intercepted by a low-level mouse hook
    /// </summary>
    public class LowLevelKeyboardMessage : LowLevelMessage {
        private const int KEYEVENTF_KEYUP = 0x2;

        private const int WM_KEYDOWN = 0x100,
            WM_KEYUP = 0x101,
            WM_SYSKEYDOWN = 0x104,
            WM_SYSKEYUP = 0x105;

        /// <summary>
        ///     Creates a new low-level keyboard message.
        /// </summary>
        public LowLevelKeyboardMessage(int msg, int vkCode, int scanCode, int flags, int time, IntPtr dwExtraInfo)
            : base(msg, flags, time, dwExtraInfo) {
            VirtualKeyCode = vkCode;
            ScanCode = scanCode;
        }

        /// <summary>
        ///     The virtual key code that caused this message.
        /// </summary>
        public int VirtualKeyCode { get; }

        /// <summary>
        ///     The scan code that caused this message.
        /// </summary>
        public int ScanCode { get; }

        /// <summary>
        ///     Flags needed to replay this event.
        /// </summary>
        public uint KeyboardEventFlags {
            get {
                switch (Message) {
                    case WM_KEYDOWN:
                    case WM_SYSKEYDOWN:
                        return 0;
                    case WM_KEYUP:
                    case WM_SYSKEYUP:
                        return KEYEVENTF_KEYUP;
                }

                throw new Exception("Unsupported message");
            }
        }

        /// <summary>
        ///     Replays this event.
        /// </summary>
        public override void ReplayEvent() {
            KeyboardKey.InjectKeyboardEvent((Keys)VirtualKeyCode, (byte)ScanCode, KeyboardEventFlags,
                new UIntPtr((ulong)ExtraInfo.ToInt64()));
        }
    }
}