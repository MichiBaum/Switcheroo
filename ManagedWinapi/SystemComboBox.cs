using System;
using System.Runtime.InteropServices;
using System.Text;

namespace ManagedWinapi.Windows
{
    /// <summary>
    ///     Any combo box, including those from other applications.
    /// </summary>
    public class SystemComboBox {
        #region PInvoke Declarations

        private static readonly uint CB_GETCOUNT = 0x146,
            CB_GETLBTEXT = 0x148,
            CB_GETLBTEXTLEN = 0x149;

        #endregion

        private SystemComboBox(SystemWindow sw) {
            SystemWindow = sw;
        }

        /// <summary>
        ///     The SystemWindow instance that represents this combo box.
        /// </summary>
        public SystemWindow SystemWindow { get; }

        /// <summary>
        ///     The number of elements in this combo box.
        /// </summary>
        public int Count => SystemWindow.SendGetMessage(CB_GETCOUNT);

        /// <summary>
        ///     Gets an element by index.
        /// </summary>
        public string this[int index] {
            get {
                if (index < 0 || index >= Count) throw new ArgumentException("Argument out of range");
                int length = SystemWindow.SendGetMessage(CB_GETLBTEXTLEN, (uint)index);
                StringBuilder sb = new(length);
                SystemWindow.SendMessage(new HandleRef(this, SystemWindow.HWnd), CB_GETLBTEXT, new IntPtr(index), sb);
                return sb.ToString();
            }
        }

        /// <summary>
        ///     Get a SystemComboBox reference from a SystemWindow (which is a combo box)
        /// </summary>
        public static SystemComboBox FromSystemWindow(SystemWindow sw) {
            if (sw.SendGetMessage(CB_GETCOUNT) == 0)
                return null;
            return new SystemComboBox(sw);
        }
    }
}