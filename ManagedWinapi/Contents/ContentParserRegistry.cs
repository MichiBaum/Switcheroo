using System.Collections.Generic;

namespace ManagedWinapi.Windows.Contents
{
    internal class ContentParserRegistry
    {
        static ContentParserRegistry instance = null;

        public static ContentParserRegistry Instance
        {
            get
            {
                if (instance == null)
                    instance = new ContentParserRegistry();
                return instance;
            }
        }

        List<WindowContentParser> parsers = new List<WindowContentParser>();

        private ContentParserRegistry()
        {
            parsers.Add(new ComboBoxParser());
            parsers.Add(new ListBoxParser());
            parsers.Add(new TextFieldParser(true));
            parsers.Add(new ListViewParser());
            parsers.Add(new TreeViewParser());
            parsers.Add(new AccessibleWindowParser());
            parsers.Add(new TextFieldParser(false));
        }

        public WindowContentParser GetParser(SystemWindow sw)
        {
            foreach(WindowContentParser p in parsers) {
                if (p.CanParseContent(sw))
                    return p;
            }
            return null;
        }
    }
}
