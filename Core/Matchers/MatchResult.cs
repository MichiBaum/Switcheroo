using System.Collections.Generic;

namespace Switcheroo.Core.Matchers {
    public class MatchResult {
        public MatchResult() {
            StringParts = new List<StringPart>();
        }

        public bool Matched { get; set; }
        public int Score { get; set; }
        public IList<StringPart> StringParts { get; set; }
    }
}