namespace ManagedWinapi.Accessibility {
    /// <summary>
    ///     This enumeration lists all kinds of accessible objects that can
    ///     be directly assigned to a window.
    /// </summary>
    public enum AccessibleObjectID : uint {
        /// <summary>
        ///     The window itself.
        /// </summary>
        OBJID_WINDOW = 0x00000000,

        /// <summary>
        ///     The system menu.
        /// </summary>
        OBJID_SYSMENU = 0xFFFFFFFF,

        /// <summary>
        ///     The title bar.
        /// </summary>
        OBJID_TITLEBAR = 0xFFFFFFFE,

        /// <summary>
        ///     The menu.
        /// </summary>
        OBJID_MENU = 0xFFFFFFFD,

        /// <summary>
        ///     The client area.
        /// </summary>
        OBJID_CLIENT = 0xFFFFFFFC,

        /// <summary>
        ///     The vertical scroll bar.
        /// </summary>
        OBJID_VSCROLL = 0xFFFFFFFB,

        /// <summary>
        ///     The horizontal scroll bar.
        /// </summary>
        OBJID_HSCROLL = 0xFFFFFFFA,

        /// <summary>
        ///     The size grip (part in the lower right corner that
        ///     makes resizing the window easier).
        /// </summary>
        OBJID_SIZEGRIP = 0xFFFFFFF9,

        /// <summary>
        ///     The caret (text cursor).
        /// </summary>
        OBJID_CARET = 0xFFFFFFF8,

        /// <summary>
        ///     The mouse cursor. There is only one mouse
        ///     cursor and it is not assigned to any window.
        /// </summary>
        OBJID_CURSOR = 0xFFFFFFF7,

        /// <summary>
        ///     An alert window.
        /// </summary>
        OBJID_ALERT = 0xFFFFFFF6,

        /// <summary>
        ///     A sound this window is playing.
        /// </summary>
        OBJID_SOUND = 0xFFFFFFF5
    }
}