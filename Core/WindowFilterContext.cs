using System.Collections.Generic;

namespace Switcheroo.Core
{
    public class WindowFilterContext<T> where T : IWindowText {
        public string ForegroundWindowProcessTitle { get; set; }
        public IEnumerable<T> Windows { get; set; }
    }
}