using System;
using System.Runtime.InteropServices;
using System.Text;

namespace ManagedWinapi.Windows {
    /// <summary>
    ///     Any list box, including those from other applications.
    /// </summary>
    public class SystemListBox {
        private static readonly uint LB_GETTEXT = 0x189,
            LB_GETTEXTLEN = 0x18A,
            //    LB_GETSEL = 0x187,
            LB_GETCURSEL = 0x188,
            //    LB_GETSELCOUNT = 0x190,
            //    LB_GETSELITEMS = 0x191,
            LB_GETCOUNT = 0x18B;

        private SystemListBox(SystemWindow sw) {
            SystemWindow = sw;
        }

        /// <summary>
        ///     The SystemWindow instance that represents this list box.
        /// </summary>
        public SystemWindow SystemWindow { get; }

        /// <summary>
        ///     The number of elements in this list box.
        /// </summary>
        public int Count => SystemWindow.SendGetMessage(LB_GETCOUNT);

        /// <summary>
        ///     The index of the selected element in this list box.
        /// </summary>
        public int SelectedIndex => SystemWindow.SendGetMessage(LB_GETCURSEL);

        /// <summary>
        ///     The selected element in this list box.
        /// </summary>
        public string SelectedItem {
            get {
                int idx = SelectedIndex;
                if (idx == -1)
                    return null;
                return this[idx];
            }
        }

        /// <summary>
        ///     Get an element of this list box by index.
        /// </summary>
        public string this[int index] {
            get {
                if (index < 0 || index >= Count) throw new ArgumentException("Argument out of range");
                int length = SystemWindow.SendGetMessage(LB_GETTEXTLEN, (uint)index);
                StringBuilder sb = new(length);
                SystemWindow.SendMessage(new HandleRef(this, SystemWindow.HWnd), LB_GETTEXT, new IntPtr(index), sb);
                return sb.ToString();
            }
        }

        /// <summary>
        ///     Get a SystemListBox reference from a SystemWindow (which is a list box)
        /// </summary>
        public static SystemListBox FromSystemWindow(SystemWindow sw) {
            if (sw.SendGetMessage(LB_GETCOUNT) == 0)
                return null;
            return new SystemListBox(sw);
        }
    }
}