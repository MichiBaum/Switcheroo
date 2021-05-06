// Copyright by Switcheroo

#region

using System;
using System.Runtime.InteropServices;

#endregion

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
            private readonly ushort _wYear;

            private readonly ushort _wMonth;

            private ushort _wDayOfWeek;

            private readonly ushort _wDay;

            private readonly ushort _wHour;
            private readonly ushort _wMinute;
            private readonly ushort _wSecond;
            private readonly ushort _wMilliseconds;

            internal SYSTEMTIME(DateTime time) {
                _wYear = (ushort)time.Year;
                _wMonth = (ushort)time.Month;
                _wDayOfWeek = (ushort)time.DayOfWeek;
                _wDay = (ushort)time.Day;
                _wHour = (ushort)time.Hour;
                _wMinute = (ushort)time.Minute;
                _wSecond = (ushort)time.Second;
                _wMilliseconds = (ushort)time.Millisecond;
            }

            internal DateTime ToDateTime() {
                return new(_wYear, _wMonth, _wDay, _wHour, _wMinute, _wSecond, _wMilliseconds);
            }
        }
    }
}