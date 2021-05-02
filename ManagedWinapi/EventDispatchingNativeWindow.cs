using System;
using System.Windows.Forms;

namespace ManagedWinapi.Windows {
    /// <summary>
    ///     A Win32 native window that delegates window messages to handlers. So several
    ///     components can use the same native window to save "USER resources". This class
    ///     is useful when writing your own components.
    /// </summary>
    public class EventDispatchingNativeWindow : NativeWindow {
        private static readonly Object myLock = new();
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