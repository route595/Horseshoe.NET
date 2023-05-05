using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Horseshoe.NET.IO
{
    /// <summary>
    /// Extension methods for IO such as streams, files and directories
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Tests whether a directory is devoid of any subdirectories and files.
        /// </summary>
        /// <param name="dir">A directory.</param>
        /// <returns><c>true</c> or <c>false</c></returns>
        public static bool IsEmpty(this DirectoryInfo dir)
        {
            return !dir.GetFileSystemInfos().Any();
        }

        /// <summary>
        /// Reads an entire stream iteratively and returns its contents as a <c>byte[]</c>.
        /// </summary>
        /// <param name="inputStream">A stream.</param>
        /// <param name="bufferSize">The number of bytes read in each iteration, default is <c>1024</c>.  This might have performance implications.</param>
        /// <returns></returns>
        public static byte[] ReadAllBytes(this Stream inputStream, int bufferSize = 1024)
        {
            var list = new List<byte>();
            var buf = new byte[bufferSize];
            var bytesRead = inputStream.Read(buf, 0, buf.Length);
            while (bytesRead > 0)
            {
                list.AddRange(buf.Take(bytesRead));
                bytesRead = inputStream.Read(buf, 0, buf.Length);
            }
            return list.ToArray();
        }

        /// <summary>
        /// Reads an entire stream iteratively and returns its contents as a <c>byte[]</c>.
        /// </summary>
        /// <param name="inputStream">A stream.</param>
        /// <param name="bufferSize">The number of bytes read in each iteration, default is <c>1024</c>.  This might have performance implications.</param>
        /// <returns></returns>
        public static async Task<byte[]> ReadAllBytesAsync(this Stream inputStream, int bufferSize = 1024)
        {
            var list = new List<byte>();
            var buf = new byte[bufferSize];
            var bytesRead = await inputStream.ReadAsync(buf, 0, buf.Length);
            while (bytesRead > 0)
            {
                list.AddRange(buf.Take(bytesRead));
                bytesRead = await inputStream.ReadAsync(buf, 0, buf.Length);
            }
            return list.ToArray();
        }

        /// <summary>
        /// Tests if a file size unit is a binary file size unit, e.g. KiB (kibibytes), MiB (mebibytes), etc.
        /// </summary>
        /// <param name="unit">A file size unit</param>
        /// <returns></returns>
        public static bool IsBi(this FileSizeUnit unit)
        {
            switch (unit)
            {
                case FileSizeUnit.KiB:
                case FileSizeUnit.MiB:
                case FileSizeUnit.GiB:
                case FileSizeUnit.TiB:
                case FileSizeUnit.PiB:
                case FileSizeUnit.EiB:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Returns the multiplier associated with the file size unit; e.g. KiB -> 1024, MB -> 1000000, etc.
        /// </summary>
        /// <param name="unit"></param>
        /// <returns></returns>
        public static double ToMultiplier(this FileSizeUnit unit)
        {
            switch (unit)
            {
                case FileSizeUnit.KB: return FileSize.KB;
                case FileSizeUnit.MB: return FileSize.MB;
                case FileSizeUnit.GB: return FileSize.GB;
                case FileSizeUnit.TB: return FileSize.TB;
                case FileSizeUnit.PB: return FileSize.PB;
                case FileSizeUnit.EB: return FileSize.EB;
                case FileSizeUnit.KiB: return FileSize.KiB;
                case FileSizeUnit.MiB: return FileSize.MiB;
                case FileSizeUnit.GiB: return FileSize.GiB;
                case FileSizeUnit.TiB: return FileSize.TiB;
                case FileSizeUnit.PiB: return FileSize.PiB;
                case FileSizeUnit.EiB: return FileSize.EiB;
                default: return 1D; // byte
            }
        }

        /// <summary>
        /// Tests whether this directory is in the directory tree of the supplied directory.
        /// </summary>
        /// <param name="dir">The current directory.</param>
        /// <param name="otherDir">Another directory.</param>
        /// <param name="ignoreCase">If <c>true</c>, does not take sensitivity into account when comparing paths, default is <c>false</c>.</param>
        /// <returns></returns>
        public static bool IsChildOf(this DirectoryPath dir, DirectoryPath otherDir, bool ignoreCase = false)
        {
            return ignoreCase
                ? dir.FullName.IndexOf(otherDir.FullName, StringComparison.OrdinalIgnoreCase) == 0
                : dir.FullName.IndexOf(otherDir.FullName, StringComparison.Ordinal) == 0;
        }

        /// <summary>
        /// Tests whether this file is in the directory tree of the supplied directory.
        /// </summary>
        /// <param name="file">The current file.</param>
        /// <param name="path">A directory.</param>
        /// <param name="ignoreCase">If <c>true</c>, does not take sensitivity into account when comparing paths, default is <c>false</c>.</param>
        /// <returns></returns>
        public static bool IsChildOf(this FilePath file, DirectoryPath path, bool ignoreCase = false)
        {
            return ignoreCase
                ? file.DirectoryName.IndexOf(path.FullName, StringComparison.OrdinalIgnoreCase) == 0
                : file.DirectoryName.IndexOf(path.FullName, StringComparison.Ordinal) == 0;
        }

        /// <summary>
        /// Tests whether this directory is in the directory tree of a directory named <c>directoryName</c>.
        /// </summary>
        /// <param name="directoryPath">The current directory.</param>
        /// <param name="directoryName">The name of a directory.</param>
        /// <param name="ignoreCase">If <c>true</c>, does not take sensitivity into account when comparing paths, default is <c>false</c>.</param>
        /// <returns></returns>
        public static bool ContainsParent(this DirectoryPath directoryPath, string directoryName, bool ignoreCase = false)
        {
            var parent = directoryPath.Directory.Parent;
            while (parent != null)
            {
                if (ignoreCase
                        ? string.Equals(parent.Name, directoryName, StringComparison.OrdinalIgnoreCase)
                        : string.Equals(parent.Name, directoryName))
                    return true;
                parent = parent.Parent;
            }

            return false;
        }

        private readonly static StringBuilder strb_Optimized = new StringBuilder();

        /// <summary>
        /// Displays a directory as a virtual path based on the supplied root which is represented by a tilde (~).
        /// </summary>
        /// <remarks>
        /// For example...
        /// <code>
        /// DirectoryPath root = @"C:\dirA\dirB";
        /// DirectoryPath dir = @"C:\dirA\dirB\subdir1\subdir2";
        /// var result = dir.DisplayAsVirtualPathFromRoot(root);  // -&gt; "~\subdir1\subdir2"
        /// 
        /// root = "/etc/dirA/dirB";
        /// dir = "/etc/dirA/dirB/subdir1/subdir2";
        /// var result = dir.DisplayAsVirtualPathFromRoot(root);  // -&gt; "~/subdir1/subdir2"
        /// </code>
        /// </remarks>
        /// <param name="dir">The current directory.</param>
        /// <param name="root">The root directory upon which to base the virtual path.</param>
        /// <param name="ignoreCase">If <c>true</c>, does not take sensitivity into account when comparing paths, default is <c>false</c>.</param>
        /// <param name="strict">Whether to ensure that <c>dir</c> is a child directory of <c>root</c>, default is <c>true</c>.</param>
        /// <returns>A virtual path e.g. "~\subdir1\subdir2" or "~/subdir1/subdir2"</returns>
        /// <exception cref="ValidationException"></exception>
        public static string DisplayAsVirtualPathFromRoot(this DirectoryPath dir, DirectoryPath root, bool ignoreCase = false, bool strict = true)
        {
            var strb = new StringBuilder();
            DisplayAsVirtualPathFromRoot(strb, dir, root, ignoreCase, strict);
            return strb.ToString();
        }

        /// <summary>
        /// Displays a directory as a virtual path based on the supplied root which is represented by a tilde (~).
        /// </summary>
        /// <remarks>
        /// For example...
        /// <code>
        /// DirectoryPath root = @"C:\dirA\dirB";
        /// DirectoryPath dir = @"C:\dirA\dirB\subdir1\subdir2";
        /// var result = dir.DisplayAsVirtualPathFromRoot(root);  // -&gt; "~\subdir1\subdir2"
        /// 
        /// root = "/etc/dirA/dirB";
        /// dir = "/etc/dirA/dirB/subdir1/subdir2";
        /// var result = dir.DisplayAsVirtualPathFromRoot(root);  // -&gt; "~/subdir1/subdir2"
        /// </code>
        /// </remarks>
        /// <param name="dir">The current directory.</param>
        /// <param name="root">The root directory upon which to base the virtual path.</param>
        /// <param name="ignoreCase">If <c>true</c>, does not take sensitivity into account when comparing paths, default is <c>false</c>.</param>
        /// <param name="strict">Whether to ensure that <c>dir</c> is a child directory of <c>root</c>, default is <c>true</c>.</param>
        /// <returns>A virtual path e.g. "~\subdir1\subdir2" or "~/subdir1/subdir2"</returns>
        /// <exception cref="ValidationException"></exception>
        public static string DisplayAsVirtualPathFromRoot_Optimized(this DirectoryPath dir, DirectoryPath root, bool ignoreCase = false, bool strict = true)
        {
            strb_Optimized.Clear();
            DisplayAsVirtualPathFromRoot(strb_Optimized, dir, root, ignoreCase, strict);
            return strb_Optimized.ToString();
        }

        private static void DisplayAsVirtualPathFromRoot(StringBuilder strb, DirectoryPath dir, DirectoryPath root, bool ignoreCase, bool strict)
        {
            if (strict && !dir.IsChildOf(root, ignoreCase: ignoreCase))
                throw new ValidationException(dir.Name + " is not a child of " + root.FullName);

            strb.Append('~');
            if (dir == root)
                strb.Append(Path.DirectorySeparatorChar);
            else
                strb.Append(dir.FullName.Substring(root.FullName.Length + (root.FullName.Last() == Path.DirectorySeparatorChar ? -1 : 0)));

            // Root      C : \ r o o t     (len = 7)*
            // AltRoot   C : \ r o o t \   (len = 8)
            // Dir       C : \ r o o t \ s u b d i r 1 \ s u b d i r 2
            // Cutoff                 └───────... (cutoff = index = len = 7)
            // Index     0 1 2 3 4 5 6 7
            // Virtual               ~ \ s u b d i r 1 \ s u b d i r 2
            // Prepend Indicator ────┘
        }

        /// <summary>
        /// Displays a file as a virtual path based on the supplied root which is represented by a tilde (~).
        /// </summary>
        /// <remarks>
        /// For example...
        /// <code>
        /// DirectoryPath root = @"C:\dirA\dirB";
        /// FilePath file = @"C:\dirA\dirB\subdir1\subdir2\file.txt";
        /// var result = file.DisplayAsVirtualPathFromRoot(root);  // -&gt; "~\subdir1\subdir2\file.txt"
        /// 
        /// root = "/etc/dirA/dirB";
        /// file = "/etc/dirA/dirB/subdir1/subdir2/file.txt";
        /// var result = file.DisplayAsVirtualPathFromRoot(root);  // -&gt; "~/subdir1/subdir2/file.txt"
        /// </code>
        /// </remarks>
        /// <param name="file">The current file.</param>
        /// <param name="root">The root directory upon which to base the virtual path.</param>
        /// <param name="ignoreCase">If <c>true</c>, does not take sensitivity into account when comparing paths, default is <c>false</c>.</param>
        /// <param name="strict">Whether to ensure that <c>file</c> is a child file of <c>root</c>, default is <c>true</c>.</param>
        /// <returns>A virtual path e.g. "~\subdir1\subdir2\file.txt" or "~/subdir1/subdir2/file.txt"</returns>
        /// <exception cref="ValidationException"></exception>
        public static string DisplayAsVirtualPathFromRoot(this FilePath file, DirectoryPath root, bool ignoreCase = false, bool strict = true)
        {
            var strb = new StringBuilder();
            DisplayAsVirtualPathFromRoot(strb, file, root, ignoreCase, strict);
            return strb.ToString();
        }

        /// <summary>
        /// Displays a file as a virtual path based on the supplied root which is represented by a tilde (~).
        /// </summary>
        /// <remarks>
        /// For example...
        /// <code>
        /// DirectoryPath root = @"C:\dirA\dirB";
        /// FilePath file = @"C:\dirA\dirB\subdir1\subdir2\file.txt";
        /// var result = file.DisplayAsVirtualPathFromRoot(root);  // -&gt; "~\subdir1\subdir2\file.txt"
        /// 
        /// root = "/etc/dirA/dirB";
        /// file = "/etc/dirA/dirB/subdir1/subdir2/file.txt";
        /// var result = file.DisplayAsVirtualPathFromRoot(root);  // -&gt; "~/subdir1/subdir2/file.txt"
        /// </code>
        /// </remarks>
        /// <param name="file">The current file.</param>
        /// <param name="root">The root directory upon which to base the virtual path.</param>
        /// <param name="ignoreCase">If <c>true</c>, does not take sensitivity into account when comparing paths, default is <c>false</c>.</param>
        /// <param name="strict">Whether to ensure that <c>file</c> is a child file of <c>root</c>, default is <c>true</c>.</param>
        /// <returns>A virtual path e.g. "~\subdir1\subdir2\file.txt" or "~/subdir1/subdir2/file.txt"</returns>
        /// <exception cref="ValidationException"></exception>
        public static string DisplayAsVirtualPathFromRoot_Optimized(this FilePath file, DirectoryPath root, bool ignoreCase = false, bool strict = true)
        {
            strb_Optimized.Clear();
            DisplayAsVirtualPathFromRoot(strb_Optimized, file, root, ignoreCase, strict);
            return strb_Optimized.ToString();
        }

        private static void DisplayAsVirtualPathFromRoot(StringBuilder strb, FilePath file, DirectoryPath root, bool ignoreCase, bool strict)
        {
            if (strict && !file.IsChildOf(root, ignoreCase: ignoreCase))
                throw new ValidationException(file.Name + " is not a child of " + root.FullName);

            strb.Append('~')
                .Append(file.FullName.Substring(root.FullName.Length + (root.FullName.Last() == Path.DirectorySeparatorChar ? -1 : 0)));

            // Root      C : \ r o o t     (len = 7)*
            // AltRoot   C : \ r o o t \   (len = 8)
            // File      C : \ r o o t \ s u b d i r 1 \ s u b d i r 2 \ f i l e . t x t
            // Cutoff                 └───────... (cutoff = index = len = 7)
            // Index     0 1 2 3 4 5 6 7
            // Virtual               ~ \ s u b d i r 1 \ s u b d i r 2 \ f i l e . t x t
            // Prepend Indicator ────┘
        }
    }
}
