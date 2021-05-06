// unset

namespace ManagedWinapi {
    /// <summary>
    ///     Actions that can be performed at shutdown.
    /// </summary>
    public enum ShutdownAction : uint {
        /// <summary>
        ///     Log off the currently logged-on user.
        /// </summary>
        LogOff = 0x00,

        /// <summary>
        ///     Shut down the system.
        /// </summary>
        ShutDown = 0x01,

        /// <summary>
        ///     Reboot the system.
        /// </summary>
        Reboot = 0x02,

        /// <summary>
        ///     Shut down the system and power it off.
        /// </summary>
        PowerOff = 0x08,

        /// <summary>
        ///     Reboot the system and restart applications that are running
        ///     now and support this feature.
        /// </summary>
        RestartApps = 0x40
    }
}