﻿using System.Text.RegularExpressions;

namespace Switcheroo.Core.Matchers {
    public class IndividualCharactersMatcher : IMatcher {
        public MatchResult Evaluate(string input, string pattern) {
            if (input == null || pattern == null) return NonMatchResult(input);

            string regexPattern = BuildRegexPattern(pattern);

            Match match = Regex.Match(input, regexPattern, RegexOptions.IgnoreCase);

            if (!match.Success) return NonMatchResult(input);

            MatchResult matchResult = new();
            for (int groupIndex = 1; groupIndex < match.Groups.Count; groupIndex++) {
                Group group = match.Groups[groupIndex];
                if (group.Value.Length > 0)
                    matchResult.StringParts.Add(new StringPart(group.Value, groupIndex % 2 == 0));
            }

            matchResult.Matched = true;
            matchResult.Score = 1;

            return matchResult;
        }

        private static string BuildRegexPattern(string pattern) {
            string regexPattern = "";
            char? previousChar = null;
            foreach (char p in pattern) {
                if (previousChar != null)
                    regexPattern += string.Format("([^{0}]*?)({1})", Regex.Escape(previousChar + ""),
                        Regex.Escape(p + ""));
                else
                    regexPattern += string.Format("(.*?)({0})", Regex.Escape(p + ""));
                previousChar = p;
            }

            return regexPattern + "(.*)";
        }

        private static MatchResult NonMatchResult(string input) {
            MatchResult matchResult = new();
            if (input != null) matchResult.StringParts.Add(new StringPart(input));
            return matchResult;
        }
    }
}