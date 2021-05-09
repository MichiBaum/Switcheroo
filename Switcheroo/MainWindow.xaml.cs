using ManagedWinapi;
using ManagedWinapi.Windows;
using Switcheroo.Core;
using Switcheroo.Core.Matchers;
using Switcheroo.Properties;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using Application = System.Windows.Application;
using MessageBox = System.Windows.MessageBox;

namespace Switcheroo {
    public partial class MainWindow : Window {
        public static readonly RoutedUICommand CloseWindowCommand = new();
        public static readonly RoutedUICommand SwitchToWindowCommand = new();
        public static readonly RoutedUICommand ScrollListDownCommand = new();
        public static readonly RoutedUICommand ScrollListUpCommand = new();
        private AboutWindow _aboutWindow;
        private bool _altTabAutoSwitch;
        private AltTabHook _altTabHook;
        private ObservableCollection<AppWindowViewModel> _filteredWindowList;
        private SystemWindow _foregroundWindow;
        private HotKey _hotkey;
        private NotifyIcon _notifyIcon;
        private OptionsWindow _optionsWindow;
        private List<AppWindowViewModel> _unfilteredWindowList;
        private WindowCloser _windowCloser;

        public MainWindow() {
            InitializeComponent();

            SetUpKeyBindings();

            SetUpNotifyIcon();

            SetUpHotKey();

            SetUpAltTabHook();

            CheckForUpdates();

            Opacity = 0;
        }

        private enum InitialFocus {
            NextItem,
            PreviousItem
        }

        private void SetUpKeyBindings() {
            // Enter and Esc bindings are not executed before the keys have been released.
            // This is done to prevent that the window being focused after the key presses
            // to get 'KeyUp' messages.

            KeyDown += (sender, args) => {
                // Opacity is set to 0 right away so it appears that action has been taken right away...
                if (args.Key == Key.Enter && !Keyboard.Modifiers.HasFlag(ModifierKeys.Control)) {
                    Opacity = 0;
                } else if (args.Key == Key.Escape) {
                    Opacity = 0;
                } else if (args.SystemKey == Key.S && Keyboard.Modifiers.HasFlag(ModifierKeys.Alt)) {
                    _altTabAutoSwitch = false;
                    tb.Text = "";
                    tb.IsEnabled = true;
                    tb.Focus();
                }
            };

            KeyUp += (sender, args) => {
                // ... But only when the keys are release, the action is actually executed
                if (args.Key == Key.Enter && !Keyboard.Modifiers.HasFlag(ModifierKeys.Control))
                    Switch();
                else if (args.Key == Key.Escape)
                    HideWindow();
                else if (args.SystemKey == Key.LeftAlt && !Keyboard.Modifiers.HasFlag(ModifierKeys.Control))
                    Switch();
                else if (args.Key == Key.LeftAlt && _altTabAutoSwitch) Switch();
            };
        }

        private void SetUpHotKey() {
            _hotkey = new HotKey();
            _hotkey.LoadSettings();

            Application.Current.Properties["hotkey"] = _hotkey;

            _hotkey.HotkeyPressed += hotkey_HotkeyPressed;
            try {
                _hotkey.Enabled = Settings.Default.EnableHotKey;
            } catch (HotkeyAlreadyInUseException) {
                string boxText =
                    language_en
                        .MainWindow_SetUpHotKey_The_current_hotkey_for_activating_Switcheroo_is_in_use_by_another_program_ +
                    Environment.NewLine +
                    Environment.NewLine +
                    language_en
                        .MainWindow_SetUpHotKey_You_can_change_the_hotkey_by_right_clicking_the_Switcheroo_icon_in_the_system_tray_and_choosing__Options__;
                MessageBox.Show(boxText, language_en.MainWindow_SetUpHotKey_Hotkey_already_in_use, MessageBoxButton.OK,
                    MessageBoxImage.Warning);
            }
        }

        private void SetUpAltTabHook() {
            _altTabHook = new AltTabHook();
            _altTabHook.Pressed += AltTabPressed;
        }

        private void SetUpNotifyIcon() {
            // TODO doesnt work as intended

            Icon icon = Properties.Resources.icon;

            ToolStripMenuItem runOnStartupMenuItem =
                new(language_en.MainWindow_SetUpNotifyIcon_Run_on_Startup) {Checked = new AutoStart().IsEnabled};
            runOnStartupMenuItem.Click += (s, e) => RunOnStartup((ToolStripMenuItem)s);

            ToolStripMenuItem optionsMenuItem = new(language_en.MainWindow_SetUpNotifyIcon_Options);
            runOnStartupMenuItem.Click += (s, e) => Options();

            ToolStripMenuItem aboutMenuItem = new(language_en.MainWindow_SetUpNotifyIcon_About);
            runOnStartupMenuItem.Click += (s, e) => About();

            ToolStripMenuItem exitMenuItem = new(language_en.MainWindow_SetUpNotifyIcon_Exit);
            runOnStartupMenuItem.Click += (s, e) => Quit();

            _notifyIcon = new NotifyIcon {
                Text = language_en.MainWindow_SetUpNotifyIcon_Switcheroo,
                Icon = icon,
                Visible = true,
                ContextMenuStrip = new ContextMenuStrip {
                    Items = {runOnStartupMenuItem, optionsMenuItem, aboutMenuItem, exitMenuItem}
                }
            };
        }

