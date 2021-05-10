namespace ManagedWinapi.Windows.Contents {
    internal class TextFieldParser : WindowContentParser {
        private readonly bool strict;

        public TextFieldParser(bool strict) {
            this.strict = strict;
        }

        internal override bool CanParseContent(SystemWindow sw) {
            if (!strict) return sw.Title != "";
            const uint EM_GETLINECOUNT = 0xBA;
            return sw.SendGetMessage(EM_GETLINECOUNT) != 0;

        }

        internal override WindowContent ParseContent(SystemWindow? sw) {
            return new TextContent(sw?.Title, sw is not null && sw.PasswordCharacter != 0, strict);
        }
    }
}