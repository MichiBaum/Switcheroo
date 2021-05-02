using System;

namespace ManagedWinapi.Audio.Mixer {
    /// <summary>
    ///     Provides data for the LineChanged and ControlChanged events of a
    ///     <see cref="Mixer">Mixer</see>.
    /// </summary>
    public class MixerEventArgs : EventArgs {
        /// <summary>
        ///     Initializes a new instance of the
        ///     <see cref="MixerEventArgs">MixerEventArgs</see> class.
        /// </summary>
        /// <param name="mixer">The affected mixer</param>
        /// <param name="line">The affected line</param>
        /// <param name="control">
        ///     The affected control, or <code>null</code>
        ///     if this is a LineChanged event.
        /// </param>
        public MixerEventArgs(Mixer mixer, MixerLine line, MixerControl control) {
            Mixer = mixer;
            Line = line;
            Control = control;
        }

        /// <summary>
        ///     The affected mixer.
        /// </summary>
        public Mixer Mixer { get; }

        /// <summary>
        ///     The affected line.
        /// </summary>
        public MixerLine Line { get; }

        /// <summary>
        ///     The affected control.
        /// </summary>
        public MixerControl Control { get; }
    }
}