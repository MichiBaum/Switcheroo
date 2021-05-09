using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace ManagedWinapi.Windows {
    /// <summary>
    ///     Any list view, including those from other applications.
    /// </summary>
    public class SystemListView {
        private readonly SystemWindow sw;

        private SystemListView(SystemWindow sw) {
            this.sw = sw;
        }

        /// <summary>
        ///     The number of items (icons) in this list view.
        /// </summary>
        public int Count => sw.SendGetMessage(LVM_GETITEMCOUNT);

        /// <summary>
        ///     An item of this list view.
        /// </summary>
        public SystemListViewItem this[int index] => this[index, 0];

        /// <summary>
        ///     A subitem (a column value) of an item of this list view.
        /// </summary>
        public SystemListViewItem this[int index, int subIndex] {
            get {
                LVITEM lvi = new();
                lvi.cchTextMax = 300;
                lvi.iItem = index;
                lvi.iSubItem = subIndex;
                lvi.stateMask = 0xffffffff;
                lvi.mask = LVIF_IMAGE | LVIF_STATE | LVIF_TEXT;
                ProcessMemoryChunk tc = ProcessMemoryChunk.Alloc(sw.Process, 301);
                lvi.pszText = tc.Location;
                ProcessMemoryChunk lc = ProcessMemoryChunk.AllocStruct(sw.Process, lvi);
                ApiHelper.FailIfZero(SystemWindow.SendMessage(new HandleRef(sw, sw.HWnd), LVM_GETITEM, IntPtr.Zero,
                    lc.Location));
                lvi = (LVITEM)lc.ReadToStructure(0, typeof(LVITEM));
                lc.Dispose();
                if (lvi.pszText != tc.Location) {
                    tc.Dispose();
                    tc = new ProcessMemoryChunk(sw.Process, lvi.pszText, lvi.cchTextMax);
                }

                byte[] tmp = tc.Read();
                string title = Encoding.Default.GetString(tmp);
                if (title.IndexOf('\0') != -1)
                    title = title.Substring(0, title.IndexOf('\0'));
                int image = lvi.iImage;
                uint state = lvi.state;
                tc.Dispose();
                return new SystemListViewItem(sw, index, title, state, image);
            }
        }

        /// <summary>
        ///     All columns of this list view, if it is in report view.
        /// </summary>
        public SystemListViewColumn[] Columns {
            get {
                List<SystemListViewColumn> result = new();
                LVCOLUMN lvc = new();
                lvc.cchTextMax = 300;
                lvc.mask = LVCF_FMT | LVCF_SUBITEM | LVCF_TEXT | LVCF_WIDTH;
                ProcessMemoryChunk tc = ProcessMemoryChunk.Alloc(sw.Process, 301);
                lvc.pszText = tc.Location;
                ProcessMemoryChunk lc = ProcessMemoryChunk.AllocStruct(sw.Process, lvc);
                for (int i = 0;; i++) {
                    IntPtr ok = SystemWindow.SendMessage(new HandleRef(sw, sw.HWnd), LVM_GETCOLUMN, new IntPtr(i),
                        lc.Location);
                    if (ok == IntPtr.Zero)
                        break;
                    lvc = (LVCOLUMN)lc.ReadToStructure(0, typeof(LVCOLUMN));
                    byte[] tmp = tc.Read();
                    string title = Encoding.Default.GetString(tmp);
                    if (title.IndexOf('\0') != -1)
                        title = title.Substring(0, title.IndexOf('\0'));
                    result.Add(new SystemListViewColumn(lvc.fmt, lvc.cx, lvc.iSubItem, title));
                }

                tc.Dispose();
                lc.Dispose();
                return result.ToArray();
            }
        }

        /// <summary>
        ///     Get a SystemListView reference from a SystemWindow (which is a list view)
        /// </summary>
        public static SystemListView? FromSystemWindow(SystemWindow sw) {
            if (sw.SendGetMessage(LVM_GETITEMCOUNT) == 0)
                return null;
            return new SystemListView(sw);
        }

        internal static readonly uint LVM_GETITEMRECT = 0x1000 + 14,
            LVM_SETITEMPOSITION = 0x1000 + 15,
            LVM_GETITEMPOSITION = 0x1000 + 16,
            LVM_GETITEMCOUNT = 0x1000 + 4,
            LVM_GETITEM = 0x1005,
            LVM_GETCOLUMN = 0x1000 + 25;

        private static readonly uint LVIF_TEXT = 0x1,
            LVIF_IMAGE = 0x2,
            LVIF_STATE = 0x8,
            LVCF_FMT = 0x1,
            LVCF_WIDTH = 0x2,
            LVCF_TEXT = 0x4,
            LVCF_SUBITEM = 0x8;

        [StructLayout(LayoutKind.Sequential)]
        private struct LVCOLUMN {
            public UInt32 mask;
            public readonly Int32 fmt;
            public readonly Int32 cx;
            public IntPtr pszText;
            public Int32 cchTextMax;
            public readonly Int32 iSubItem;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct LVITEM {
            public UInt32 mask;
            public Int32 iItem;
            public Int32 iSubItem;
            public readonly UInt32 state;
            public UInt32 stateMask;
            public IntPtr pszText;
            public Int32 cchTextMax;
            public readonly Int32 iImage;
            public readonly IntPtr lParam;
        }
    }
}