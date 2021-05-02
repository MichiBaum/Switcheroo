namespace ManagedWinapi.Audio.Mixer {
    /// <summary>
    ///     Types of source or destination lines. The descriptions for these
    ///     lines have been taken from http://www.borg.com/~jglatt/tech/mixer.htm.
    /// </summary>
    public enum MixerLineComponentType {
        /// <summary>
        ///     An undefined destination line type.
        /// </summary>
        DST_UNDEFINED = 0,

        /// <summary>
        ///     A digital destination, for example, a SPDIF output jack.
        /// </summary>
        DST_DIGITAL = 1,

        /// <summary>
        ///     A line output destination. Typically used for a line output
        ///     jack, if there is a separate speaker output (ie,
        ///     MIXERLINE_COMPONENTTYPE_DST_SPEAKERS).
        /// </summary>
        DST_LINE = 2,

        /// <summary>
        ///     Typically a "Monitor Out" jack to be used for a speaker system
        ///     separate from the main speaker out. Or, it could be some built-in
        ///     monitor speaker on the sound card itself, such as a speaker for a
        ///     built-in modem.
        /// </summary>
        DST_MONITOR = 3,

        /// <summary>
        ///     The audio output to a pair of speakers (ie, the "Speaker Out" jack).
        /// </summary>
        DST_SPEAKERS = 4,

        /// <summary>
        ///     Typically, a headphone output jack.
        /// </summary>
        DST_HEADPHONES = 5,

        /// <summary>
        ///     Typically used to daisy-chain a telephone to an analog
        ///     modem's "telephone out" jack.
        /// </summary>
        DST_TELEPHONE = 6,

        /// <summary>
        ///     The card's ADC (to digitize analog sources, for example,
        ///     in recording WAVE files of such).
        /// </summary>
        DST_WAVEIN = 7,

        /// <summary>
        ///     May be some sort of hardware used for voice recognition.
        ///     Typically, a microphone source line would be attached to this.
        /// </summary>
        DST_VOICEIN = 8,

        /// <summary>
        ///     An undefined source line type.
        /// </summary>
        SRC_UNDEFINED = 0x1000,

        /// <summary>
        ///     A digital source, for example, a SPDIF input jack.
        /// </summary>
        SRC_DIGITAL = 0x1001,

        /// <summary>
        ///     A line input source. Typically used for a line input jack,
        ///     if there is a separate microphone input (ie,
        ///     MIXERLINE_COMPONENTTYPE_SRC_MICROPHONE).
        /// </summary>
        SRC_LINE = 0x1002,

        /// <summary>
        ///     Microphone input (but also used for a combination of
        ///     Mic/Line input if there isn't a separate line input source).
        /// </summary>
        SRC_MICROPHONE = 0x1003,

        /// <summary>
        ///     Musical synth. Typically used for a card that contains a
        ///     synth capable of playing MIDI. This would be the audio out
        ///     of that built-in synth.
        /// </summary>
        SRC_SYNTHESIZER = 0x1004,

        /// <summary>
        ///     The audio feed from an internal CDROM drive (connected to
        ///     the sound card).
        /// </summary>
        SRC_COMPACTDISC = 0x1005,

        /// <summary>
        ///     Typically used for a telephone line's incoming audio
        ///     to be piped through the computer's speakers, or the
        ///     telephone line in jack for a built-in modem.
        /// </summary>
        SRC_TELEPHONE = 0x1006,

        /// <summary>
        ///     Typically, to allow sound, that normally goes to the computer's
        ///     built-in speaker, to instead be routed through the card's speaker
        ///     output. The motherboard's system speaker connector would be internally
        ///     connected to some connector on the sound card for this purpose.
        /// </summary>
        SRC_PCSPEAKER = 0x1007,

        /// <summary>
        ///     Wave playback (ie, this is the card's DAC).
        /// </summary>
        SRC_WAVEOUT = 0x1008,

        /// <summary>
        ///     An aux jack meant to be routed to the Speaker Out, or to the
        ///     ADC (for WAVE recording). Typically, this is used to connect external,
        ///     analog equipment (such as tape decks, the audio outputs of musical
        ///     instruments, etc) for digitalizing or playback through the sound card.
        /// </summary>
        SRC_AUXILIARY = 0x1009,

        /// <summary>
        ///     May be used similiarly to MIXERLINE_COMPONENTTYPE_SRC_AUXILIARY (although
        ///     I have seen some mixers use this like MIXERLINE_COMPONENTTYPE_SRC_PCSPEAKER).
        ///     In general, this would be some analog connector on the sound card which is
        ///     only accessible internally, to be used to internally connect some analog component
        ///     inside of the computer case so that it plays through the speaker out.
        /// </summary>
        SRC_ANALOG = 0x100A
    }
}