// Copyright by Switcheroo

#region

using System;
using System.Text;

#endregion

namespace ManagedWinapi {
    /// <summary>
    ///     Utility class to escape literal strings so that they can be used
    ///     for the <see cref="System.Windows.Forms.SendKeys" /> class.
    /// </summary>
    public class SendKeysEscaper {
        private static SendKeysEscaper? _instance;

        private readonly EscapableState[] lookupTable = new EscapableState[256];

        private SendKeysEscaper() {
            for (int i = 0; i < lookupTable.Length; i++) lookupTable[i] = EscapableState.ALWAYS;
            foreach (char c in "%()+^`{}~´") lookupTable[c] = EscapableState.BRACED_ONLY;
            lookupTable[180] = EscapableState.BRACED_ONLY;
            for (int i = 9; i <= 13; i++) lookupTable[i] = EscapableState.UNBRACED_ONLY;
            lookupTable[32] = EscapableState.UNBRACED_ONLY;
            lookupTable[133] = EscapableState.UNBRACED_ONLY;
            lookupTable[160] = EscapableState.UNBRACED_ONLY;
            for (int i = 0; i < 9; i++) lookupTable[i] = EscapableState.NOT_AT_ALL;
            for (int i = 14; i < 30; i++) lookupTable[i] = EscapableState.NOT_AT_ALL;
            lookupTable[127] = EscapableState.NOT_AT_ALL;
        }

        /// <summary>
        ///     The singleton instance.
        /// </summary>
        public SendKeysEscaper Instance => _instance ??= new SendKeysEscaper();

        private EscapableState getEscapableState(char c) {
            return c < 256 ? lookupTable[c] : EscapableState.ALWAYS;
        }

        /// <summary>
        ///     Escapes a literal string.
        /// </summary>
        /// <param name="literal">The literal string to be sent.</param>
        /// <param name="preferBraced">
        ///     Whether you prefer to put characters into braces.
        /// </param>
        /// <returns>The escaped string.</returns>
        public string Escape(string literal, bool preferBraced) {
            StringBuilder sb = new(literal.Length);
            foreach (char c in literal)
                switch (getEscapableState(c)) {
                    case EscapableState.NOT_AT_ALL:
                        // ignore
                        break;
                    case EscapableState.BRACED_ONLY:
                        sb.Append("{").Append(c).Append("}");
                        break;
                    case EscapableState.UNBRACED_ONLY:
                        sb.Append(c);
                        break;
                    case EscapableState.ALWAYS:
                        if (preferBraced)
                            sb.Append("{").Append(c).Append("}");
                        else
                            sb.Append(c);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

            return sb.ToString();
        }
    }
}