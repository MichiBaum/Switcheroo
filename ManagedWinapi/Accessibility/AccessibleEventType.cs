// Copyright by Switcheroo

namespace ManagedWinapi.Accessibility {
    /// <summary>
    ///     This enumeration lists known accessible event types.
    /// </summary>
    public enum AccessibleEventType {
        /// <summary>
        ///     Sent when a sound is played.  Currently nothing is generating this, we
        ///     are going to be cleaning up the SOUNDSENTRY feature in the control panel
        ///     and will use this at that time.  Applications implementing WinEvents
        ///     are perfectly welcome to use it.  Clients of IAccessible* will simply
        ///     turn around and get back a non-visual object that describes the sound.
        /// </summary>
        EventSystemSound = 0x0001,

        /// <summary>
        ///     Sent when an alert needs to be given to the user.  MessageBoxes generate
        ///     alerts for example.
        /// </summary>
        EventSystemAlert = 0x0002,

        /// <summary>
        ///     Sent when the foreground (active) window changes, even if it is changing
        ///     to another window in the same thread as the previous one.
        /// </summary>
        EventSystemForeground = 0x0003,

        /// <summary>
        ///     Sent when entering into and leaving from menu mode (system, app bar, and
        ///     track popups).
        /// </summary>
        EventSystemMenuStart = 0x0004,

        /// <summary>
        ///     Sent when entering into and leaving from menu mode (system, app bar, and
        ///     track popups).
        /// </summary>
        EventSystemMenuEnd = 0x0005,

        /// <summary>
        ///     Sent when a menu popup comes up and just before it is taken down.  Note
        ///     that for a call to TrackPopupMenu(), a client will see EVENT_SYSTEM_MENUSTART
        ///     followed almost immediately by EVENT_SYSTEM_MENUPOPUPSTART for the popup
        ///     being shown.
        /// </summary>
        EventSystemMenuPopupStart = 0x0006,

        /// <summary>
        ///     Sent when a menu popup comes up and just before it is taken down.  Note
        ///     that for a call to TrackPopupMenu(), a client will see EVENT_SYSTEM_MENUSTART
        ///     followed almost immediately by EVENT_SYSTEM_MENUPOPUPSTART for the popup
        ///     being shown.
        /// </summary>
        EventSystemMenuPopupEnd = 0x0007,

        /// <summary>
        ///     Sent when a window takes the capture and releases the capture.
        /// </summary>
        EventSystemCaptureStart = 0x0008,

        /// <summary>
        ///     Sent when a window takes the capture and releases the capture.
        /// </summary>
        EventSystemCaptureEnd = 0x0009,

        /// <summary>
        ///     Sent when a window enters and leaves move-size dragging mode.
        /// </summary>
        EventSystemMoveSizeStart = 0x000A,

        /// <summary>
        ///     Sent when a window enters and leaves move-size dragging mode.
        /// </summary>
        EventSystemMoveSizeEnd = 0x000B,

        /// <summary>
        ///     Sent when a window enters and leaves context sensitive help mode.
        /// </summary>
        EventSystemContextHelpStart = 0x000C,

        /// <summary>
        ///     Sent when a window enters and leaves context sensitive help mode.
        /// </summary>
        EventSystemContextHelpEnd = 0x000D,

        /// <summary>
        ///     Sent when a window enters and leaves drag drop mode.  Note that it is up
        ///     to apps and OLE to generate this, since the system doesn't know.  Like
        ///     EVENT_SYSTEM_SOUND, it will be a while before this is prevalent.
        /// </summary>
        EventSystemDragDropStart = 0x000E,

        /// <summary>
        ///     Sent when a window enters and leaves drag drop mode.  Note that it is up
        ///     to apps and OLE to generate this, since the system doesn't know.  Like
        ///     EVENT_SYSTEM_SOUND, it will be a while before this is prevalent.
        /// </summary>
        EventSystemDragDropEnd = 0x000F,

        /// <summary>
        ///     Sent when a dialog comes up and just before it goes away.
        /// </summary>
        EventSystemDialogStart = 0x0010,

