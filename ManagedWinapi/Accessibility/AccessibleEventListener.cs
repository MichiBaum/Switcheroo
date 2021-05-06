using Accessibility;
using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace ManagedWinapi.Accessibility {
    /// <summary>
    ///     Listens to events from the Windows accessibility system. These events are useful
    ///     if you want to write a screenreader or similar program.
    /// </summary>
    public class AccessibleEventListener : Component {
        private readonly WinEventDelegate internalDelegate;
        private bool enabled;
        private GCHandle gch;
        private IntPtr handle = IntPtr.Zero;
        private AccessibleEventType max = AccessibleEventType.EventMax;
        private AccessibleEventType min = AccessibleEventType.EventMin;
        private UInt32 processId;
        private UInt32 threadId;

        /// <summary>
        ///     Initializes a new instance of this class with the specified container.
        /// </summary>
        /// <param name="container">The container to add it to.</param>
        public AccessibleEventListener(IContainer container)
            : this() {
            container.Add(this);
        }

        /// <summary>
        ///     Initializes a new instance of this class.
        /// </summary>
        public AccessibleEventListener() {
            internalDelegate = InternalCallback;
            gch = GCHandle.Alloc(internalDelegate);
        }

        /// <summary>
        ///     Enables this listener so that it reports accessible events.
        /// </summary>
        public bool Enabled {
            get => enabled;
            set {
                enabled = value;
                updateListener();
            }
        }

        /// <summary>
        ///     The minimal event type to listen to.
        /// </summary>
        public AccessibleEventType MinimalEventType {
            get => min;
            set {
                min = value;
                updateListener();
            }
        }

        /// <summary>
        ///     The maximal event type to listen to.
        /// </summary>
        public AccessibleEventType MaximalEventType {
            get => max;
            set {
                max = value;
                updateListener();
            }
        }

        /// <summary>
        ///     The Process ID to listen to.
        ///     Default 0 listens to all processes.
        /// </summary>
        public UInt32 ProcessId {
            get => processId;
            set {
                processId = value;
                updateListener();
            }
        }

        /// <summary>
        ///     The Thread ID to listen to.
        ///     Default 0 listens to all threads.
        /// </summary>
        public UInt32 ThreadId {
            get => threadId;
            set {
                threadId = value;
                updateListener();
            }
        }

        /// <summary>
        ///     Occurs when an accessible event is received.
        /// </summary>
        public event AccessibleEventHandler EventOccurred;

        private void updateListener() {
            if (handle != IntPtr.Zero) {
                UnhookWinEvent(handle);
                handle = IntPtr.Zero;
            }

            if (enabled) handle = SetWinEventHook(min, max, IntPtr.Zero, internalDelegate, processId, threadId, 0);
        }

        /// <summary>
        ///     Releases all resources used by the System.ComponentModel.Component.
        /// </summary>
        /// <param name="disposing">Whether to dispose managed resources.</param>
        protected override void Dispose(bool disposing) {
            if (enabled) {
                enabled = false;
                updateListener();
            }

            gch.Free();
            base.Dispose(disposing);
        }

        private void InternalCallback(IntPtr hWinEventHook, AccessibleEventType eventType,
            IntPtr hwnd, uint idObject, uint idChild, uint dwEventThread, uint dwmsEventTime) {
            if (hWinEventHook != handle)
                return;
            AccessibleEventArgs aea =
                new(eventType, hwnd, idObject, idChild, dwEventThread, dwmsEventTime);
            if (EventOccurred != null)
                EventOccurred(this, aea);
        }

        internal static SystemAccessibleObject GetAccessibleObject(AccessibleEventArgs e) {
            IAccessible iacc;
            object child;
            uint result = AccessibleObjectFromEvent(e.HWnd, e.ObjectID, e.ChildID, out iacc, out child);
            if (result != 0)
                throw new Exception("AccessibleObjectFromPoint returned " + result);
            return new SystemAccessibleObject(iacc, (int)child);
        }

        [DllImport("user32.dll")]
        private static extern IntPtr SetWinEventHook(AccessibleEventType eventMin, AccessibleEventType eventMax, IntPtr
                hmodWinEventProc, WinEventDelegate lpfnWinEventProc, uint idProcess,
            uint idThread, uint dwFlags);

        [DllImport("user32.dll")]
        private static extern bool UnhookWinEvent(IntPtr hWinEventHook);

        [DllImport("oleacc.dll")]
        private static extern uint AccessibleObjectFromEvent(IntPtr hwnd, uint dwObjectID, uint dwChildID,
            out IAccessible ppacc, [MarshalAs(UnmanagedType.Struct)] out object pvarChild);

        private delegate void WinEventDelegate(IntPtr hWinEventHook, AccessibleEventType eventType,
            IntPtr hwnd, uint idObject, uint idChild, uint dwEventThread, uint dwmsEventTime);
    }
}