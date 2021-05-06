// Copyright by Switcheroo

#region

using System.Drawing;
using System.Runtime.InteropServices;

#endregion

// from www.pinvoke.net
namespace ManagedWinapi.Windows {
    /// <summary>
    ///     Wrapper around the Winapi POINT type.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct POINT {
        /// <summary>
        ///     The X Coordinate.
        /// </summary>
        public int X;

        /// <summary>
        ///     The Y Coordinate.
        /// </summary>
        public int Y;

        /// <summary>
        ///     Creates a new POINT.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public POINT(int x, int y) {
            X = x;
            Y = y;
        }

        /// <summary>
        ///     Implicit cast.
        /// </summary>
        /// <returns></returns>
        public static implicit operator Point(POINT p) {
            return new(p.X, p.Y);
        }

        /// <summary>
        ///     Implicit cast.
        /// </summary>
        /// <returns></returns>
        public static implicit operator POINT(Point p) {
            return new(p.X, p.Y);
        }
    }
}