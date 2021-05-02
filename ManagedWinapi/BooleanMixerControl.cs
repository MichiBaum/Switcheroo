using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace ManagedWinapi.Audio.Mixer {
    /// <summary>
    ///     A control that is has only two states (ie, values),
    ///     and is therefore adjusted via a button.
    /// </summary>
    public class BooleanMixerControl : MixerControl {
        internal BooleanMixerControl(Mixer mx, MixerLine ml, MIXERCONTROL mc) : base(mx, ml, mc) { }

        /// <summary>
        ///     Used to get or set the values of this switch.
        /// </summary>
        public bool[] Values {
            get {
                bool[] result = new bool[RawValueMultiplicity];
                MIXERCONTROLDETAILS mcd = new();
                MIXERCONTROLDETAILS_BOOLEAN mcdb = new();
                mcd.cbStruct = Marshal.SizeOf(mcd);
                mcd.dwControlID = ctrl.dwControlID;
                mcd.cChannels = ChannelCount;
                mcd.cMultipleItems = ctrl.cMultipleItems;
                mcd.paDetails = Marshal.AllocCoTaskMem(Marshal.SizeOf(mcdb) * result.Length);
                mcd.cbDetails = Marshal.SizeOf(mcdb);
                int err;
                if ((err = mixerGetControlDetailsA(mx.Handle, ref mcd, 0)) != 0)
                    throw new Win32Exception("Error #" + err + " calling mixerGetControlDetails()");
                for (int i = 0; i < result.Length; i++) {
                    mcdb = (MIXERCONTROLDETAILS_BOOLEAN)Marshal.PtrToStructure(
                        new IntPtr(mcd.paDetails.ToInt64() + (Marshal.SizeOf(mcdb) * i)),
                        typeof(MIXERCONTROLDETAILS_BOOLEAN));
                    result[i] = mcdb.fValue != 0;
                }

                return result;
            }
            set {
                if (value.Length != RawValueMultiplicity)
                    throw new ArgumentException("Incorrect dimension");

                MIXERCONTROLDETAILS mcd = new();
                MIXERCONTROLDETAILS_BOOLEAN mcdb = new();
                mcd.cbStruct = Marshal.SizeOf(mcd);
                mcd.dwControlID = ctrl.dwControlID;
                mcd.cChannels = ChannelCount;
                mcd.cMultipleItems = ctrl.cMultipleItems;
                mcd.paDetails = Marshal.AllocCoTaskMem(Marshal.SizeOf(mcdb) * value.Length);
                mcd.cbDetails = Marshal.SizeOf(mcdb);
                for (int i = 0; i < value.Length; i++) {
                    mcdb.fValue = value[i] ? 1 : 0;
                    Marshal.StructureToPtr(mcdb, new IntPtr(mcd.paDetails.ToInt64() + (Marshal.SizeOf(mcdb) * i)),
                        false);
                }

                int err;
                if ((err = mixerSetControlDetails(mx.Handle, ref mcd, 0)) != 0)
                    throw new Win32Exception("Error #" + err + " calling mixerGetControlDetails()");
            }
        }
    }
}