        /// <summary>
        ///     Sent when a dialog comes up and just before it goes away.
        /// </summary>
        EventSystemDialogEnd = 0x0011,

        /// <summary>
        ///     Sent when beginning and ending the tracking of a scrollbar in a window,
        ///     and also for scrollbar controls.
        /// </summary>
        EventSystemScrollingStart = 0x0012,

        /// <summary>
        ///     Sent when beginning and ending the tracking of a scrollbar in a window,
        ///     and also for scrollbar controls.
        /// </summary>
        EventSystemScrollingEnd = 0x0013,

        /// <summary>
        ///     Sent when beginning and ending alt-tab mode with the switch window.
        /// </summary>
        EventSystemSwitchStart = 0x0014,

        /// <summary>
        ///     Sent when beginning and ending alt-tab mode with the switch window.
        /// </summary>
        EventSystemSwitchEnd = 0x0015,

        /// <summary>
        ///     Sent when a window minimizes.
        /// </summary>
        EventSystemMinimizeStart = 0x0016,

        /// <summary>
        ///     Sent just before a window restores.
        /// </summary>
        EventSystemMinimizeEnd = 0x0017,

        /// <summary>
        ///     hwnd + ID + idChild is created item
        /// </summary>
        EventObjectCreate = 0x8000,

        /// <summary>
        ///     hwnd + ID + idChild is destroyed item
        /// </summary>
        EventObjectDestroy = 0x8001,

        /// <summary>
        ///     hwnd + ID + idChild is shown item
        /// </summary>
        EventObjectShow = 0x8002,

        /// <summary>
        ///     hwnd + ID + idChild is hidden item
        /// </summary>
        EventObjectHide = 0x8003,

        /// <summary>
        ///     hwnd + ID + idChild is parent of zordering children
        /// </summary>
        EventObjectReorder = 0x8004,

        /// <summary>
        ///     hwnd + ID + idChild is focused item
        /// </summary>
        EventObjectFocus = 0x8005,

        /// <summary>
        ///     hwnd + ID + idChild is selected item (if only one), or idChild is OBJID_WINDOW if complex
        /// </summary>
        EventObjectSelection = 0x8006,

        /// <summary>
        ///     hwnd + ID + idChild is item added
        /// </summary>
        EventObjectSelectionAdd = 0x8007,

        /// <summary>
        ///     hwnd + ID + idChild is item removed
        /// </summary>
        EventObjectSelectionRemove = 0x8008,

        /// <summary>
        ///     hwnd + ID + idChild is parent of changed selected items
        /// </summary>
        EventObjectSelectionWithin = 0x8009,

        /// <summary>
        ///     hwnd + ID + idChild is item w/ state change
        /// </summary>
        EventObjectStateChange = 0x800A,

        /// <summary>
        ///     hwnd + ID + idChild is moved/sized item
        /// </summary>
        EventObjectLocationChange = 0x800B,

        /// <summary>
        ///     hwnd + ID + idChild is item w/ name change
        /// </summary>
        EventObjectNameChange = 0x800C,

        /// <summary>
        ///     hwnd + ID + idChild is item w/ desc change
        /// </summary>
        EventObjectDescriptionChange = 0x800D,

        /// <summary>
        ///     hwnd + ID + idChild is item w/ value change
        /// </summary>
        EventObjectValueChange = 0x800E,

        /// <summary>
        ///     hwnd + ID + idChild is item w/ new parent
        /// </summary>
        EventObjectParentChange = 0x800F,

        /// <summary>
        ///     hwnd + ID + idChild is item w/ help change
        /// </summary>
        EventObjectHelpChange = 0x8010,

        /// <summary>
        ///     hwnd + ID + idChild is item w/ def action change
        /// </summary>
        EventObjectDefactionChange = 0x8011,

        /// <summary>
        ///     hwnd + ID + idChild is item w/ keybd accel change
        /// </summary>
        EventObjectAcceleratorChange = 0x8012,

        /// <summary>
        ///     The lowest possible event value
        /// </summary>
        EventMin = 0x00000001,

        /// <summary>
        ///     The highest possible event value
        /// </summary>
        EventMax = 0x7FFFFFFF
    }
}