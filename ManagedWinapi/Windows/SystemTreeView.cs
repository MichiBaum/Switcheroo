using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace ManagedWinapi.Windows {
    /// <summary>
    ///     Any tree view, including those from other applications.
    /// </summary>
    public class SystemTreeView {
        #region PInvoke Declarations

        private static readonly uint TVM_GETCOUNT = 0x1100 + 5,
            TVM_GETNEXTITEM = 0x1100 + 10,
            TVGN_ROOT = 0,
            TVGN_NEXT = 1,
            TVGN_CHILD = 4;

        #endregion

        internal readonly SystemWindow sw;

        private SystemTreeView(SystemWindow sw) {
            this.sw = sw;
        }

        /// <summary>
        ///     The number of items (icons) in this tree view.
        /// </summary>
        public int Count => sw.SendGetMessage(TVM_GETCOUNT);

        /// <summary>
        ///     The root items of this tree view.
        /// </summary>
        public SystemTreeViewItem[] Roots => FindSubItems(sw, IntPtr.Zero);

        /// <summary>
        ///     Get a SystemTreeView reference from a SystemWindow (which is a tree view)
        /// </summary>
        public static SystemTreeView? FromSystemWindow(SystemWindow sw) {
            if (sw.SendGetMessage(TVM_GETCOUNT) == 0)
                return null;
            return new SystemTreeView(sw);
        }

        internal static SystemTreeViewItem[] FindSubItems(SystemWindow sw, IntPtr hParent) {
            List<SystemTreeViewItem> result = new();
            IntPtr hChild;
            HandleRef hr = new(sw, sw.HWnd);
            if (hParent == IntPtr.Zero)
                hChild = SystemWindow.SendMessage(hr, TVM_GETNEXTITEM, new IntPtr(TVGN_ROOT), IntPtr.Zero);
            else
                hChild = SystemWindow.SendMessage(hr, TVM_GETNEXTITEM, new IntPtr(TVGN_CHILD), hParent);
            while (hChild != IntPtr.Zero) {
                result.Add(new SystemTreeViewItem(sw, hChild));
                hChild = SystemWindow.SendMessage(hr, TVM_GETNEXTITEM, new IntPtr(TVGN_NEXT), hChild);
            }

            return result.ToArray();
        }
    }
}