using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace ManagedWinapi.Audio.Mixer
{
    /// <summary>
    ///     A mixer control that is adjusted by a vertical fader, with a linear scale
    ///     of positive values (ie, 0 is the lowest possible value).
    /// </summary>
    public class FaderMixerControl : MixerControl {
        internal FaderMixerControl(Mixer mx, MixerLine ml, MIXERCONTROL mc) : base(mx, ml, mc) { }

        /// <summary>
        ///     The minimum value of this fader.
        /// </summary>
        public int Minimum => ctrl.lMinimum;

        /// <summary>
        ///     The maximum value of this fader.
        /// </summary>
        public int Maximum => ctrl.lMaximum;

        /// <summary>
        ///     Used to get or set the values of this fader.
        /// </summary>
        public int[] Values {
            get {
                int[] result = new int[RawValueMultiplicity];
                MIXERCONTROLDETAILS mcd = new();
                MIXERCONTROLDETAILS_UNSIGNED mcdu = new();
                mcd.cbStruct = Marshal.SizeOf(mcd);
                mcd.dwControlID = ctrl.dwControlID;
                mcd.cChannels = ChannelCount;
                mcd.cMultipleItems = ctrl.cMultipleItems;
                mcd.paDetails = Marshal.AllocCoTaskMem(Marshal.SizeOf(mcdu) * result.Length);
                mcd.cbDetails = Marshal.SizeOf(mcdu);
                int err;
                if ((err = mixerGetControlDetailsA(mx.Handle, ref mcd, 0)) != 0)
                    throw new Win32Exception("Error #" + err + " calling mixerGetControlDetails()");
                for (int i = 0; i < result.Length; i++) {
                    mcdu = (MIXERCONTROLDETAILS_UNSIGNED)Marshal.PtrToStructure(
                        new IntPtr(mcd.paDetails.ToInt64() + (Marshal.SizeOf(mcdu) * i)),
                        typeof(MIXERCONTROLDETAILS_UNSIGNED));
                    result[i] = mcdu.dwValue;
                }

                return result;
            }
            set {
                if (value.Length != RawValueMultiplicity)
                    throw new ArgumentException("Incorrect dimension");

                MIXERCONTROLDETAILS mcd = new();
                MIXERCONTROLDETAILS_UNSIGNED mcdu = new();
                mcd.cbStruct = Marshal.SizeOf(mcd);
                mcd.dwControlID = ctrl.dwControlID;
                mcd.cChannels = ChannelCount;
                mcd.cMultipleItems = ctrl.cMultipleItems;
                mcd.paDetails = Marshal.AllocCoTaskMem(Marshal.SizeOf(mcdu) * value.Length);
                mcd.cbDetails = Marshal.SizeOf(mcdu);
                for (int i = 0; i < value.Length; i++) {
                    mcdu.dwValue = value[i];
                    Marshal.StructureToPtr(mcdu, new IntPtr(mcd.paDetails.ToInt64() + (Marshal.SizeOf(mcdu) * i)),
                        false);
                }

                int err;
                if ((err = mixerSetControlDetails(mx.Handle, ref mcd, 0)) != 0)
                    throw new Win32Exception("Error #" + err + " calling mixerGetControlDetails()");
            }
        }
    }
}