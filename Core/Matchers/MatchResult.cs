// Copyright by Switcheroo

#region

using System.Collections.Generic;

#endregion

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