using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

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
        public static SystemTreeView FromSystemWindow(SystemWindow sw) {
            if (sw.SendGetMessage(TVM_GETCOUNT) == 0)
                return null;
            return new SystemTreeView(sw);
        }

        internal static SystemTreeViewItem[] FindSubItems(SystemWindow sw, IntPtr hParent) {
            List<SystemTreeViewItem> result = new List<SystemTreeViewItem>();
            IntPtr hChild;
            HandleRef hr = new HandleRef(sw, sw.HWnd);
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

    /// <summary>
    ///     An item of a tree view.
    /// </summary>
    public class SystemTreeViewItem {
        private readonly IntPtr handle;
        private readonly SystemWindow sw;

        internal SystemTreeViewItem(SystemWindow sw, IntPtr handle) {
            this.sw = sw;
            this.handle = handle;
        }

        /// <summary>
        ///     The title of that item.
        /// </summary>
        public string Title {
            get {
                ProcessMemoryChunk tc = ProcessMemoryChunk.Alloc(sw.Process, 2001);
                TVITEM tvi = new TVITEM();
                tvi.hItem = handle;
                tvi.mask = TVIF_TEXT;
                tvi.cchTextMax = 2000;
                tvi.pszText = tc.Location;
                ProcessMemoryChunk ic = ProcessMemoryChunk.AllocStruct(sw.Process, tvi);
                SystemWindow.SendMessage(new HandleRef(sw, sw.HWnd), TVM_GETITEM, IntPtr.Zero, ic.Location);
                tvi = (TVITEM)ic.ReadToStructure(0, typeof(TVITEM));
                if (tvi.pszText != tc.Location)
                    MessageBox.Show(tvi.pszText + " != " + tc.Location);
                string result = Encoding.Default.GetString(tc.Read());
                if (result.IndexOf('\0') != -1)
                    result = result.Substring(0, result.IndexOf('\0'));
                ic.Dispose();
                tc.Dispose();
                return result;
            }
        }

        /// <summary>
        ///     All child items of that item.
        /// </summary>
        public SystemTreeViewItem[] Children => SystemTreeView.FindSubItems(sw, handle);

        #region PInvoke Declarations

        private static readonly uint TVM_GETITEM = 0x1100 + 12, TVIF_TEXT = 1;

        [StructLayout(LayoutKind.Sequential)]
        private struct TVITEM {
            public UInt32 mask;
            public IntPtr hItem;
            public readonly UInt32 state;
            public readonly UInt32 stateMask;
            public IntPtr pszText;
            public Int32 cchTextMax;
            public readonly Int32 iImage;
            public readonly Int32 iSelectedImage;
            public readonly Int32 cChildren;
            public readonly IntPtr lParam;
        }

        #endregion
    }
}