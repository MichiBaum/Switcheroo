// Copyright by Switcheroo

namespace Switcheroo.Core.Matchers {
    public interface IMatcher {
        public MatchResult Evaluate(string? input, string? pattern);
    }
}