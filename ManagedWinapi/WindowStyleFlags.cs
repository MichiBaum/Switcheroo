using System;

namespace ManagedWinapi.Windows
{
    /// <summary>
    ///     Window Style Flags. The original constants started with WS_.
    /// </summary>
    /// <seealso cref="SystemWindow.Style" />
    [Flags]
    public enum WindowStyleFlags : long {
        /// <summary>
        ///     WS_OVERLAPPED
        /// </summary>
        OVERLAPPED = 0x00000000,

        /// <summary>
        ///     WS_POPUP
        /// </summary>
        POPUP = unchecked((int)0x80000000),

        /// <summary>
        ///     WS_CHILD
        /// </summary>
        CHILD = 0x40000000,

        /// <summary>
        ///     WS_MINIMIZE
        /// </summary>
        MINIMIZE = 0x20000000,

        /// <summary>
        ///     WS_VISIBLE
        /// </summary>
        VISIBLE = 0x10000000,

        /// <summary>
        ///     WS_DISABLED
        /// </summary>
        DISABLED = 0x08000000,

        /// <summary>
        ///     WS_CLIPSIBLINGS
        /// </summary>
        CLIPSIBLINGS = 0x04000000,

        /// <summary>
        ///     WS_CLIPCHILDREN
        /// </summary>
        CLIPCHILDREN = 0x02000000,

        /// <summary>
        ///     WS_MAXIMIZE
        /// </summary>
        MAXIMIZE = 0x01000000,

        /// <summary>
        ///     WS_BORDER
        /// </summary>
        BORDER = 0x00800000,

        /// <summary>
        ///     WS_DLGFRAME
        /// </summary>
        DLGFRAME = 0x00400000,

        /// <summary>
        ///     WS_VSCROLL
        /// </summary>
        VSCROLL = 0x00200000,

        /// <summary>
        ///     WS_HSCROLL
        /// </summary>
        HSCROLL = 0x00100000,

        /// <summary>
        ///     WS_SYSMENU
        /// </summary>
        SYSMENU = 0x00080000,

        /// <summary>
        ///     WS_THICKFRAME
        /// </summary>
        THICKFRAME = 0x00040000,

        /// <summary>
        ///     WS_GROUP
        /// </summary>
        GROUP = 0x00020000,

        /// <summary>
        ///     WS_TABSTOP
        /// </summary>
        TABSTOP = 0x00010000,

        /// <summary>
        ///     WS_MINIMIZEBOX
        /// </summary>
        MINIMIZEBOX = 0x00020000,

        /// <summary>
        ///     WS_MAXIMIZEBOX
        /// </summary>
        MAXIMIZEBOX = 0x00010000,

        /// <summary>
        ///     WS_CAPTION
        /// </summary>
        CAPTION = BORDER | DLGFRAME,

        /// <summary>
        ///     WS_TILED
        /// </summary>
        TILED = OVERLAPPED,

        /// <summary>
        ///     WS_ICONIC
        /// </summary>
        ICONIC = MINIMIZE,

        /// <summary>
        ///     WS_SIZEBOX
        /// </summary>
        SIZEBOX = THICKFRAME,

        /// <summary>
        ///     WS_TILEDWINDOW
        /// </summary>
        TILEDWINDOW = OVERLAPPEDWINDOW,

        /// <summary>
        ///     WS_OVERLAPPEDWINDOW
        /// </summary>
        OVERLAPPEDWINDOW = OVERLAPPED | CAPTION | SYSMENU | THICKFRAME | MINIMIZEBOX | MAXIMIZEBOX,

        /// <summary>
        ///     WS_POPUPWINDOW
        /// </summary>
        POPUPWINDOW = POPUP | BORDER | SYSMENU,

        /// <summary>
        ///     WS_CHILDWINDOW
        /// </summary>
        CHILDWINDOW = CHILD,

        /// <summary>
        ///     Usually WindowExStyleFlags.TOOLWINDOW should be used, but it seems like the style
        ///     is sometimes placed in the Style instead of ExtentedStyle
        /// </summary>
        TOOLWINDOW = 0x00000080
    }
}