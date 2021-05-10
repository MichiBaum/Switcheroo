using ManagedWinapi.Accessibility;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace ManagedWinapi.Windows.Contents {
    /// <summary>
    ///     The content of an object that supports the Accessibility API
    ///     (used by screen readers and similar programs).
    /// </summary>
    public class AccessibleWindowContent : WindowContent {
        private readonly bool hasMenu, hasSysMenu, hasClientArea;
        private readonly string name;
        private readonly SystemWindow systemWindow;
        private string menu, sysmenu, clientarea;

        private bool parsed;

        internal AccessibleWindowContent(string name, bool hasMenu, bool hasSysMenu, bool hasClientArea,
            SystemWindow systemWindow) {
            this.name = name;
            this.hasMenu = hasMenu;
            this.hasSysMenu = hasSysMenu;
            this.hasClientArea = hasClientArea;
            this.systemWindow = systemWindow;
        }

        public string ComponentType => "AccessibleWindow";

        public string ShortDescription =>
            name + " <AccessibleWindow:" +
            (hasSysMenu ? " SystemMenu" : "") +
            (hasMenu ? " Menu" : "") +
            (hasClientArea ? " ClientArea" : "") + ">";

        public string? LongDescription {
            get {
                ParseIfNeeded();
                string result = ShortDescription + "\n";
                if (sysmenu != null)
                    result += "System menu:\n" + sysmenu + "\n";
                if (menu != null)
                    result += "Menu:\n" + menu + "\n";
                if (clientarea != null)
                    result += "Client area:\n" + clientarea + "\n";
                return result;
            }
        }

        ///
        public Dictionary<string, string?> PropertyList {
            get {
                Dictionary<string, string> result = new();
                return result;
            }
        }

        private void ParseIfNeeded() {
            if (parsed)
                return;
            if (hasSysMenu)
                sysmenu = ParseMenu(systemWindow, AccessibleObjectID.OBJID_SYSMENU);
            if (hasMenu)
                menu = ParseMenu(systemWindow, AccessibleObjectID.OBJID_MENU);
            if (hasClientArea)
                clientarea = ParseClientArea(systemWindow);
            parsed = true;
        }

        private string ParseMenu(SystemWindow sw, AccessibleObjectID accessibleObjectID) {
            SystemAccessibleObject sao = SystemAccessibleObject.FromWindow(sw, accessibleObjectID);
            StringBuilder menuitems = new();
            ParseSubMenu(menuitems, sao, 1);
            return menuitems.ToString();
        }

        private void ParseSubMenu(StringBuilder menuitems, SystemAccessibleObject sao, int depth) {
            foreach (SystemAccessibleObject c in sao.Children)
                if (c.RoleIndex == 11 || c.RoleIndex == 12) {
                    menuitems.Append(ListContent.Repeat('\t', depth)).Append(c.Name).Append('\n');
                    ParseSubMenu(menuitems, c, depth + 1);
                }
        }

        private string ParseClientArea(SystemWindow sw) {
            SystemAccessibleObject sao = SystemAccessibleObject.FromWindow(sw, AccessibleObjectID.OBJID_CLIENT);
            StringBuilder sb = new();
            ParseClientAreaElement(sb, sao, 1);
            return sb.ToString();
        }

        private void ParseClientAreaElement(StringBuilder sb, SystemAccessibleObject sao, int depth) {
            sb.Append("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~\n");
            sb.Append(ListContent.Repeat('*', depth)).Append(' ').Append(sao).Append('\n');
            try {
                sb.Append("D: ").Append(sao.Description).Append('\n');
            } catch (COMException) { }

            try {
                sb.Append("V: " + sao.Value + "\n");
            } catch (COMException) { }

            foreach (SystemAccessibleObject c in sao.Children)
                if (c.Window == sao.Window)
                    ParseClientAreaElement(sb, c, depth + 1);
        }
    }
}