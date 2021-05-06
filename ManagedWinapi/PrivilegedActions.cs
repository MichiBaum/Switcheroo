using System;
using System.Runtime.InteropServices;

namespace ManagedWinapi {
    /// <summary>
    ///     Collection of miscellaneous actions that cannot be performed as
    ///     a non-administrative user, like shutdown or setting the system time.
    /// </summary>
    public static class PrivilegedActions {
        private const uint SHTDN_REASON_FLAG_PLANNED = 0x80000000;

        /// <summary>
        ///     Get or set the system time in the local timezone.
        /// </summary>
        public static DateTime LocalTime {
            get {
                SYSTEMTIME st = new();
                ApiHelper.FailIfZero(GetLocalTime(ref st));
                return st.ToDateTime();
            }

            set {
                SYSTEMTIME st = new(value);
                // Set it twice due to possible daylight savings change
                ApiHelper.FailIfZero(SetLocalTime(ref st));
                ApiHelper.FailIfZero(SetLocalTime(ref st));
            }
        }

        /// <summary>
        ///     Get or set the system time, in UTC.
        /// </summary>
        public static DateTime SystemTime {
            get {
                SYSTEMTIME st = new();
                ApiHelper.FailIfZero(GetSystemTime(ref st));
                return st.ToDateTime();
            }

            set {
                SYSTEMTIME st = new(value);
                ApiHelper.FailIfZero(SetLocalTime(ref st));
            }
        }

        /// <summary>
        ///     Shutdown the system.
        /// </summary>
        public static void ShutDown(ShutdownAction action) {
            ShutDown(action, ShutdownForceMode.NoForce);
        }

        /// <summary>
        ///     Shutdown the system.
        /// </summary>
        public static void ShutDown(ShutdownAction action, ShutdownForceMode forceMode) {
            ApiHelper.FailIfZero(ExitWindowsEx((uint)action | (uint)forceMode, SHTDN_REASON_FLAG_PLANNED));
        }

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int ExitWindowsEx(uint uFlags, uint dwReason);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern int GetSystemTime(ref SYSTEMTIME lpSystemTime);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern int SetSystemTime(ref SYSTEMTIME lpSystemTime);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern int GetLocalTime(ref SYSTEMTIME lpSystemTime);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern int SetLocalTime(ref SYSTEMTIME lpSystemTime);

        private struct SYSTEMTIME {
            internal readonly ushort wYear;

            internal readonly ushort wMonth;

            internal ushort wDayOfWeek;

            internal readonly ushort wDay;

            internal readonly ushort wHour;
            internal readonly ushort wMinute;
            internal readonly ushort wSecond;
            internal readonly ushort wMilliseconds;

            internal SYSTEMTIME(DateTime time) {
                wYear = (ushort)time.Year;
                wMonth = (ushort)time.Month;
                wDayOfWeek = (ushort)time.DayOfWeek;
                wDay = (ushort)time.Day;
                wHour = (ushort)time.Hour;
                wMinute = (ushort)time.Minute;
                wSecond = (ushort)time.Second;
                wMilliseconds = (ushort)time.Millisecond;
            }

            internal DateTime ToDateTime() {
                return new(wYear, wMonth, wDay, wHour, wMinute, wSecond, wMilliseconds);
            }
        }
    }
}