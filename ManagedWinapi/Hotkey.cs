using ManagedWinapi.Windows;
using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace ManagedWinapi {
    /// <summary>
    ///     Specifies a component that creates a global keyboard hotkey.
    /// </summary>
    [DefaultEvent("HotkeyPressed")]
    public class Hotkey : Component {
        private static readonly Object myStaticLock = new();
        private static int hotkeyCounter = 0xA000;

        private static readonly int MOD_ALT = 0x0001,
            MOD_CONTROL = 0x0002,
            MOD_SHIFT = 0x0004,
            MOD_WIN = 0x0008;

        private static readonly int WM_HOTKEY = 0x0312;

        private readonly int hotkeyIndex;
        private readonly IntPtr hWnd;
        private bool _ctrl, _alt, _shift, _windows;
        private Keys _keyCode;
        private bool isDisposed, isEnabled, isRegistered;

        /// <summary>
        ///     Initializes a new instance of this class with the specified container.
        /// </summary>
        /// <param name="container">The container to add it to.</param>
        public Hotkey(IContainer container) : this() {
            container.Add(this);
        }

        /// <summary>
        ///     Initializes a new instance of this class.
        /// </summary>
        public Hotkey() {
            EventDispatchingNativeWindow.Instance.EventHandler += nw_EventHandler;
            lock (myStaticLock) {
                hotkeyIndex = ++hotkeyCounter;
            }

            hWnd = EventDispatchingNativeWindow.Instance.Handle;
        }

        /// <summary>
        ///     Enables the hotkey. When the hotkey is enabled, pressing it causes a
        ///     <c>HotkeyPressed</c> event instead of being handled by the active
        ///     application.
        /// </summary>
        public bool Enabled {
            get => isEnabled;
            set {
                isEnabled = value;
                updateHotkey(false);
            }
        }

        /// <summary>
        ///     The key code of the hotkey.
        /// </summary>
        public Keys KeyCode {
            get => _keyCode;

            set {
                _keyCode = value;
                updateHotkey(true);
            }
        }

        /// <summary>
        ///     Whether the shortcut includes the Control modifier.
        /// </summary>
        public bool Ctrl {
            get => _ctrl;
            set {
                _ctrl = value;
                updateHotkey(true);
            }
        }

        /// <summary>
        ///     Whether this shortcut includes the Alt modifier.
        /// </summary>
        public bool Alt {
            get => _alt;
            set {
                _alt = value;
                updateHotkey(true);
            }
        }

        /// <summary>
        ///     Whether this shortcut includes the shift modifier.
        /// </summary>
        public bool Shift {
            get => _shift;
            set {
                _shift = value;
                updateHotkey(true);
            }
        }

        /// <summary>
        ///     Whether this shortcut includes the Windows key modifier. The windows key
        ///     is an addition by Microsoft to the keyboard layout. It is located between
        ///     Control and Alt and depicts a Windows flag.
        /// </summary>
        public bool WindowsKey {
            get => _windows;
            set {
                _windows = value;
                updateHotkey(true);
            }
        }

        /// <summary>
        ///     Occurs when the hotkey is pressed.
        /// </summary>
        public event EventHandler HotkeyPressed;

        private void nw_EventHandler(ref Message m, ref bool handled) {
            if (handled)
                return;
            if (m.Msg == WM_HOTKEY && m.WParam.ToInt32() == hotkeyIndex) {
                if (HotkeyPressed != null)
                    HotkeyPressed(this, EventArgs.Empty);
                handled = true;
            }
        }

        /// <summary>
        ///     Releases all resources used by the System.ComponentModel.Component.
        /// </summary>
        /// <param name="disposing">Whether to dispose managed resources.</param>
        protected override void Dispose(bool disposing) {
            isDisposed = true;
            updateHotkey(false);
            EventDispatchingNativeWindow.Instance.EventHandler -= nw_EventHandler;
            base.Dispose(disposing);
        }

        private void updateHotkey(bool reregister) {
            bool shouldBeRegistered = isEnabled && !isDisposed && !DesignMode;
            if (isRegistered && (!shouldBeRegistered || reregister)) {
                // unregister hotkey
                UnregisterHotKey(hWnd, hotkeyIndex);
                isRegistered = false;
            }

            if (!isRegistered && shouldBeRegistered) {
                // register hotkey
                bool success = RegisterHotKey(hWnd, hotkeyIndex,
                    (_shift ? MOD_SHIFT : 0) + (_ctrl ? MOD_CONTROL : 0) +
                    (_alt ? MOD_ALT : 0) + (_windows ? MOD_WIN : 0), (int)_keyCode);
                if (!success)
                    throw new HotkeyAlreadyInUseException();
                isRegistered = true;
            }
        }

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vlc);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);
    }
}