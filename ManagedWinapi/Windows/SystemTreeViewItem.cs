using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace ManagedWinapi.Windows {
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
                TVITEM tvi = new();
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
    }
}