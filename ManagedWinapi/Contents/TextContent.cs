using System.Collections.Generic;

namespace ManagedWinapi.Windows.Contents
{
    /// <summary>
    ///     The content of a text box.
    /// </summary>
    public class TextContent : WindowContent {
        private readonly bool password;
        private readonly bool strict;
        private readonly string text;

        internal TextContent(string text, bool password, bool strict) {
            this.text = text;
            this.password = password;
            this.strict = strict;
        }

        ///
        public string ComponentType => strict ? "TextBox" : "Text";

        ///
        public string ShortDescription {
            get {
                string s = strict ? " <TextBox>" : "";
                if (text.IndexOf("\n") != -1)
                    return "<MultiLine>" + s;
                if (password)
                    return text + " <Password>" + s;
                return text + s;
            }
        }

        ///
        public string LongDescription {
            get {
                if (password)
                    return text + " <Password>";
                return text;
            }
        }

        ///
        public Dictionary<string, string> PropertyList {
            get {
                Dictionary<string, string> result = new();
                result.Add("Password", password ? "True" : "False");
                result.Add("MultiLine", text.IndexOf('\n') != -1 ? "True" : "False");
                result.Add("Text", text);
                return result;
            }
        }
    }
}