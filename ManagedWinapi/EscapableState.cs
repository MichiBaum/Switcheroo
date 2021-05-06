// Copyright by Switcheroo

namespace ManagedWinapi {
    /// <summary>
    ///     Specifies if a character needs to be escaped.
    /// </summary>
    internal enum EscapableState {
        /// <summary>
        ///     The character cannot be used at all with SendKeys.
        /// </summary>
        NOT_AT_ALL,

        /// <summary>
        ///     The character must be escaped by putting it into braces
        /// </summary>
        BRACED_ONLY,

        /// <summary>
        ///     The character may not be escaped by putting it into braces
        /// </summary>
        UNBRACED_ONLY,

        /// <summary>
        ///     Both ways are okay.
        /// </summary>
        ALWAYS
    }
}