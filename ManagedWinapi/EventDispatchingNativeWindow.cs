using System;
using System.Windows.Forms;

namespace ManagedWinapi.Windows {
    /// <summary>
    ///     Called by an EventDispatchingNativeWindow when a window message is received
    /// </summary>
    /// <param name="m">The message to handle.</param>
    /// <param name="handled">
    ///     Whether the event has already been handled. If this value is true, the handler
    ///     should return immediately. It may set the value to true to indicate that no others
    ///     should handle it. If the event is not handled by any handler, it is passed to the
    ///     default WindowProc.
    /// </param>
    public delegate void WndProcEventHandler(ref Message m, ref bool handled);

    /// <summary>
    ///     A Win32 native window that delegates window messages to handlers. So several
    ///     components can use the same native window to save "USER resources". This class
    ///     is useful when writing your own components.
    /// </summary>
    public class EventDispatchingNativeWindow : NativeWindow {
        private static readonly Object myLock = new Object();
        private static EventDispatchingNativeWindow _instance;

        /// <summary>
        ///     Create your own event dispatching window.
        /// </summary>
        public EventDispatchingNativeWindow() {
            CreateHandle(new CreateParams());
        }

        /// <summary>
        ///     A global instance which can be used by components that do not need
        ///     their own window.
        /// </summary>
        public static EventDispatchingNativeWindow Instance {
            get {
                lock (myLock) {
                    if (_instance == null)
                        _instance = new EventDispatchingNativeWindow();
                    return _instance;
                }
            }
        }

        /// <summary>
        ///     Attach your event handlers here.
        /// </summary>
        public event WndProcEventHandler EventHandler;

        /// <summary>
        ///     Parse messages passed to this window and send them to the event handlers.
        /// </summary>
        /// <param name="m">
        ///     A System.Windows.Forms.Message that is associated with the
        ///     current Windows message.
        /// </param>
        protected override void WndProc(ref Message m) {
            bool handled = false;
            EventHandler?.Invoke(ref m, ref handled);
            if (!handled)
                base.WndProc(ref m);
        }
    }
}