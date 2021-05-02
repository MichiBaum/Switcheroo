using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace ManagedWinapi.Windows
{
    /// <summary>
    ///     A device context of a window that allows you to draw onto that window.
    /// </summary>
    public class WindowDeviceContext : IDisposable {
        private readonly SystemWindow sw;

        internal WindowDeviceContext(SystemWindow sw, IntPtr hDC) {
            this.sw = sw;
            HDC = hDC;
        }

        /// <summary>
        ///     The device context handle.
        /// </summary>
        public IntPtr HDC { get; private set; }

        /// <summary>
        ///     Frees this device context.
        /// </summary>
        public void Dispose() {
            if (HDC != IntPtr.Zero) {
                ReleaseDC(sw.HWnd, HDC);
                HDC = IntPtr.Zero;
            }
        }

        /// <summary>
        ///     Creates a Graphics object for this device context.
        /// </summary>
        public Graphics CreateGraphics() {
            return Graphics.FromHdc(HDC);
        }

        [DllImport("user32.dll")]
        private static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);
    }
}