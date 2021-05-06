// Copyright by Switcheroo

#region

using System;
using System.Collections.Generic;
using System.Text;

#endregion

namespace ManagedWinapi.Windows.Contents {
    /// <summary>
    ///     The content of a list box or combo box.
    /// </summary>
    public class ListContent : WindowContent {
        private readonly string[] values;

        internal ListContent(string type, int selected, string current, string[] values) {
            ComponentType = type;
            SelectedIndex = selected;
            SelectedValue = current;
            this.values = values;
        }

        /// <summary>
        ///     The value in this list or combo box that is selected.
        ///     In a combo box, this value may not be in the list.
        /// </summary>
        public String SelectedValue { get; }

        /// <summary>
        ///     The index of the selected item, or -1 if no item
        ///     is selected.
        /// </summary>
        public int SelectedIndex { get; }

        /// <summary>
        ///     The number of items in this list.
        /// </summary>
        public int Count => values.Length;

        /// <summary>
        ///     Accesses individual list items.
        /// </summary>
        /// <param name="index">Index of list item.</param>
        /// <returns>The list item.</returns>
        public string this[int index] => values[index];

        ///
        public string ComponentType { get; }

        ///
        public string ShortDescription =>
            (SelectedValue == null ? "" : SelectedValue + " ") + "<" + ComponentType + ">";

        ///
        public string LongDescription {
            get {
                StringBuilder sb = new();
                sb.Append('<').Append(ComponentType).Append('>');
                if (SelectedValue != null)
                    sb.Append(" (selected value: \"").Append(SelectedValue).Append("\")");
                sb.Append("\nAll values:\n");
                int idx = 0;
                foreach (string v in values) {
                    if (SelectedIndex == idx)
                        sb.Append("*");
                    sb.Append('\t').Append(v).Append('\n');
                    idx++;
                }

                return sb.ToString();
            }
        }

        ///
        public Dictionary<string, string> PropertyList {
            get {
                Dictionary<string, string> result = new();
                result.Add("SelectedValue", SelectedValue);
                result.Add("SelectedIndex", "" + SelectedIndex);
                result.Add("Count", "" + values.Length);
                for (int i = 0; i < values.Length; i++) result.Add("Value" + i, values[i]);
                return result;
            }
        }

        internal static string Repeat(char ch, int count) {
            char[] tmp = new char[count];
            for (int i = 0; i < tmp.Length; i++) tmp[i] = ch;
            return new string(tmp);
        }
    }
}