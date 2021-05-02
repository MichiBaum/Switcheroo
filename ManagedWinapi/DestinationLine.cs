using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace ManagedWinapi.Audio.Mixer
{
    /// <summary>
    ///     Represents a destination line. There is one destination line for
    ///     each way sound can leave the mixer. Usually there are two destination lines,
    ///     one for playback and one for recording.
    /// </summary>
    public class DestinationLine : MixerLine {
        private IList<MixerLine> childLines;

        private IList<SourceLine> srcLines;
        private DestinationLine(Mixer mixer, MIXERLINE line) : base(mixer, line) { }

        /// <summary>
        ///     Gets the number of source lines of this destination line.
        /// </summary>
        public int SourceLineCount => line.cConnections;

        /// <summary>
        ///     Gets all source lines of this destination line.
        /// </summary>
        public IList<SourceLine> SourceLines {
            get {
                if (srcLines == null) {
                    List<SourceLine> sls = new(SourceLineCount);
                    for (int i = 0; i < SourceLineCount; i++) sls.Add(SourceLine.GetLine(mixer, line.dwDestination, i));
                    srcLines = sls.AsReadOnly();
                }

                return srcLines;
            }
        }

        internal override IList<MixerLine> ChildLines {
            get {
                if (childLines == null) {
                    List<MixerLine> cl = new();
                    foreach (MixerLine ml in SourceLines) cl.Add(ml);
                    childLines = cl.AsReadOnly();
                }

                return childLines;
            }
        }

        internal static DestinationLine GetLine(Mixer mixer, int index) {
            MIXERLINE m = new();
            m.cbStruct = Marshal.SizeOf(m);
            m.dwDestination = index;
            mixerGetLineInfoA(mixer.Handle, ref m, MIXER_GETLINEINFOF_DESTINATION);
            return new DestinationLine(mixer, m);
        }

        ///
        public override void Dispose() {
        }
    }
}