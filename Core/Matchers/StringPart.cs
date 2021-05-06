namespace Switcheroo.Core.Matchers {
    public class StringPart {
        public StringPart(string value, bool isMatch = false) {
            Value = value;
            IsMatch = isMatch;
        }

        public string Value { get; set; }
        public bool IsMatch { get; set; }
    }
}