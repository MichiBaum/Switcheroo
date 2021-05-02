using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace ManagedWinapi.Windows {
    /// <summary>
    ///     An item of a list view.
    /// </summary>
    public class SystemListViewItem {
        private readonly int index;
        private readonly SystemWindow sw;

        internal SystemListViewItem(SystemWindow sw, int index, string title, uint state, int image) {
            this.sw = sw;
            this.index = index;
            Title = title;
            State = state;
            Image = image;
        }

        /// <summary>
        ///     The title of this item
        /// </summary>
        public string Title { get; }

        /// <summary>
        ///     The index of this item's image in the image list of this list view.
        /// </summary>
        public int Image { get; }

        /// <summary>
        ///     State bits of this item.
        /// </summary>
        public uint State { get; }

        /// <summary>
        ///     Position of the upper left corner of this item.
        /// </summary>
        public Point Position {
            get {
                POINT pt = new();
                ProcessMemoryChunk c = ProcessMemoryChunk.AllocStruct(sw.Process, pt);
                ApiHelper.FailIfZero(SystemWindow.SendMessage(new HandleRef(sw, sw.HWnd),
                    SystemListView.LVM_GETITEMPOSITION, new IntPtr(index), c.Location));
                pt = (POINT)c.ReadToStructure(0, typeof(POINT));
                return new Point(pt.X, pt.Y);
            }
            set => SystemWindow.SendMessage(new HandleRef(sw, sw.HWnd), SystemListView.LVM_SETITEMPOSITION,
                new IntPtr(index), new IntPtr(value.X + (value.Y << 16)));
        }

        /// <summary>
        ///     Bounding rectangle of this item.
        /// </summary>
        public RECT Rectangle {
            get {
                RECT r = new();
                ProcessMemoryChunk c = ProcessMemoryChunk.AllocStruct(sw.Process, r);
                SystemWindow.SendMessage(new HandleRef(sw, sw.HWnd), SystemListView.LVM_GETITEMRECT, new IntPtr(index),
                    c.Location);
                return (RECT)c.ReadToStructure(0, typeof(RECT));
            }
        }
    }
}