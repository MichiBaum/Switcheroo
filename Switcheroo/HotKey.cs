// Copyright by Switcheroo

#region

using ManagedWinapi;
using Switcheroo.Properties;
using System.Windows.Forms;

#endregion

namespace Switcheroo {
    public class HotKey : Hotkey {
        public void LoadSettings() {
            KeyCode = (Keys)Settings.Default.HotKey;
            WindowsKey = Settings.Default.WindowsKey;
            Alt = Settings.Default.Alt;
            Ctrl = Settings.Default.Ctrl;
            Shift = Settings.Default.Shift;
        }

        public void SaveSettings() {
            Settings.Default.HotKey = (int)KeyCode;
            Settings.Default.WindowsKey = WindowsKey;
            Settings.Default.Alt = Alt;
            Settings.Default.Ctrl = Ctrl;
            Settings.Default.Shift = Shift;
            Settings.Default.Save();
        }
    }
}