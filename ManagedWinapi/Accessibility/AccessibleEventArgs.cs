using System;

namespace ManagedWinapi.Accessibility {
    /// <summary>
    ///     Provides data for accessible events.
    /// </summary>
    public class AccessibleEventArgs : EventArgs {
        /// <summary>
        ///     Initializes a new instance of the AccessibleEventArgs class.
        /// </summary>
        public AccessibleEventArgs(AccessibleEventType eventType,
            IntPtr hwnd, uint idObject, uint idChild, uint dwEventThread, uint dwmsEventTime) {
            EventType = eventType;
            HWnd = hwnd;
            ObjectID = idObject;
            ChildID = idChild;
            Thread = dwEventThread;
            Time = dwmsEventTime;
        }

        /// <summary>
        ///     Type of this accessible event
        /// </summary>
        public AccessibleEventType EventType { get; }

        /// <summary>
        ///     Handle of the affected window, if any.
        /// </summary>
        public IntPtr HWnd { get; }

        /// <summary>
        ///     Object ID.
        /// </summary>
        public uint ObjectID { get; }

        /// <summary>
        ///     Child ID.
        /// </summary>
        public uint ChildID { get; }

        /// <summary>
        ///     The thread that generated this event.
        /// </summary>
        public uint Thread { get; }

        /// <summary>
        ///     Time in milliseconds when the event was generated.
        /// </summary>
        public uint Time { get; }

        /// <summary>
        ///     The accessible object related to this event.
        /// </summary>
        public SystemAccessibleObject AccessibleObject => AccessibleEventListener.GetAccessibleObject(this);
    }
}