        private static void RunOnStartup(ToolStripMenuItem menuItem) {
            try {
                AutoStart autoStart = new() {IsEnabled = !menuItem.Checked};
                menuItem.Checked = autoStart.IsEnabled;
            } catch (AutoStartException e) {
                MessageBox.Show(e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private static void CheckForUpdates() {
            Version currentVersion = Assembly.GetEntryAssembly().GetName().Version;
            if (currentVersion == new Version(0, 0, 0, 0)) return;

            DispatcherTimer timer = new();

            timer.Tick += async (sender, args) => {
                timer.Stop();
                Version latestVersion = await GetLatestVersion();
                if (latestVersion != null && latestVersion > currentVersion) {
                    MessageBoxResult result = MessageBox.Show(
                        string.Format(
                            language_en.MainWindow_CheckForUpdates_,
                            latestVersion, currentVersion),
                        "Update Available", MessageBoxButton.YesNo, MessageBoxImage.Information);
                    if (result ==
                        MessageBoxResult
                            .Yes) // TODO can throw exception like https://stackoverflow.com/a/53245993/10258204
                        Process.Start("https://github.com/MichiBaum/Switcheroo/releases/latest");
                } else {
                    timer.Interval = new TimeSpan(24, 0, 0);
                    timer.Start();
                }
            };

            timer.Interval = new TimeSpan(0, 0, 0);
            timer.Start();
        }

        private static async Task<Version?> GetLatestVersion() {
            try {
                // TODO add own versioning
                string versionAsString = await new WebClient()
                    .DownloadStringTaskAsync("https://raw.github.com/MichiBaum/Switcheroo/update/version.txt")
                    .ConfigureAwait(false);
                if (Version.TryParse(versionAsString, out Version newVersion)) return newVersion;
            } catch (WebException) {
            }

            return null;
        }

        /// <summary>
        ///     Populates the window list with the current running windows.
        /// </summary>
        private void LoadData(InitialFocus focus) {
            _unfilteredWindowList =
                new WindowFinder().GetWindows().ConvertAll(window => new AppWindowViewModel(window));

            AppWindowViewModel firstWindow = _unfilteredWindowList.FirstOrDefault();

            bool foregroundWindowMovedToBottom = false;

            // Move first window to the bottom of the list if it's related to the foreground window
            if (firstWindow != null && AreWindowsRelated(firstWindow.AppWindow, _foregroundWindow)) {
                _unfilteredWindowList.RemoveAt(0);
                _unfilteredWindowList.Add(firstWindow);
                foregroundWindowMovedToBottom = true;
            }

            _filteredWindowList = new ObservableCollection<AppWindowViewModel>(_unfilteredWindowList);
            _windowCloser = new WindowCloser();

            foreach (AppWindowViewModel window in _unfilteredWindowList) {
                window.FormattedTitle = new XamlHighlighter().Highlight(new[] {new StringPart(window.AppWindow.Title)});
                window.FormattedProcessTitle =
                    new XamlHighlighter().Highlight(new[] {new StringPart(window.AppWindow.ProcessTitle)});
            }

            lb.DataContext = null;
            lb.DataContext = _filteredWindowList;

            FocusItemInList(focus, foregroundWindowMovedToBottom);

            tb.Clear();
            tb.Focus();
            CenterWindow();
            ScrollSelectedItemIntoView();
        }

        private static bool AreWindowsRelated(SystemWindow window1, SystemWindow window2) {
            return window1.HWnd == window2.HWnd || window1.Process.Id == window2.Process.Id;
        }

        private void FocusItemInList(InitialFocus focus, bool foregroundWindowMovedToBottom) {
            if (focus == InitialFocus.PreviousItem) {
                int previousItemIndex = lb.Items.Count - 1;
                if (foregroundWindowMovedToBottom) previousItemIndex--;

                lb.SelectedIndex = previousItemIndex > 0 ? previousItemIndex : 0;
            } else {
                lb.SelectedIndex = 0;
            }
        }

        /// <summary>
        ///     Place the Switcheroo window in the center of the screen
        /// </summary>
        private void CenterWindow() {
            // Reset height every time to ensure that resolution changes take effect
            Border.MaxHeight = SystemParameters.PrimaryScreenHeight;

            // Force a rendering before repositioning the window
            SizeToContent = SizeToContent.Manual;
            SizeToContent = SizeToContent.WidthAndHeight;

            // Position the window in the center of the screen
            Left = (SystemParameters.PrimaryScreenWidth / 2) - (ActualWidth / 2);
            Top = (SystemParameters.PrimaryScreenHeight / 2) - (ActualHeight / 2);
        }

        /// <summary>
        ///     Switches the window associated with the selected item.
        /// </summary>
        private void Switch() {
            foreach (object item in lb.SelectedItems) {
                AppWindowViewModel win = (AppWindowViewModel)item;
                win.AppWindow.SwitchToLastVisibleActivePopup();
            }

            HideWindow();
        }

        private void HideWindow() {
            if (_windowCloser != null) {
                _windowCloser.Dispose();
                _windowCloser = null;
            }

            _altTabAutoSwitch = false;
            Opacity = 0;
            Dispatcher.BeginInvoke(new Action(Hide), DispatcherPriority.Input);
        }

        /// <summary>
        ///     Show Options dialog.
        /// </summary>
        private void Options() {
            if (_optionsWindow == null) {
                _optionsWindow = new OptionsWindow {WindowStartupLocation = WindowStartupLocation.CenterScreen};
                _optionsWindow.Closed += (sender, args) => _optionsWindow = null;
                _optionsWindow.ShowDialog();
            } else {
                _optionsWindow.Activate();
            }
        }

        /// <summary>
        ///     Show About dialog.
        /// </summary>
        private void About() {
            if (_aboutWindow == null) {
                _aboutWindow = new AboutWindow {WindowStartupLocation = WindowStartupLocation.CenterScreen};
                _aboutWindow.Closed += (sender, args) => _aboutWindow = null;
                _aboutWindow.ShowDialog();
            } else {
                _aboutWindow.Activate();
            }
        }

        /// <summary>
        ///     Quit Switcheroo
        /// </summary>
        private void Quit() {
            _notifyIcon.Dispose();
            _notifyIcon = null;
            _hotkey.Dispose();
            Application.Current.Shutdown();
        }
        
        private void OnClose(object sender, CancelEventArgs e) {
            e.Cancel = true;
            HideWindow();
        }

        private void hotkey_HotkeyPressed(object sender, EventArgs e) {
            if (!Settings.Default.EnableHotKey) return;

            if (Visibility != Visibility.Visible) {
                tb.IsEnabled = true;

                _foregroundWindow = SystemWindow.ForegroundWindow;
                Show();
                Activate();
                Keyboard.Focus(tb);
                LoadData(InitialFocus.NextItem);
                Opacity = 1;
            } else {
                HideWindow();
            }
        }

        private void AltTabPressed(object sender, AltTabHookEventArgs e) {
            if (!Settings.Default.AltTabHook) // Ignore Alt+Tab presses if the hook is not activated by the user
                return;

            _foregroundWindow = SystemWindow.ForegroundWindow;

            if (_foregroundWindow.ClassName ==
                "MultitaskingViewFrame") // If Windows' task switcher is on the screen then don't do anything
                return;

            e.Handled = true;

            if (Visibility != Visibility.Visible) {
                tb.IsEnabled = true;

                ActivateAndFocusMainWindow();

                Keyboard.Focus(tb);
                if (e.ShiftDown)
                    LoadData(InitialFocus.PreviousItem);
                else
                    LoadData(InitialFocus.NextItem);

                if (Settings.Default.AutoSwitch && !e.CtrlDown) {
                    _altTabAutoSwitch = true;
                    tb.IsEnabled = false;
                    tb.Text = language_en.MainWindow_AltTabPressed_Press_Alt___S_to_search;
                }

                Opacity = 1;
            } else {
                if (e.ShiftDown)
                    PreviousItem();
                else
                    NextItem();
            }
        }

        private void ActivateAndFocusMainWindow() {
            // What happens below looks a bit weird, but for Switcheroo to get focus when using the Alt+Tab hook,
            // it is needed to simulate an Alt keypress will bring Switcheroo to the foreground. Otherwise Switcheroo
            // will become the foreground window, but the previous window will retain focus, and receive keep getting
            // the keyboard input.
            // http://www.codeproject.com/Tips/76427/How-to-bring-window-to-top-with-SetForegroundWindo

            IntPtr thisWindowHandle = new WindowInteropHelper(this).Handle;
            AppWindow thisWindow = new(thisWindowHandle);

            KeyboardKey altKey = new(Keys.Alt);
            bool altKeyPressed = false;

            // Press the Alt key if it is not already being pressed
            if ((altKey.AsyncState & 0x8000) == 0) {
                altKey.Press();
                altKeyPressed = true;
            }

            // Bring the Switcheroo window to the foreground
            Show();
            SystemWindow.ForegroundWindow = thisWindow;
            Activate();

            // Release the Alt key if it was pressed above
            if (altKeyPressed) altKey.Release();
        }

        private void TextChanged(object sender, TextChangedEventArgs args) {
            if (!tb.IsEnabled) return;

            string query = tb.Text;

            WindowFilterContext<AppWindowViewModel> context = new() {
                Windows = _unfilteredWindowList,
                ForegroundWindowProcessTitle = new AppWindow(_foregroundWindow.HWnd).ProcessTitle
            };

            List<FilterResult<AppWindowViewModel>> filterResults = new WindowFilterer().Filter(context, query).ToList();

            foreach (FilterResult<AppWindowViewModel> filterResult in filterResults) {
                filterResult.AppWindow.FormattedTitle =
                    GetFormattedTitleFromBestResult(filterResult.WindowTitleMatchResults);
                filterResult.AppWindow.FormattedProcessTitle =
                    GetFormattedTitleFromBestResult(filterResult.ProcessTitleMatchResults);
            }

            _filteredWindowList = new ObservableCollection<AppWindowViewModel>(filterResults.Select(r => r.AppWindow));
            lb.DataContext = _filteredWindowList;
            if (lb.Items.Count > 0) lb.SelectedItem = lb.Items[0];
        }

        private static string GetFormattedTitleFromBestResult(IList<MatchResult> matchResults) {
            MatchResult bestResult = matchResults.FirstOrDefault(r => r.Matched) ?? matchResults.First();
            return new XamlHighlighter().Highlight(bestResult.StringParts);
        }

        private void OnEnterPressed(object sender, ExecutedRoutedEventArgs e) {
            Switch();
            e.Handled = true;
        }

        private void ListBoxItem_MouseDoubleClick(object sender, MouseButtonEventArgs e) {
            Switch();
            e.Handled = true;
        }

        private async void CloseWindow(object sender, ExecutedRoutedEventArgs e) {
            List<AppWindowViewModel> windows = lb.SelectedItems.Cast<AppWindowViewModel>().ToList();
            foreach (AppWindowViewModel win in windows) {
                bool isClosed = await _windowCloser.TryCloseAsync(win);
                if (isClosed)
                    RemoveWindow(win);
            }

            if (lb.Items.Count == 0)
                HideWindow();

            e.Handled = true;
        }

        private void RemoveWindow(AppWindowViewModel window) {
            int index = _filteredWindowList.IndexOf(window);
            if (index < 0)
                return;

            if (lb.SelectedIndex == index) {
                if (_filteredWindowList.Count > index + 1) {
                    lb.SelectedIndex++;
                } else {
                    if (index > 0)
                        lb.SelectedIndex--;
                }
            }

            _filteredWindowList.Remove(window);
            _unfilteredWindowList.Remove(window);
        }

        private void ScrollListUp(object sender, ExecutedRoutedEventArgs e) {
            PreviousItem();
            e.Handled = true;
        }

        private void PreviousItem() {
            if (lb.Items.Count > 0) {
                if (lb.SelectedIndex != 0)
                    lb.SelectedIndex--;
                else
                    lb.SelectedIndex = lb.Items.Count - 1;

                ScrollSelectedItemIntoView();
            }
        }

        private void ScrollListDown(object sender, ExecutedRoutedEventArgs e) {
            NextItem();
            e.Handled = true;
        }

        private void NextItem() {
            if (lb.Items.Count > 0) {
                if (lb.SelectedIndex != lb.Items.Count - 1)
                    lb.SelectedIndex++;
                else
                    lb.SelectedIndex = 0;

                ScrollSelectedItemIntoView();
            }
        }

        private void ScrollSelectedItemIntoView() {
            object selectedItem = lb.SelectedItem;
            if (selectedItem != null) lb.ScrollIntoView(selectedItem);
        }

        private void MainWindow_OnLostFocus(object sender, EventArgs e) {
            HideWindow();
        }

        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e) {
            DisableSystemMenu();
        }

        private void DisableSystemMenu() {
            IntPtr windowHandle = new WindowInteropHelper(this).Handle;
            SystemWindow window = new(windowHandle);
            window.Style = window.Style & ~WindowStyleFlags.SYSMENU;
        }

        private void ShowHelpTextBlock_OnPreviewMouseDown(object sender, MouseButtonEventArgs e) {
            Duration duration = new(TimeSpan.FromSeconds(0.150));
            int newHeight = HelpPanel.Height > 0 ? 0 : +17;
            HelpPanel.BeginAnimation(HeightProperty, new DoubleAnimation(HelpPanel.Height, newHeight, duration));
        }
    }
}