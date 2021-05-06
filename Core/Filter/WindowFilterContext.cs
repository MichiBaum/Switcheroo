// Copyright by Switcheroo

#region

using System.Collections.Generic;

#endregion

namespace Switcheroo.Core.Filter {
    public class WindowFilterContext<T> where T : IWindowText {
        public string? ForegroundWindowProcessTitle { get; set; }
        public IEnumerable<T>? Windows { get; set; }
    }
}