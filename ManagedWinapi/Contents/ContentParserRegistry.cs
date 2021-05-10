using ManagedWinapi.Windows;
using ManagedWinapi.Windows.Contents;
using System.Collections.Generic;

namespace ManagedWinapi.Contents {
    internal class ContentParserRegistry {
        private static ContentParserRegistry? instance;
        private readonly List<WindowContentParser> parsers = new();

        private ContentParserRegistry() {
            parsers.Add(new ComboBoxParser());
            parsers.Add(new ListBoxParser());
            parsers.Add(new TextFieldParser(true));
            parsers.Add(new ListViewParser());
            parsers.Add(new TreeViewParser());
            parsers.Add(new AccessibleWindowParser());
            parsers.Add(new TextFieldParser(false));
        }

        public static ContentParserRegistry Instance {
            get {
                if (instance == null)
                    instance = new ContentParserRegistry();
                return instance;
            }
        }

        public WindowContentParser? GetParser(SystemWindow? sw) {
            foreach (WindowContentParser p in parsers)
                if (p.CanParseContent(sw))
                    return p;
            return null;
        }
    }
}