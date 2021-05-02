namespace ManagedWinapi.Windows.Contents {
    internal class TextFieldParser : WindowContentParser {
        private readonly bool strict;

        public TextFieldParser(bool strict) {
            this.strict = strict;
        }

        internal override bool CanParseContent(SystemWindow sw) {
            if (strict) {
                const uint EM_GETLINECOUNT = 0xBA;
                return sw.SendGetMessage(EM_GETLINECOUNT) != 0;
            }

            return sw.Title != "";
        }

        internal override WindowContent ParseContent(SystemWindow sw) {
            return new TextContent(sw.Title, sw.PasswordCharacter != 0, strict);
        }
    }
}