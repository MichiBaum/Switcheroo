// Copyright by Switcheroo

#region

using ManagedWinapi.Accessibility;
using System.Runtime.InteropServices;

#endregion

namespace ManagedWinapi.Windows.Contents {
    internal class AccessibleWindowParser : WindowContentParser {
        internal override bool CanParseContent(SystemWindow sw) {
            return TestMenu(sw, AccessibleObjectID.ObjidMenu) ||
                   TestMenu(sw, AccessibleObjectID.ObjidSysmenu) ||
                   TestClientArea(sw);
        }

        internal override WindowContent ParseContent(SystemWindow sw) {
            SystemAccessibleObject sao = SystemAccessibleObject.FromWindow(sw, AccessibleObjectID.ObjidWindow);
            bool sysmenu = TestMenu(sw, AccessibleObjectID.ObjidSysmenu);
            bool menu = TestMenu(sw, AccessibleObjectID.ObjidMenu);
            bool clientarea = TestClientArea(sw);
            return new AccessibleWindowContent(sao.Name, menu, sysmenu, clientarea, sw);
        }

        private bool TestClientArea(SystemWindow sw) {
            try {
                SystemAccessibleObject sao = SystemAccessibleObject.FromWindow(sw, AccessibleObjectID.ObjidClient);
                foreach (SystemAccessibleObject c in sao.Children)
                    if (c.Window == sw)
                        return true;
            } catch (COMException) { }

            return false;
        }

        private bool TestMenu(SystemWindow sw, AccessibleObjectID accessibleObjectID) {
            SystemAccessibleObject sao = SystemAccessibleObject.FromWindow(sw, accessibleObjectID);
            return sao.Children.Length > 0;
        }
    }
}