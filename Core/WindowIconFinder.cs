using Switcheroo.Core.WinApi;
using System;
using System.ComponentModel;
using System.Drawing;

namespace Switcheroo.Core {
    public class WindowIconFinder {
        public Icon Find(AppWindow window, WindowIconSize size) {
            Icon icon = null;
            try {
                // http://msdn.microsoft.com/en-us/library/windows/desktop/ms632625(v=vs.85).aspx
                IntPtr outvalue = WinApi.WinApi.SendMessageTimeout(window.HWnd, 0x007F,
                    size == WindowIconSize.Small ? new IntPtr(2) : new IntPtr(1),
                    IntPtr.Zero, SendMessageTimeoutFlags.SMTO_ABORTIFHUNG, 100, out IntPtr response);

                if (outvalue == IntPtr.Zero || response == IntPtr.Zero)
                    response = WinApi.WinApi.GetClassLongPtr(window.HWnd,
                        size == WindowIconSize.Small
                            ? ClassLongFlags.GCLP_HICONSM
                            : ClassLongFlags.GCLP_HICON);

                if (response != IntPtr.Zero) {
                    icon = Icon.FromHandle(response);
                } else {
                    string executablePath = window.ExecutablePath;
                    icon = Icon.ExtractAssociatedIcon(executablePath);
                }
            } catch (Win32Exception) {
                // TODO Could not extract icon
            }

            return icon;
        }
    }
}