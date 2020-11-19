using System;
using System.Runtime.InteropServices;
using System.Text;

namespace ManagedWinapi.Windows {
    /// <summary>
    /// Any list box, including those from other applications.
    /// </summary>
    public class SystemListBox {
        /// <summary>
        /// Get a SystemListBox reference from a SystemWindow (which is a list box)
        /// </summary>
        public static SystemListBox FromSystemWindow(SystemWindow sw) {
            if (sw.SendGetMessage(LB_GETCOUNT) == 0)
                return null;
            return new SystemListBox(sw);
        }

        private SystemWindow sw;

        private SystemListBox(SystemWindow sw) {
            this.sw = sw;
        }

        /// <summary>
        /// The SystemWindow instance that represents this list box.
        /// </summary>
        public SystemWindow SystemWindow {
            get { return sw; }
        }

        /// <summary>
        /// The number of elements in this list box.
        /// </summary>
        public int Count {
            get {
                return sw.SendGetMessage(LB_GETCOUNT);
            }
        }

        /// <summary>
        /// The index of the selected element in this list box.
        /// </summary>
        public int SelectedIndex {
            get {
                return sw.SendGetMessage(LB_GETCURSEL);
            }
        }

        /// <summary>
        /// The selected element in this list box.
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
        /// Get an element of this list box by index.
        /// </summary>
        public string this[int index] {
            get {
                if (index < 0 || index >= Count) {
                    throw new ArgumentException("Argument out of range");
                }
                int length = sw.SendGetMessage(LB_GETTEXTLEN, (uint)index);
                StringBuilder sb = new StringBuilder(length);
                SystemWindow.SendMessage(new HandleRef(this, sw.HWnd), LB_GETTEXT, new IntPtr(index), sb);
                return sb.ToString();

            }
        }

        #region PInvoke Declarations

        private static readonly uint LB_GETTEXT = 0x189,
            LB_GETTEXTLEN = 0x18A,
        //    LB_GETSEL = 0x187,
            LB_GETCURSEL = 0x188,
        //    LB_GETSELCOUNT = 0x190,
        //    LB_GETSELITEMS = 0x191,
            LB_GETCOUNT = 0x18B;
        #endregion
    }

    /// <summary>
    /// Any combo box, including those from other applications.
    /// </summary>
    public class SystemComboBox {
        /// <summary>
        /// Get a SystemComboBox reference from a SystemWindow (which is a combo box)
        /// </summary>
        public static SystemComboBox FromSystemWindow(SystemWindow sw) {
            if (sw.SendGetMessage(CB_GETCOUNT) == 0)
                return null;
            return new SystemComboBox(sw);
        }

        SystemWindow sw;

        private SystemComboBox(SystemWindow sw) {
            this.sw = sw;
        }

        /// <summary>
        /// The SystemWindow instance that represents this combo box.
        /// </summary>
        public SystemWindow SystemWindow {
            get { return sw; }
        }

        /// <summary>
        /// The number of elements in this combo box.
        /// </summary>
        public int Count {
            get {
                return sw.SendGetMessage(CB_GETCOUNT);
            }
        }

        /// <summary>
        /// Gets an element by index.
        /// </summary>
        public string this[int index] {
            get {
                if (index < 0 || index >= Count) {
                    throw new ArgumentException("Argument out of range");
                }
                int length = sw.SendGetMessage(CB_GETLBTEXTLEN, (uint)index);
                StringBuilder sb = new StringBuilder(length);
                SystemWindow.SendMessage(new HandleRef(this, sw.HWnd), CB_GETLBTEXT, new IntPtr(index), sb);
                return sb.ToString();

            }
        }

        #region PInvoke Declarations

        private static readonly uint CB_GETCOUNT = 0x146,
            CB_GETLBTEXT = 0x148,
            CB_GETLBTEXTLEN = 0x149;

        #endregion
    }
}
