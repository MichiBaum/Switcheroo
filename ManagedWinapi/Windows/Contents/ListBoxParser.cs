namespace ManagedWinapi.Windows.Contents {
    internal class ListBoxParser : WindowContentParser {
        internal override bool CanParseContent(SystemWindow sw) {
            return SystemListBox.FromSystemWindow(sw) != null;
        }

        internal override WindowContent ParseContent(SystemWindow? sw) {
            SystemListBox? slb = SystemListBox.FromSystemWindow(sw);
            int c = slb.Count;
            string[] values = new string[c];
            for (int i = 0; i < c; i++) values[i] = slb[i];
            return new ListContent("ListBox", slb.SelectedIndex, slb.SelectedItem, values);
        }
    }
}