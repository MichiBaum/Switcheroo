namespace ManagedWinapi.Windows.Contents {
    internal abstract class WindowContentParser {
        internal abstract bool CanParseContent(SystemWindow sw);
        internal abstract WindowContent ParseContent(SystemWindow sw);

        internal static WindowContent Parse(SystemWindow sw) {
            WindowContentParser parser = ContentParserRegistry.Instance.GetParser(sw);
            if (parser == null)
                return null;
            return parser.ParseContent(sw);
        }
    }
}
