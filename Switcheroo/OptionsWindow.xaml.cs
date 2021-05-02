using ManagedWinapi;
using Switcheroo.Core;
using Switcheroo.Properties;
using System.Text;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using Application = System.Windows.Application;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using MessageBox = System.Windows.MessageBox;

namespace Switcheroo {
    public partial class OptionsWindow : Window {
        private readonly HotKey _hotkey;
        private HotkeyViewModel _hotkeyViewModel;

        public OptionsWindow() {
            InitializeComponent();

            // Show what's already selected     
            _hotkey = (HotKey)Application.Current.Properties["hotkey"];

            try {
                _hotkey.LoadSettings();
            } catch (HotkeyAlreadyInUseException) {
            }

            _hotkeyViewModel = new HotkeyViewModel {
                KeyCode = KeyInterop.KeyFromVirtualKey((int)_hotkey.KeyCode),
                Alt = _hotkey.Alt,
                Ctrl = _hotkey.Ctrl,
                Windows = _hotkey.WindowsKey,
                Shift = _hotkey.Shift
            };

            HotKeyCheckBox.IsChecked = Settings.Default.EnableHotKey;
            HotkeyPreview.Text = _hotkeyViewModel.ToString();
            HotkeyPreview.IsEnabled = Settings.Default.EnableHotKey;
            AltTabCheckBox.IsChecked = Settings.Default.AltTabHook;
            AutoSwitch.IsChecked = Settings.Default.AutoSwitch;
            AutoSwitch.IsEnabled = Settings.Default.AltTabHook;
            RunAsAdministrator.IsChecked = Settings.Default.RunAsAdmin;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e) {
            Close();
        }

        private void Ok_Click(object sender, RoutedEventArgs e) {
            bool closeOptionsWindow = true;

            try {
                _hotkey.Enabled = false;

                if (Settings.Default.EnableHotKey) {
                    // Change the active hotkey
                    _hotkey.Alt = _hotkeyViewModel.Alt;
                    _hotkey.Shift = _hotkeyViewModel.Shift;
                    _hotkey.Ctrl = _hotkeyViewModel.Ctrl;
                    _hotkey.WindowsKey = _hotkeyViewModel.Windows;
                    _hotkey.KeyCode = (Keys)KeyInterop.VirtualKeyFromKey(_hotkeyViewModel.KeyCode);
                    _hotkey.Enabled = true;
                }

                _hotkey.SaveSettings();
            } catch (HotkeyAlreadyInUseException) {
                string boxText =
                    "Sorry! The selected shortcut for activating Switcheroo is in use by another program. " +
                    "Please choose another.";
                MessageBox.Show(boxText, "Shortcut already in use", MessageBoxButton.OK, MessageBoxImage.Warning);
                closeOptionsWindow = false;
            }

            Settings.Default.EnableHotKey = HotKeyCheckBox.IsChecked.GetValueOrDefault();
            Settings.Default.AltTabHook = AltTabCheckBox.IsChecked.GetValueOrDefault();
            Settings.Default.AutoSwitch = AutoSwitch.IsChecked.GetValueOrDefault();
            Settings.Default.RunAsAdmin = RunAsAdministrator.IsChecked.GetValueOrDefault();
            Settings.Default.Save();

            if (closeOptionsWindow) Close();
        }

        private void HotkeyPreview_OnPreviewKeyDown(object sender, KeyEventArgs e) {
            // The text box grabs all input
            e.Handled = true;

            // Fetch the actual shortcut key
            Key key = e.Key == Key.System ? e.SystemKey : e.Key;

            // Ignore modifier keys
            if (key == Key.LeftShift || key == Key.RightShift
                                     || key == Key.LeftCtrl || key == Key.RightCtrl
                                     || key == Key.LeftAlt || key == Key.RightAlt
                                     || key == Key.LWin || key == Key.RWin)
                return;

            HotkeyViewModel previewHotkeyModel = new() {
                Ctrl = (Keyboard.Modifiers & ModifierKeys.Control) != 0,
                Shift = (Keyboard.Modifiers & ModifierKeys.Shift) != 0,
                Alt = (Keyboard.Modifiers & ModifierKeys.Alt) != 0
            };

            KeyboardKey winLKey = new(Keys.LWin);
            KeyboardKey winRKey = new(Keys.RWin);
            previewHotkeyModel.Windows = (winLKey.State & 0x8000) == 0x8000 || (winRKey.State & 0x8000) == 0x8000;
            previewHotkeyModel.KeyCode = key;

            string previewText = previewHotkeyModel.ToString();

            // Jump to the next element if the user presses only the Tab key
            if (previewText == "Tab") {
                ((UIElement)sender).MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
                return;
            }

            HotkeyPreview.Text = previewText;
            _hotkeyViewModel = previewHotkeyModel;
        }

        private void HotkeyPreview_OnGotFocus(object sender, RoutedEventArgs e) {
            // Disable the current hotkey while the hotkey field is active
            _hotkey.Enabled = false;
        }

        private void HotkeyPreview_OnLostFocus(object sender, RoutedEventArgs e) {
            try {
                _hotkey.Enabled = true;
            } catch (HotkeyAlreadyInUseException) {
                // It is alright if the hotkey can't be reactivated
            }
        }

        private void AltTabCheckBox_OnChecked(object sender, RoutedEventArgs e) {
            AutoSwitch.IsEnabled = true;
        }

        private void AltTabCheckBox_OnUnchecked(object sender, RoutedEventArgs e) {
            AutoSwitch.IsEnabled = false;
            AutoSwitch.IsChecked = false;
        }

        private void HotKeyCheckBox_Checked(object sender, RoutedEventArgs e) {
            HotkeyPreview.IsEnabled = true;
        }

        private void HotKeyCheckBox_OnUnchecked(object sender, RoutedEventArgs e) {
            HotkeyPreview.IsEnabled = false;
        }

        private class HotkeyViewModel {
            public Key KeyCode { get; set; }
            public bool Shift { get; set; }
            public bool Alt { get; set; }
            public bool Ctrl { get; set; }
            public bool Windows { get; set; }

            public override string ToString() {
                StringBuilder shortcutText = new();

                if (Ctrl) shortcutText.Append("Ctrl + ");

                if (Shift) shortcutText.Append("Shift + ");

                if (Alt) shortcutText.Append("Alt + ");

                if (Windows) shortcutText.Append("Win + ");

                string keyString =
                    KeyboardHelper.CodeToString((uint)KeyInterop.VirtualKeyFromKey(KeyCode)).ToUpper().Trim();
                if (keyString.Length == 0) keyString = new KeysConverter().ConvertToString(KeyCode);

                // If the user presses "Escape" then show "Escape" :)
                if (keyString == "\u001B") keyString = "Escape";

                shortcutText.Append(keyString);
                return shortcutText.ToString();
            }
        }
    }
}