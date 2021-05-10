using System;
using System.Runtime.InteropServices;

namespace ManagedWinapi.Hooks {
    /// <summary>
    ///     A hook that can be used to playback a log of keyboard and mouse events.
    /// </summary>
    public class JournalPlaybackHook : JournalHook {
        /// <summary>
        ///     Represents a method that yields the next journal message.
        /// </summary>
        public delegate JournalMessage? JournalQuery(ref int timestamp);

        private JournalMessage? nextEvent;
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
        public event EventHandler? SystemModalDialogAppeared;

        /// <summary>
        ///     Occurs when a system modal dialog disappears. This may be used
        ///     to continue playback.
        /// </summary>
        public event EventHandler? SystemModalDialogDisappeared;

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
                EVENTMSG em = (EVENTMSG)(Marshal.PtrToStructure(lParam, typeof(EVENTMSG)) ?? throw new InvalidOperationException());
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
}