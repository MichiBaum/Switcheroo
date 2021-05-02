using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace ManagedWinapi.Hooks {
    /// <summary>
    ///     Abstract base class for hooks that can be used to create or playback
    ///     a log of keyboard and mouse events.
    /// </summary>
    public abstract class JournalHook : Hook {
        private readonly LocalMessageHook lmh;

        /// <summary>
        ///     Creates a new journal hook.
        /// </summary>
        public JournalHook(HookType type) : base(type, true, false) {
            lmh = new LocalMessageHook();
            lmh.MessageOccurred += Lmh_Callback;
        }

        /// <summary>
        ///     Occurs when the journal activity has been cancelled by
        ///     CTRL+ALT+DEL or CTRL+ESC.
        /// </summary>
        public event EventHandler JournalCancelled;

        private void Lmh_Callback(Message msg) {
            if (msg.Msg == WM_CANCELJOURNAL) {
                hooked = false;
                lmh.Unhook();
                JournalCancelled?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        ///     Hooks the hook.
        /// </summary>
        public override void StartHook() {
            if (Hooked)
                return;
            lmh.StartHook();
            base.StartHook();
        }

        /// <summary>
        ///     Unhooks the hook.
        /// </summary>
        public override void Unhook() {
            if (!Hooked)
                return;
            base.Unhook();
            lmh.Unhook();
        }

        #region PInvoke Declarations

        private static readonly int WM_CANCELJOURNAL = 0x4B;

        [StructLayout(LayoutKind.Sequential)]
        internal struct EVENTMSG {
            public uint message;
            public uint paramL;
            public uint paramH;
            public int time;
            public IntPtr hWnd;
        }

        #endregion
    }

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
        public event EventHandler SystemModalDialogAppeared;

        /// <summary>
        ///     Occurs when a system modal dialog disappears. This may be used
        ///     to continue recording.
        /// </summary>
        public event EventHandler SystemModalDialogDisappeared;

        /// <summary>
        ///     Occurs when an event can be recorded.
        /// </summary>
        public event EventHandler<JournalRecordEventArgs> RecordEvent;

        private int JournalRecordHook_Callback(int code, IntPtr wParam, IntPtr lParam, ref bool callNext) {
            if (code == HC_ACTION) {
                EVENTMSG em = (EVENTMSG)Marshal.PtrToStructure(lParam, typeof(EVENTMSG));
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

    /// <summary>
    ///     A hook that can be used to playback a log of keyboard and mouse events.
    /// </summary>
    public class JournalPlaybackHook : JournalHook {
        /// <summary>
        ///     Represents a method that yields the next journal message.
        /// </summary>
        public delegate JournalMessage JournalQuery(ref int timestamp);

        private JournalMessage nextEvent;
        private int nextEventTime;

        /// <summary>
        ///     Creates a new journal playback hook.
        /// </summary>
        public JournalPlaybackHook() : base(HookType.WH_JOURNALPLAYBACK) {
            Callback += JournalPlaybackHook_Callback;
        }

        /// <summary>
        ///     Occurs when a system modal dialog appears. This may be used to
        ///     stop playback.
        /// </summary>
        public event EventHandler SystemModalDialogAppeared;

        /// <summary>
        ///     Occurs when a system modal dialog disappears. This may be used
        ///     to continue playback.
        /// </summary>
        public event EventHandler SystemModalDialogDisappeared;

        /// <summary>
        ///     Occurs when the next journal message is needed. If the message is
        ///     <null /> and a timestamp in the future, it just waits for that time and
        ///     asks for a message again. If the message is <null /> and the timestamp is
        ///     in the past, playback stops.
        /// </summary>
        public event JournalQuery GetNextJournalMessage;

        private int JournalPlaybackHook_Callback(int code, IntPtr wParam, IntPtr lParam, ref bool callNext) {
            if (code == HC_GETNEXT) {
                callNext = false;
                int tick = Environment.TickCount;
                if (nextEventTime > tick) return nextEventTime - tick;
                if (nextEvent == null) {
                    nextEventTime = 0;
                    nextEvent = GetNextJournalMessage(ref nextEventTime);
                    if (nextEventTime <= tick) {
                        if (nextEvent == null) {
                            // shutdown the hook
                            Unhook();
                            return 1;
                        }

                        nextEventTime = nextEvent.Time;
                    }

                    if (nextEventTime > tick) return nextEventTime - tick;
                }

                // now we have the next event, which should be sent
                EVENTMSG em = (EVENTMSG)Marshal.PtrToStructure(lParam, typeof(EVENTMSG));
                em.hWnd = nextEvent.HWnd;
                em.time = nextEvent.Time;
                em.message = nextEvent.Message;
                em.paramH = nextEvent.ParamH;
                em.paramL = nextEvent.ParamL;
                Marshal.StructureToPtr(em, lParam, false);
                return 0;
            }

            if (code == HC_SKIP) {
                nextEvent = null;
                nextEventTime = 0;
            } else if (code == HC_SYSMODALON) {
                SystemModalDialogAppeared?.Invoke(this, EventArgs.Empty);
            } else if (code == HC_SYSMODALOFF) {
                SystemModalDialogDisappeared?.Invoke(this, EventArgs.Empty);
            }

            return 0;
        }
    }

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