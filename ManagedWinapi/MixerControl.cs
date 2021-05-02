using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace ManagedWinapi.Audio.Mixer {
    /// <summary>
    ///     A control of a mixer line. This can be for example a volume slider
    ///     or a mute switch.
    /// </summary>
    public class MixerControl {
        /// <summary>
        ///     Occurs when the value of this control is changed
        /// </summary>
        public EventHandler Changed;

        internal MIXERCONTROL ctrl;
        internal MixerLine ml;
        internal Mixer mx;

        internal MixerControl(Mixer mx, MixerLine ml, MIXERCONTROL ctrl) {
            this.mx = mx;
            this.ml = ml;
            this.ctrl = ctrl;
        }

        /// <summary>
        ///     The ID of this control.
        /// </summary>
        public int Id => ctrl.dwControlID;

        /// <summary>
        ///     The short name of this control.
        /// </summary>
        public string ShortName => ctrl.szShortName;

        /// <summary>
        ///     The long name of this control.
        /// </summary>
        public string LongName => ctrl.szName;

        /// <summary>
        ///     The class of this control. For example FADER or SWITCH.
        /// </summary>
        public MixerControlClass Class => (MixerControlClass)(ctrl.dwControlType & MIXERCONTROL_CT_CLASS_MASK);

        /// <summary>
        ///     The type of the control. For example mute switch.
        /// </summary>
        public MixerControlType ControlType => (MixerControlType)ctrl.dwControlType;

        /// <summary>
        ///     The flags of this control.
        /// </summary>
        public MixerControlFlags Flags => (MixerControlFlags)ctrl.fdwControl;

        /// <summary>
        ///     Whether this control is uniform. A uniform control controls
        ///     more than one channel, but can only set one value for all
        ///     channels.
        /// </summary>
        public bool IsUniform => (Flags & MixerControlFlags.UNIFORM) != 0;

        /// <summary>
        ///     Whether this control has multiple values per channel. An
        ///     example for a multiple value control is a three-band equalizer.
        /// </summary>
        public bool IsMultiple => (Flags & MixerControlFlags.MULTIPLE) != 0;

        /// <summary>
        ///     The number of channels.
        /// </summary>
        public int ChannelCount => ml.ChannelCount;

        /// <summary>
        ///     The number of multiple values. For a three band equalizer,
        ///     this is 3. Will be always one if IsMultiple is false.
        /// </summary>
        public int MultipleValuesCount => IsMultiple ? ctrl.cMultipleItems : 1;

        /// <summary>
        ///     The number of raw values that have to be get or set. This
        ///     value is provided as a convenience; it can be computed from
        ///     MultipleValuesCount, IsUniform and ChannelCount.
        /// </summary>
        public int RawValueMultiplicity {
            get {
                int val = MultipleValuesCount;
                if (!IsUniform) val *= ChannelCount;
                return val;
            }
        }

        /// <summary>
        ///     The line this control belongs to.
        /// </summary>
        public MixerLine Line => ml;

        /// <summary>
        ///     The mixer this control belongs to.
        /// </summary>
        public Mixer Mixer => mx;

        internal static MixerControl[] GetControls(Mixer mx, MixerLine line, int controlCount) {
            if (controlCount == 0)
                return new MixerControl[0];
            MIXERCONTROL[] mc = new MIXERCONTROL[controlCount];
            int mxsize = Marshal.SizeOf(mc[0]);
            if (mxsize != 148)
                throw new Exception("" + mxsize);
            //mxsize = 148;

            MIXERLINECONTROLS mlc = new();
            mlc.cbStruct = Marshal.SizeOf(mlc);
            mlc.cControls = controlCount;
            mlc.dwLineID = line.Id;

            mlc.pamxctrl = Marshal.AllocCoTaskMem(mxsize * controlCount);
            mlc.cbmxctrl = mxsize;

            int err;
            if ((err = mixerGetLineControlsA(mx.Handle, ref mlc, MIXER_GETLINECONTROLSF_ALL)) != 0)
                throw new Win32Exception("Error #" + err + " calling mixerGetLineControls()\n");
            for (int i = 0; i < controlCount; i++)
                mc[i] = (MIXERCONTROL)Marshal.PtrToStructure(new IntPtr(mlc.pamxctrl.ToInt64() + (mxsize * i)),
                    typeof(MIXERCONTROL));
            Marshal.FreeCoTaskMem(mlc.pamxctrl);
            MixerControl[] result = new MixerControl[controlCount];
            for (int i = 0; i < controlCount; i++) result[i] = GetControl(mx, line, mc[i]);
            return result;
        }

        private static MixerControl GetControl(Mixer mx, MixerLine ml, MIXERCONTROL mc) {
            MixerControl result = new(mx, ml, mc);
            if (result.Class == MixerControlClass.FADER && ((uint)result.ControlType & MIXERCONTROL_CT_UNITS_MASK) ==
                (uint)MixerControlType.MIXERCONTROL_CT_UNITS_UNSIGNED)
                return new FaderMixerControl(mx, ml, mc);
            if (result.Class == MixerControlClass.SWITCH &&
                ((uint)result.ControlType & MIXERCONTROL_CT_SUBCLASS_MASK) ==
                (uint)MixerControlType.MIXERCONTROL_CT_SC_SWITCH_BOOLEAN &&
                ((uint)result.ControlType & MIXERCONTROL_CT_UNITS_MASK) ==
                (uint)MixerControlType.MIXERCONTROL_CT_UNITS_BOOLEAN) return new BooleanMixerControl(mx, ml, mc);
            return result;
        }

        internal void OnChanged() {
            Changed?.Invoke(this, EventArgs.Empty);
        }

        #region PInvoke Declarations

        [DllImport("winmm.dll", CharSet = CharSet.Ansi)]
        private static extern int mixerGetLineControlsA(IntPtr hmxobj, ref
            MIXERLINECONTROLS pmxlc, int fdwControls);

        private struct MIXERLINECONTROLS {
            public int cbStruct;
            public int dwLineID;

            public int dwControl;
            public int cControls;
            public int cbmxctrl;
            public IntPtr pamxctrl;
        }

#pragma warning disable 649
        internal struct MIXERCONTROL {
            public int cbStruct;
            public int dwControlID;
            public uint dwControlType;
            public int fdwControl;
            public int cMultipleItems;

            [MarshalAs(UnmanagedType.ByValTStr,
                SizeConst = 16)]
            public string szShortName;

            [MarshalAs(UnmanagedType.ByValTStr,
                SizeConst = 64)]
            public string szName;

            public int lMinimum;
            public int lMaximum;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10, ArraySubType = UnmanagedType.I4)]
            public int[] reserved;
        }
