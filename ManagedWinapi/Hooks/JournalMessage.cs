// Copyright by Switcheroo

#region

using System;

#endregion

namespace ManagedWinapi.Hooks {
    /// <summary>
    ///     An event that has been recorded by a journal hook.
    /// </summary>
    public class JournalMessage {
        private JournalHook.EVENTMSG msg;

        private JournalMessage(JournalHook.EVENTMSG msg) {
            this.msg = msg;
        }

        /// <summary>
        ///     Creates a new journal message.
        /// </summary>
        public JournalMessage(IntPtr hWnd, uint message, uint paramL, uint paramH, uint time) {
            msg = new JournalHook.EVENTMSG {
                hWnd = hWnd,
                message = message,
                paramL = paramL,
                paramH = paramH,
                time = 0
            };
        }

        /// <summary>
        ///     The window this message has been sent to.
        /// </summary>
        public IntPtr HWnd => msg.hWnd;

        /// <summary>
        ///     The message.
        /// </summary>
        public uint Message => msg.message;

        /// <summary>
        ///     The first parameter of the message.
        /// </summary>
        public uint ParamL => msg.paramL;

        /// <summary>
        ///     The second parameter of the message.
        /// </summary>
        public uint ParamH => msg.paramH;

        /// <summary>
        ///     The timestamp of the message.
        /// </summary>
        public int Time {
            get => msg.time;
            set => msg.time = value;
        }

        internal static JournalMessage Create(JournalHook.EVENTMSG msg) {
            return new(msg);
        }

        /// <summary>
        ///     Returns a System.String that represents the current System.Object.
        /// </summary>
        public override string ToString() {
            return "JournalMessage[hWnd=" + msg.hWnd + ",message=" + msg.message + ",L=" + msg.paramL +
                   ",H=" + msg.paramH + ",time=" + msg.time + "]";
        }
    }
}