// Copyright by Switcheroo

#region

using System.Runtime.InteropServices;

#endregion

namespace ManagedWinapi.Audio.Mixer {
    /// <summary>
    ///     Represents a source line. Source lines represent way sound for one
    ///     destination enters the mixer. So, if you can both record and playback
    ///     CD audio, there will be two CD audio source lines, one for the Recording
    ///     destination line and one for the Playback destination line.
    /// </summary>
    public class SourceLine : MixerLine {
        private SourceLine(Mixer m, MIXERLINE l) : base(m, l) { }

        internal static SourceLine GetLine(Mixer mixer, int destIndex, int srcIndex) {
            MIXERLINE m = new();
            m.cbStruct = Marshal.SizeOf(m);
            m.dwDestination = destIndex;
            m.dwSource = srcIndex;
            mixerGetLineInfoA(mixer.Handle, ref m, MIXER_GETLINEINFOF_SOURCE);
            return new SourceLine(mixer, m);
        }
    }
}