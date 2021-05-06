namespace ManagedWinapi.Accessibility {
    /// <summary>
    ///     This enumeration lists all kinds of accessible objects that can
    ///     be directly assigned to a window.
    /// </summary>
    public enum AccessibleObjectID : uint {
        /// <summary>
        ///     The window itself.
        /// </summary>
        ObjidWindow = 0x00000000,

        /// <summary>
        ///     The system menu.
        /// </summary>
        ObjidSysmenu = 0xFFFFFFFF,

        /// <summary>
        ///     The title bar.
        /// </summary>
        ObjidTitlebar = 0xFFFFFFFE,

        /// <summary>
        ///     The menu.
        /// </summary>
        ObjidMenu = 0xFFFFFFFD,

        /// <summary>
        ///     The client area.
        /// </summary>
        ObjidClient = 0xFFFFFFFC,

        /// <summary>
        ///     The vertical scroll bar.
        /// </summary>
        ObjidVscroll = 0xFFFFFFFB,

        /// <summary>
        ///     The horizontal scroll bar.
        /// </summary>
        ObjidHscroll = 0xFFFFFFFA,

        /// <summary>
        ///     The size grip (part in the lower right corner that
        ///     makes resizing the window easier).
        /// </summary>
        ObjidSizegrip = 0xFFFFFFF9,

        /// <summary>
        ///     The caret (text cursor).
        /// </summary>
        ObjidCaret = 0xFFFFFFF8,

        /// <summary>
        ///     The mouse cursor. There is only one mouse
        ///     cursor and it is not assigned to any window.
        /// </summary>
        ObjidCursor = 0xFFFFFFF7,

        /// <summary>
        ///     An alert window.
        /// </summary>
        ObjidAlert = 0xFFFFFFF6,

        /// <summary>
        ///     A sound this window is playing.
        /// </summary>
        ObjidSound = 0xFFFFFFF5
    }
}