using Switcheroo.Core.Matchers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Switcheroo.Core {
    public class XamlHighlighter {
        public string Highlight(IEnumerable<StringPart> stringParts) {
            XDocument xDocument = new(new XElement("Root"));
            foreach (StringPart stringPart in stringParts)
                if (stringPart.IsMatch)
                    xDocument.Root?.Add(new XElement("Bold", stringPart.Value));
                else
                    xDocument.Root?.Add(new XText(stringPart.Value));
            return string.Concat(xDocument.Root?.Nodes().Select(x => x.ToString()).ToArray() ?? Array.Empty<string>());
        }
    }
}