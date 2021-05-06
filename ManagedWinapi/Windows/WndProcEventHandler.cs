// Copyright by Switcheroo

#region

using System.Windows.Forms;

#endregion

namespace ManagedWinapi.Windows {
    /// <summary>
    ///     Called by an EventDispatchingNativeWindow when a window message is received
    /// </summary>
    /// <param name="m">The message to handle.</param>
    /// <param name="handled">
    ///     Whether the event has already been handled. If this value is true, the handler
    ///     should return immediately. It may set the value to true to indicate that no others
    ///     should handle it. If the event is not handled by any handler, it is passed to the
    ///     default WindowProc.
    /// </param>
    public delegate void WndProcEventHandler(ref Message m, ref bool handled);
}