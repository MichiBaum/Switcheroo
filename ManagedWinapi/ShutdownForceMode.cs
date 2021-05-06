// Copyright by Switcheroo

namespace ManagedWinapi {
    /// <summary>
    ///     Whether shutdown should be forced if an application cancels it
    ///     or is hung.
    /// </summary>
    public enum ShutdownForceMode : uint {
        /// <summary>
        ///     Do not force shutdown, applications can cancel it.
        /// </summary>
        NoForce = 0x00,

        /// <summary>
        ///     Force shutdown, even if application cancels it or is hung.
        /// </summary>
        Force = 0x04,

        /// <summary>
        ///     Force shutdown if application is hung, but not if it cancels it.
        /// </summary>
        ForceIfHung = 0x10
    }
}