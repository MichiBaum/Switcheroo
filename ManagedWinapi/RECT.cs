using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace ManagedWinapi.Windows
{
    /// <summary>
    ///     Wrapper around the Winapi RECT type.
    /// </summary>
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct RECT {
        /// <summary>
        ///     LEFT
        /// </summary>
        public int Left;

        /// <summary>
        ///     TOP
        /// </summary>
        public int Top;

        /// <summary>
        ///     RIGHT
        /// </summary>
        public int Right;

        /// <summary>
        ///     BOTTOM
        /// </summary>
        public int Bottom;

        /// <summary>
        ///     Creates a new RECT.
        /// </summary>
        public RECT(int left_, int top_, int right_, int bottom_) {
            Left = left_;
            Top = top_;
            Right = right_;
            Bottom = bottom_;
        }

        /// <summary>
        ///     HEIGHT
        /// </summary>
        public int Height => Bottom - Top;

        /// <summary>
        ///     WIDTH
        /// </summary>
        public int Width => Right - Left;

        /// <summary>
        ///     SIZE
        /// </summary>
        public Size Size => new(Width, Height);

        /// <summary>
        ///     LOCATION
        /// </summary>
        public Point Location => new(Left, Top);

        // Handy method for converting to a System.Drawing.Rectangle
        /// <summary>
        ///     Convert RECT to a Rectangle.
        /// </summary>
        public Rectangle ToRectangle() { return Rectangle.FromLTRB(Left, Top, Right, Bottom); }

        /// <summary>
        ///     Convert Rectangle to a RECT
        /// </summary>
        public static RECT FromRectangle(Rectangle rectangle) {
            return new(rectangle.Left, rectangle.Top, rectangle.Right, rectangle.Bottom);
        }

        /// <summary>
        ///     Returns the hash code for this instance.
        /// </summary>
        public override int GetHashCode() {
            return Left ^ ((Top << 13) | (Top >> 0x13))
                        ^ ((Width << 0x1a) | (Width >> 6))
                        ^ ((Height << 7) | (Height >> 0x19));
        }

        #region Operator overloads

        /// <summary>
        ///     Implicit Cast.
        /// </summary>
        public static implicit operator Rectangle(RECT rect) {
            return Rectangle.FromLTRB(rect.Left, rect.Top, rect.Right, rect.Bottom);
        }

        /// <summary>
        ///     Implicit Cast.
        /// </summary>
        public static implicit operator RECT(Rectangle rect) {
            return new(rect.Left, rect.Top, rect.Right, rect.Bottom);
        }

        #endregion
    }
}