using Switcheroo.Core.Matchers;
using System.Collections.Generic;

namespace Switcheroo.Core.Filter {
    public class FilterResult<T> where T : IWindowText {
        public T? AppWindow { get; set; }
        public IList<MatchResult>? WindowTitleMatchResults { get; set; }
        public IList<MatchResult>? ProcessTitleMatchResults { get; set; }
    }
}