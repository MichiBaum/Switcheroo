﻿// Copyright by Switcheroo

#region

using Switcheroo.Core.WinApi;
using System;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;

#endregion

namespace Switcheroo.Core {
    // Convert a keycode to the relevant display character
    // http://stackoverflow.com/a/375047/198065
    public class KeyboardHelper {
        public static string CodeToString(uint virtualKey) {
            uint thread =
                WinApi.WinApi.GetWindowThreadProcessId(Process.GetCurrentProcess().MainWindowHandle, out uint procId);
            IntPtr hkl = WinApi.WinApi.GetKeyboardLayout(thread);

            if (hkl == IntPtr.Zero) return string.Empty;

            Keys[] keyStates = new Keys[256];
            if (!WinApi.WinApi.GetKeyboardState(keyStates)) return string.Empty;

            uint scanCode = WinApi.WinApi.MapVirtualKeyEx(virtualKey, MapVirtualKeyMapTypes.MAPVK_VK_TO_CHAR, hkl);

            StringBuilder sb = new(10);

            _ = WinApi.WinApi.ToUnicodeEx(virtualKey, scanCode, new Keys[0], sb, sb.Capacity, 0, hkl);
            return sb.ToString();
        }
    }
}