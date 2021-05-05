﻿using System;

namespace Switcheroo.Core.Matchers {
    public class StartsWithMatcher : IMatcher {
        public MatchResult Evaluate(string? input, string? pattern) {
            MatchResult matchResult = new();

            if (input == null) return matchResult;

            if (pattern == null) {
                matchResult.StringParts.Add(new StringPart(input));
                return matchResult;
            }

            if (!InputStartsWithPattern(input, pattern)) {
                matchResult.StringParts.Add(new StringPart(input));
                return matchResult;
            }

            string matchedPart = input.Substring(0, pattern.Length);
            string restOfInput = input.Substring(pattern.Length);

            matchResult.Matched = true;
            matchResult.Score = 4;
            matchResult.StringParts.Add(new StringPart(matchedPart, true));
            matchResult.StringParts.Add(new StringPart(restOfInput));

            return matchResult;
        }

        private static bool InputStartsWithPattern(string input, string pattern) {
            return input.StartsWith(pattern, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}