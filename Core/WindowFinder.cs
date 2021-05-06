// Copyright by Switcheroo

#region

using System.Collections.Generic;
using System.Linq;

#endregion

namespace Switcheroo.Core {
    public class WindowFinder {
        public List<AppWindow> GetWindows() {
            return AppWindow.AllToplevelWindows
                .Where(a => a.IsAltTabWindow())
                .ToList();
        }
    }
}