// Copyright by Switcheroo

#region

using System;

#endregion

namespace ManagedWinapi {
    /// <summary>
    ///     The exception is thrown when a hotkey should be registered that
    ///     has already been registered by another application.
    /// </summary>
    public class HotkeyAlreadyInUseException : Exception {
    }
}