#pragma warning restore 649

        internal struct MIXERCONTROLDETAILS {
            public int cbStruct;
            public int dwControlID;
            public int cChannels;
            public int cMultipleItems;
            public int cbDetails;
            public IntPtr paDetails;
        }

        internal struct MIXERCONTROLDETAILS_UNSIGNED {
            public int dwValue;
        }

        internal struct MIXERCONTROLDETAILS_BOOLEAN {
            public int fValue;
        }

        private static readonly int MIXER_GETLINECONTROLSF_ALL = 0x0;
        private static readonly uint MIXERCONTROL_CT_CLASS_MASK = 0xF0000000;

        private static readonly uint MIXERCONTROL_CT_SUBCLASS_MASK = 0x0F000000;
        private static readonly uint MIXERCONTROL_CT_UNITS_MASK = 0x00FF0000;

        [DllImport("winmm.dll", CharSet = CharSet.Ansi)]
        internal static extern int mixerGetControlDetailsA(IntPtr hmxobj, ref
            MIXERCONTROLDETAILS pmxcd, int fdwDetails);

        [DllImport("winmm.dll", CharSet = CharSet.Ansi)]
        internal static extern int mixerSetControlDetails(IntPtr hmxobj, ref
            MIXERCONTROLDETAILS pmxcd, int fdwDetails);

        #endregion
    }
}