namespace ManagedWinapi.Windows
{
    /// <summary>
    ///     A column of a list view.
    /// </summary>
    public class SystemListViewColumn {
        internal SystemListViewColumn(int format, int width, int subIndex, string title) {
            Format = format;
            Width = width;
            SubIndex = subIndex;
            Title = title;
        }

        /// <summary>
        ///     The format (like left justified) of this column.
        /// </summary>
        public int Format { get; }

        /// <summary>
        ///     The width of this column.
        /// </summary>
        public int Width { get; }

        /// <summary>
        ///     The subindex of the subitem displayed in this column. Note
        ///     that the second column does not necessarily display the second
        ///     subitem - especially when the columns can be reordered by the user.
        /// </summary>
        public int SubIndex { get; }

        /// <summary>
        ///     The title of this column.
        /// </summary>
        public string Title { get; }
    }
}