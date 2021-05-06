using ManagedWinapi.Windows;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.Caching;
using System.Runtime.InteropServices;
using System.Text;

namespace Switcheroo.Core {
    /// <summary>
    ///     This class is a wrapper around the Win32 api window handles
    /// </summary>
    public class AppWindow : SystemWindow {
        public AppWindow(IntPtr HWnd) : base(HWnd) {
        }

        public string ProcessTitle {
            get {
                string key = "ProcessTitle-" + HWnd;
                string? processTitle = MemoryCache.Default.Get(key) as string;
                if (processTitle == null) {
                    if (IsApplicationFrameWindow()) {
                        processTitle = "UWP";

                        Process? underlyingProcess = AllChildWindows.Where(w => w.Process.Id != Process.Id)
                            .Select(w => w.Process)
                            .FirstOrDefault();

                        if (underlyingProcess != null && underlyingProcess.ProcessName != "")
                            processTitle = underlyingProcess.ProcessName;
                    } else {
                        processTitle = Process.ProcessName;
                    }

                    MemoryCache.Default.Add(key, processTitle, DateTimeOffset.Now.AddHours(1));
                }

                return processTitle;
            }
        }

        public Icon LargeWindowIcon => new WindowIconFinder().Find(this, WindowIconSize.Large);

        public Icon SmallWindowIcon => new WindowIconFinder().Find(this, WindowIconSize.Small);

        public string ExecutablePath => GetExecutablePath(Process.Id);

        public AppWindow? Owner {
            get {
                IntPtr ownerHandle = WinApi.GetWindow(HWnd, WinApi.GetWindowCmd.GW_OWNER);
                if (ownerHandle == IntPtr.Zero)
                    return null;
                return new AppWindow(ownerHandle);
            }
        }

        public static new IEnumerable<AppWindow> AllToplevelWindows =>
            SystemWindow.AllToplevelWindows
                .Select(w => new AppWindow(w.HWnd));

        /// <summary>
        ///     Sets the focus to this window and brings it to the foreground.
        /// </summary>
        public void SwitchTo() {
            // This function is deprecated, so should probably be replaced.
            WinApi.SwitchToThisWindow(HWnd, true);
        }

        public void SwitchToLastVisibleActivePopup() {
            IntPtr lastActiveVisiblePopup = GetLastActiveVisiblePopup();
            WinApi.SwitchToThisWindow(lastActiveVisiblePopup, true);
        }

        public bool IsAltTabWindow() {
            if (!Visible)
                return false;
            if (!HasWindowTitle())
                return false;
            if (IsAppWindow())
                return true;
            if (IsToolWindow())
                return false;
            if (IsNoActivate())
                return false;
            if (!IsOwnerOrOwnerNotVisible())
                return false;
            if (HasITaskListDeletedProperty())
                return false;
            if (IsCoreWindow())
                return false;
            if (IsApplicationFrameWindow() && !HasAppropriateApplicationViewCloakType())
                return false;

            return true;
        }

        private bool HasWindowTitle() {
            return !string.IsNullOrEmpty(Title);
        }

        private bool IsToolWindow() {
            return (ExtendedStyle & WindowExStyleFlags.TOOLWINDOW) == WindowExStyleFlags.TOOLWINDOW
                   || (Style & WindowStyleFlags.TOOLWINDOW) == WindowStyleFlags.TOOLWINDOW;
        }

        private bool IsAppWindow() {
            return (ExtendedStyle & WindowExStyleFlags.APPWINDOW) == WindowExStyleFlags.APPWINDOW;
        }

        private bool IsNoActivate() {
            return (ExtendedStyle & WindowExStyleFlags.NOACTIVATE) == WindowExStyleFlags.NOACTIVATE;
        }

        private IntPtr GetLastActiveVisiblePopup() {
            // Which windows appear in the Alt+Tab list? -Raymond Chen
            // http://blogs.msdn.com/b/oldnewthing/archive/2007/10/08/5351207.aspx

            // Start at the root owner
            IntPtr hwndWalk = WinApi.GetAncestor(HWnd, WinApi.GetAncestorFlags.GetRootOwner);

            // See if we are the last active visible popup
            IntPtr hwndTry = IntPtr.Zero;
            while (hwndWalk != hwndTry) {
                hwndTry = hwndWalk;
                hwndWalk = WinApi.GetLastActivePopup(hwndTry);
                if (WinApi.IsWindowVisible(hwndWalk)) return hwndWalk;
            }

            return hwndWalk;
        }

        private bool IsOwnerOrOwnerNotVisible() {
            return Owner?.Visible != true;
        }

        private bool HasITaskListDeletedProperty() {
            return WinApi.GetProp(HWnd, "ITaskList_Deleted") != IntPtr.Zero;
        }

        private bool IsCoreWindow() {
            // Avoids double entries for Windows Store Apps on Windows 10
            return ClassName == "Windows.UI.Core.CoreWindow";
        }

        private bool IsApplicationFrameWindow() {
            // Is a UWP application
            return ClassName == "ApplicationFrameWindow";
        }

        private bool HasAppropriateApplicationViewCloakType() {
            // The ApplicationFrameWindows that host Windows Store Apps like to
            // hang around in Windows 10 even after the underlying program has been
            // closed. A way to figure out if the ApplicationFrameWindow is
            // currently hosting an application is to check if it has a property called
            // "ApplicationViewCloakType", and that the value != 1.
            //
            // I've stumbled upon these values of "ApplicationViewCloakType":
            //    0 = Program is running on current virtual desktop
            //    1 = Program is not running
            //    2 = Program is running on a different virtual desktop

            bool hasAppropriateApplicationViewCloakType = false;
            WinApi.EnumPropsEx(HWnd, (_, lpszString, data, __) => {
                string propName = Marshal.PtrToStringAnsi(lpszString);
                if (propName == "ApplicationViewCloakType") {
                    hasAppropriateApplicationViewCloakType = data != 1;
                    return 0;
                }

                return 1;
            }, IntPtr.Zero);

            return hasAppropriateApplicationViewCloakType;
        }

        // This method only works on Windows >= Windows Vista
        private static string GetExecutablePath(int processId) {
            StringBuilder buffer = new(1024);
            IntPtr hprocess = WinApi.OpenProcess(WinApi.ProcessAccess.QueryLimitedInformation, false, processId);
            if (hprocess == IntPtr.Zero)
                throw new Win32Exception(Marshal.GetLastWin32Error());

            try {
                // ReSharper disable once RedundantAssignment
                int size = buffer.Capacity;
                if (WinApi.QueryFullProcessImageName(hprocess, 0, buffer, out size)) return buffer.ToString();
            } finally {
                WinApi.CloseHandle(hprocess);
            }

            throw new Win32Exception(Marshal.GetLastWin32Error());
        }
    }
}