using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;

namespace ManagedWinapi {
    /// <summary>
    ///     Provides methods for getting additional information about
    ///     files, like icons or compressed file size.
    /// </summary>
    public sealed class ExtendedFileInfo {
        /// <summary>
        ///     Get the icon used for folders.
        /// </summary>
        /// <param name="small">Whether to get the small icon instead of the large one</param>
        public static Icon GetFolderIcon(bool small) {
            return GetIconForFilename(Environment.GetFolderPath(Environment.SpecialFolder.System), small);
        }

        /// <summary>
        ///     Get the icon used for files that do not have their own icon
        /// </summary>
        /// <param name="small">Whether to get the small icon instead of the large one</param>
        public static Icon GetFileIcon(bool small) {
            return GetExtensionIcon("", small);
        }

        /// <summary>
        ///     Get the icon used by files of a given extension.
        /// </summary>
        /// <param name="extension">The extension without leading dot</param>
        /// <param name="small">Whether to get the small icon instead of the large one</param>
        public static Icon GetExtensionIcon(string extension, bool small) {
            string tmp = Path.GetTempFileName();
            File.Delete(tmp);
            string fn = tmp + "." + extension;
            try {
                File.Create(fn).Close();
                return GetIconForFilename(fn, small);
            } finally {
                File.Delete(fn);
            }
        }

        /// <summary>
        ///     Get the icon used for a given, existing file.
        /// </summary>
        /// <param name="fileName">Name of the file</param>
        /// <param name="small">Whether to get the small icon instead of the large one</param>
        public static Icon GetIconForFilename(string fileName, bool small) {
            SHFILEINFO shinfo = new();

            if (small) {
                IntPtr hImgSmall = SHGetFileInfo(fileName, 0, ref shinfo,
                    (uint)Marshal.SizeOf(shinfo),
                    SHGFI_ICON |
                    SHGFI_SMALLICON);
            } else {
                IntPtr hImgLarge = SHGetFileInfo(fileName, 0,
                    ref shinfo, (uint)Marshal.SizeOf(shinfo),
                    SHGFI_ICON | SHGFI_LARGEICON);
            }

            if (shinfo.hIcon == IntPtr.Zero)
                return null;

            Icon myIcon =
                Icon.FromHandle(shinfo.hIcon);
            return myIcon;
        }

        /// <summary>
        ///     Get the size a file requires on disk. This takes NTFS
        ///     compression into account.
        /// </summary>
        public static ulong GetPhysicalFileSize(string filename) {
            uint low = GetCompressedFileSize(filename, out uint high);
            int error = Marshal.GetLastWin32Error();
            if (error == 32) return (ulong)new FileInfo(filename).Length;
            if (high == 0 && low == 0xFFFFFFFF && error != 0)
                throw new Win32Exception(error);
            return ((ulong)high << 32) + low;
        }

        /// <summary>
        ///     Get the cluster size for the filesystem that contains the given file.
        /// </summary>
        public static uint GetClusterSize(string filename) {
            string drive = Path.GetPathRoot(filename);

            bool hasFreeDiskSpace = GetDiskFreeSpace(drive, out uint lpSectorsPerCluster, out uint lpBytesPerSector,
                out _, out _);
            if (!hasFreeDiskSpace) throw new Win32Exception(Marshal.GetLastWin32Error());
            return lpSectorsPerCluster * lpBytesPerSector;
        }

        private const uint SHGFI_ICON = 0x100;
        private const uint SHGFI_LARGEICON = 0x0; // 'Large icon
        private const uint SHGFI_SMALLICON = 0x1; // 'Small icon

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern bool GetDiskFreeSpace(string lpRootPathName,
            out uint lpSectorsPerCluster,
            out uint lpBytesPerSector,
            out uint lpNumberOfFreeClusters,
            out uint lpTotalNumberOfClusters);

        [DllImport("kernel32.dll")]
        private static extern uint GetCompressedFileSize(string lpFileName,
            out uint lpFileSizeHigh);

        [DllImport("shell32.dll")]
        private static extern IntPtr SHGetFileInfo(string pszPath,
            uint dwFileAttributes,
            ref SHFILEINFO psfi,
            uint cbSizeFileInfo,
            uint uFlags);

        [StructLayout(LayoutKind.Sequential)]
        private struct SHFILEINFO {
            public readonly IntPtr hIcon;
            public readonly IntPtr iIcon;
            public readonly uint dwAttributes;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public readonly string szDisplayName;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
            public readonly string szTypeName;
        }

    }
}