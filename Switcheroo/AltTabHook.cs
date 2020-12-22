using ManagedWinapi;
using ManagedWinapi.Hooks;
using System;
using System.Windows.Forms;

namespace Switcheroo {
    public delegate void AltTabHookEventHandler(object sender, AltTabHookEventArgs args);

    public class AltTabHookEventArgs : EventArgs {
        public bool CtrlDown { get; set; }
        public bool ShiftDown { get; set; }
        public bool Handled { get; set; }
    }

    public class AltTabHook : IDisposable {
        public event AltTabHookEventHandler Pressed;
        private const int AltKey = 32;
        private const int CtrlKey = 11;
        private readonly KeyboardKey _shiftKey = new KeyboardKey(Keys.LShiftKey);
        private readonly KeyboardKey _ctrlKey = new KeyboardKey(Keys.LControlKey);
        private readonly KeyboardKey _altKey = new KeyboardKey(Keys.LMenu);
        private readonly int WM_KEYDOWN = 0x0100;
        private readonly int WM_SYSKEYDOWN = 0x0104;

        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
        private readonly LowLevelKeyboardHook _lowLevelKeyboardHook;

        public AltTabHook() {
            _lowLevelKeyboardHook = new LowLevelKeyboardHook();
            _lowLevelKeyboardHook.MessageIntercepted += OnMessageIntercepted;
            _lowLevelKeyboardHook.StartHook();
        }

        private void OnMessageIntercepted(LowLevelMessage lowLevelMessage, ref bool handled) {
            if (handled || !(lowLevelMessage is LowLevelKeyboardMessage keyboardMessage)) {
                return;
            }

            if (!IsTabKeyDown(keyboardMessage)) {
                return;
            }

            if (!IsKeyDown(_altKey)) {
                return;
            }

            var shiftKeyDown = IsKeyDown(_shiftKey);
            var ctrlKeyDown = IsKeyDown(_ctrlKey);

            var eventArgs = OnPressed(shiftKeyDown, ctrlKeyDown);

            handled = eventArgs.Handled;
        }

        private static bool IsKeyDown(KeyboardKey keyboardKey) {
            return (keyboardKey.AsyncState & 32768) != 0;
        }

        private bool IsTabKeyDown(LowLevelKeyboardMessage keyboardMessage) {
            return keyboardMessage.VirtualKeyCode == (int)Keys.Tab &&
                   (keyboardMessage.Message == WM_KEYDOWN || keyboardMessage.Message == WM_SYSKEYDOWN);
        }

        private AltTabHookEventArgs OnPressed(bool shiftDown, bool ctrlDown) {
            var altTabHookEventArgs = new AltTabHookEventArgs { ShiftDown = shiftDown, CtrlDown = ctrlDown };
            Pressed?.Invoke(this, altTabHookEventArgs);
            return altTabHookEventArgs;
        }

        public void Dispose() {
            _lowLevelKeyboardHook?.Dispose();
        }
    }
}