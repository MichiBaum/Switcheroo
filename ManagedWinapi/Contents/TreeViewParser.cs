using ManagedWinapi.Accessibility;
using System.Collections.Generic;

namespace ManagedWinapi.Windows.Contents
{
    internal class TreeViewParser : WindowContentParser {
        private readonly uint TVM_GETCOUNT = 0x1100 + 5;

        internal override bool CanParseContent(SystemWindow sw) {
            int cnt = sw.SendGetMessage(TVM_GETCOUNT, 0);
            return cnt != 0;
        }

        internal override WindowContent ParseContent(SystemWindow sw) {
            SystemAccessibleObject sao = SystemAccessibleObject.FromWindow(sw, AccessibleObjectID.OBJID_CLIENT);
            if (sao.RoleIndex == 35) {
                List<string> treeNodes = new();
                int selected = -1;
                foreach (SystemAccessibleObject n in sao.Children)
                    if (n.RoleIndex == 36) {
                        if ((n.State & 0x2) != 0) selected = treeNodes.Count;
                        treeNodes.Add(ListContent.Repeat('\t', int.Parse(n.Value)) + n.Name);
                    }

                if (treeNodes.Count > 0) return new ListContent("TreeView", selected, null, treeNodes.ToArray());
            }

            return new ListContent("EmptyTreeView", -1, null, new string[0]);
        }
    }
}