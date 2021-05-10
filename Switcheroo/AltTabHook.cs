using ManagedWinapi;
using ManagedWinapi.Hooks;
using System;
using System.Windows.Forms;

namespace Switcheroo {
    public class AltTabHook : IDisposable {
        private const int AltKey = 32;
        private const int CtrlKey = 11;
        private readonly KeyboardKey _altKey = new(Keys.LMenu);
        private readonly KeyboardKey _ctrlKey = new(Keys.LControlKey);

        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
        private readonly LowLevelKeyboardHook _lowLevelKeyboardHook;
        private readonly KeyboardKey _shiftKey = new(Keys.LShiftKey);
        private const int WmKeydown = 0x0100;
        private const int WmSyskeydown = 0x0104;

        public AltTabHook() {
            _lowLevelKeyboardHook = new LowLevelKeyboardHook();
            _lowLevelKeyboardHook.MessageIntercepted += OnMessageIntercepted;
            _lowLevelKeyboardHook.StartHook();
        }

        public void Dispose() {
            _lowLevelKeyboardHook?.Dispose();
        }

        public event AltTabHookEventHandler Pressed;

        private void OnMessageIntercepted(LowLevelMessage lowLevelMessage, ref bool handled) {
            if (handled || lowLevelMessage is not LowLevelKeyboardMessage keyboardMessage) return;

            if (!IsTabKeyDown(keyboardMessage)) return;

            if (!IsKeyDown(_altKey)) return;

            bool shiftKeyDown = IsKeyDown(_shiftKey);
            bool ctrlKeyDown = IsKeyDown(_ctrlKey);

            AltTabHookEventArgs eventArgs = OnPressed(shiftKeyDown, ctrlKeyDown);

            handled = eventArgs.Handled;
        }

        private static bool IsKeyDown(KeyboardKey keyboardKey) {
            return (keyboardKey.AsyncState & 32768) != 0;
        }

        private bool IsTabKeyDown(LowLevelKeyboardMessage keyboardMessage) {
            return keyboardMessage.VirtualKeyCode == (int)Keys.Tab &&
                   (keyboardMessage.Message == WmKeydown || keyboardMessage.Message == WmSyskeydown);
        }

        private AltTabHookEventArgs OnPressed(bool shiftDown, bool ctrlDown) {
            AltTabHookEventArgs altTabHookEventArgs =
                new() {ShiftDown = shiftDown, CtrlDown = ctrlDown};
            Pressed?.Invoke(this, altTabHookEventArgs);
            return altTabHookEventArgs;
        }
    }
}