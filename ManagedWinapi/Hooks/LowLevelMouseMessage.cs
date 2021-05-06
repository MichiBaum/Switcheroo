// Copyright by Switcheroo

#region

using ManagedWinapi.Windows;
using System;
using System.Windows.Forms;

#endregion

namespace ManagedWinapi.Hooks {
    /// <summary>
    ///     A message that has been intercepted by a low-level mouse hook
    /// </summary>
    public class LowLevelMouseMessage : LowLevelMessage {
        private const int WM_MOUSEMOVE = 0x200;
        private const int WM_LBUTTONDOWN = 0x201;
        private const int WM_LBUTTONUP = 0x202;
        private const int WM_LBUTTONDBLCLK = 0x203;
        private const int WM_RBUTTONDOWN = 0x204;
        private const int WM_RBUTTONUP = 0x205;
        private const int WM_RBUTTONDBLCLK = 0x206;
        private const int WM_MBUTTONDOWN = 0x207;
        private const int WM_MBUTTONUP = 0x208;
        private const int WM_MBUTTONDBLCLK = 0x209;
        private const int WM_MOUSEWHEEL = 0x20A;
        private const int WM_MOUSEHWHEEL = 0x020E;

        /// <summary>
        ///     Creates a new low-level mouse message.
        /// </summary>
        public LowLevelMouseMessage(int msg, POINT pt, int mouseData, int flags, int time, IntPtr dwExtraInfo)
            : base(msg, flags, time, dwExtraInfo) {
            Point = pt;
            MouseData = mouseData;
        }

        /// <summary>
        ///     The mouse position where this message occurred.
        /// </summary>
        public POINT Point { get; }

        /// <summary>
        ///     Additional mouse data, depending on the type of event.
        /// </summary>
        public int MouseData { get; }

        /// <summary>
        ///     Mouse event flags needed to replay this message.
        /// </summary>
        public uint MouseEventFlags {
            get {
                switch (Message) {
                    case WM_LBUTTONDOWN:
                        return (uint)MouseEventFlagValues.LEFTDOWN;
                    case WM_LBUTTONUP:
                        return (uint)MouseEventFlagValues.LEFTUP;
                    case WM_MOUSEMOVE:
                        return (uint)MouseEventFlagValues.MOVE;
                    case WM_MOUSEWHEEL:
                        return (uint)MouseEventFlagValues.WHEEL;
                    case WM_MOUSEHWHEEL:
                        return (uint)MouseEventFlagValues.HWHEEL;
                    case WM_RBUTTONDOWN:
                        return (uint)MouseEventFlagValues.RIGHTDOWN;
                    case WM_RBUTTONUP:
                        return (uint)MouseEventFlagValues.RIGHTUP;
                    case WM_MBUTTONDOWN:
                        return (uint)MouseEventFlagValues.MIDDLEDOWN;
                    case WM_MBUTTONUP:
                        return (uint)MouseEventFlagValues.MIDDLEUP;
                    case WM_MBUTTONDBLCLK:
                    case WM_RBUTTONDBLCLK:
                    case WM_LBUTTONDBLCLK:
                        return 0;
                }

                throw new Exception("Unsupported message");
            }
        }


        /// <summary>
        ///     Replays this event.
        /// </summary>
        public override void ReplayEvent() {
            Cursor.Position = Point;
            if (MouseEventFlags != 0)
                KeyboardKey.InjectMouseEvent(MouseEventFlags, 0, 0, (uint)MouseData >> 16,
                    new UIntPtr((ulong)ExtraInfo.ToInt64()));
        }

        [Flags]
        private enum MouseEventFlagValues {
            LEFTDOWN = 0x00000002,
            LEFTUP = 0x00000004,
            MIDDLEDOWN = 0x00000020,
            MIDDLEUP = 0x00000040,
            MOVE = 0x00000001,
            RIGHTDOWN = 0x00000008,
            RIGHTUP = 0x00000010,
            WHEEL = 0x00000800,
            HWHEEL = 0x00001000
        }
    }
}