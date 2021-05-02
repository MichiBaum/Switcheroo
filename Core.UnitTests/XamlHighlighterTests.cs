using NUnit.Framework;
using Switcheroo.Core.Matchers;

namespace Switcheroo.Core.UnitTests {
    [TestFixture]
    public class XamlHighlighterTests {
        [Test]
        public void DoesItWork() {
            StringPart input = new StringPart("test > test-1", true);
            string output = new XamlHighlighter().Highlight(new[] {input, new StringPart("test")});
            Assert.That(output, Is.EqualTo("<Bold>test &gt; test-1</Bold>test"));
        }
    }
}