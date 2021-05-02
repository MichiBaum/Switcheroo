namespace ManagedWinapi.Windows.Contents
{
    internal class ComboBoxParser : WindowContentParser {
        internal override bool CanParseContent(SystemWindow sw) {
            return SystemComboBox.FromSystemWindow(sw) != null;
        }

        internal override WindowContent ParseContent(SystemWindow sw) {
            SystemComboBox slb = SystemComboBox.FromSystemWindow(sw);
            int c = slb.Count;
            string[] values = new string[c];
            for (int i = 0; i < c; i++) values[i] = slb[i];
            return new ListContent("ComboBox", -1, sw.Title, values);
        }
    }
}