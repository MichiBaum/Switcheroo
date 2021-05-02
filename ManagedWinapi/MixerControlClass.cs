namespace ManagedWinapi.Audio.Mixer {
    /// <summary>
    ///     Mixer control type classes. These classes are roughly based upon what type of
    ///     value a control adjusts, and therefore what kind of graphical user interface
    ///     you would normally present to the enduser to let him adjust that control's value.
    ///     The descriptions for these classes have been taken from
    ///     http://www.borg.com/~jglatt/tech/mixer.htm.
    /// </summary>
    public enum MixerControlClass {
        /// <summary>
        ///     A custom class of control. If none of the others are applicable.
        /// </summary>
        CUSTOM = 0x00000000,

        /// <summary>
        ///     A control that is adjusted by a graphical meter.
        /// </summary>
        METER = 0x10000000,

        /// <summary>
        ///     A control that is has only two states (ie, values), and is
        ///     therefore adjusted via a button.
        /// </summary>
        SWITCH = 0x20000000,

        /// <summary>
        ///     A control that is adjusted by numeric entry.
        /// </summary>
        NUMBER = 0x30000000,

        /// <summary>
        ///     A control that is adjusted by a horizontal slider
        ///     with a linear scale of negative and positive values.
        ///     (ie, Generally, 0 is the mid or "neutral" point).
        /// </summary>
        SLIDER = 0x40000000,

        /// <summary>
        ///     A control that is adjusted by a vertical fader, with
        ///     a linear scale of positive values (ie, 0 is the lowest
        ///     possible value).
        /// </summary>
        FADER = 0x50000000,

        /// <summary>
        ///     A control that allows the user to enter a time value, such
        ///     as Reverb Decay Time.
        /// </summary>
        TIME = 0x60000000,

        /// <summary>
        ///     A control that is adjusted by a listbox containing numerous
        ///     "values" to be selected. The user will single-select, or perhaps
        ///     multiple-select if desired, his choice of value(s).
        /// </summary>
        LIST = 0x70000000
    }
}