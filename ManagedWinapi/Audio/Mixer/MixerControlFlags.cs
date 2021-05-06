// Copyright by Switcheroo

#region

using System;

#endregion

namespace ManagedWinapi.Audio.Mixer {
    /// <summary>
    ///     Flags of a mixer control.
    /// </summary>
    [Flags]
    public enum MixerControlFlags {
        /// <summary>
        ///     This control has multiple channels, but only one value for
        ///     all of them.
        /// </summary>
        UNIFORM = 0x00000001,

        /// <summary>
        ///     This control has multiple values for one channel (like an equalizer).
        /// </summary>
        MULTIPLE = 0x00000002,

        /// <summary>
        ///     This control is disabled.
        /// </summary>
        DISABLED = unchecked((int)0x80000000)
    }
}