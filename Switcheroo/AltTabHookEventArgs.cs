// Copyright by Switcheroo

#region

using System;

#endregion

namespace Switcheroo {
    public class AltTabHookEventArgs : EventArgs {
        public bool CtrlDown { get; set; }
        public bool ShiftDown { get; set; }
        public bool Handled { get; set; }
    }
}