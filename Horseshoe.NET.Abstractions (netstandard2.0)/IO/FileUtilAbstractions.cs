using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace Horseshoe.NET.IO
{
    /// <summary>
    /// Utility methods for files and directories.
    /// </summary>
    public static class FileUtilAbstractions
    {
        /// <summary>
        /// Formats a file size as a number of bytes, kilobytes, etc.
        /// </summary>
        /// <param name="file">A file</param>
        /// <param name="minDecimalPlaces">Decimal places for rounding</param>
        /// <param name="maxDecimalPlaces">Decimal places for rounding</param>
        /// <param name="addSeparators">Set to <c>true</c> to add separators, such as commas like in culture <c>"en-US"</c></param>
        /// <param name="unit">The preferred file size unit, e.g. KB, MB, etc.  If not supplied the software will use its best guess.</param>
        /// <param name="bi">Whether to use kibibytes vs kilobytes, for example, default is <c>false</c>.</param>
        /// <returns></returns>
        public static string GetDisplayFileSize(FilePath file, int? minDecimalPlaces = null, int? maxDecimalPlaces = null, bool addSeparators = false, FileSizeUnit? unit = null, bool bi = false)
        {
            return GetDisplayFileSize(file.Exists ? (long?)file.Size : null, minDecimalPlaces: minDecimalPlaces, maxDecimalPlaces: maxDecimalPlaces, addSeparators: addSeparators, unit: unit, bi: bi);
        }

        /// <summary>
        /// Formats a file size as a number of bytes, kilobytes, etc.
        /// </summary>
        /// <param name="size">A <c>long</c> representing the size of a file</param>
        /// <param name="minDecimalPlaces">Decimal places for rounding</param>
        /// <param name="maxDecimalPlaces">Decimal places for rounding</param>
        /// <param name="addSeparators">Set to <c>true</c> to add separators, such as commas like in culture <c>"en-US"</c></param>
        /// <param name="unit">The preferred file size unit, e.g. KB, MB, etc.  If not supplied the software will use its best guess.</param>
        /// <param name="bi">Whether to use kibibytes vs kilobytes, for example, default is <c>false</c>.</param>
        /// <returns></returns>
        /// <exception cref="ValidationException"></exception>
        public static string GetDisplayFileSize(long? size, int? minDecimalPlaces = null, int? maxDecimalPlaces = null, bool addSeparators = true, FileSizeUnit? unit = null, bool bi = false)
        {
            if (!size.HasValue) return "-";
            if (size < 0L) return size + (unit.HasValue ? " " + unit : "");
            if (size == 0L) return size + " " + (unit ?? FileSizeUnit.B);

            minDecimalPlaces = minDecimalPlaces ?? 0;
            maxDecimalPlaces = maxDecimalPlaces ?? NumberFormatInfo.CurrentInfo.NumberDecimalDigits;

            if (minDecimalPlaces < 0)
            {
                throw new ValidationException("Minimum decimal places must be >= 0");
            }
            if (maxDecimalPlaces < minDecimalPlaces)
            {
                throw new ValidationException("Maximum decimal places must be >= minimum decimal places");
            }

            if (unit.HasValue)
            {
                if (bi && !IsBi(unit.Value))
                {
                    throw new ValidationException(unit + " is a non-'bi' unit");
                }
                //if (!bi && unit.Value.IsBi())
                //{
                //    throw new ValidationException(unit + " is a 'bi' unit");
                //}
            }
            else
            {
                unit = FileSize.DeriveUnit(size, bi);
            }

            var sb = new StringBuilder();

            if (addSeparators)
            {
                var groupSizes = NumberFormatInfo.CurrentInfo.NumberGroupSizes;
                var groupSep = NumberFormatInfo.CurrentInfo.NumberGroupSeparator;
                sb.Append("#" + groupSep + new string('#', groupSizes[0]));
                sb.Length -= 1;
            }
            sb.Append("0");
            if (maxDecimalPlaces > 0)
            {
                var decimSep = NumberFormatInfo.CurrentInfo.NumberDecimalSeparator;
                sb.Append(decimSep);
                sb.Append(new string('0', minDecimalPlaces.Value));
                sb.Append(new string('#', maxDecimalPlaces.Value));
            }

            var format = "{0:" + sb + " \"" + unit.Value + "\"}";
            var result = size / ToMultiplier(unit.Value);

            return string.Format(format, result);
        }

        /// <summary>
        /// Tests whether <c>directory</c> is a descendant of <c>ancestorDirectory</c>.  The test is a simple
        /// path string comparison.
        /// </summary>
        /// <param name="directory">A directory.</param>
        /// <param name="ancestorDirectory">A directory of whom <c>directory</c> might be a descendant (e.g. subdirectory).</param>
        /// <param name="ignoreCase">If <c>true</c>, does not take sensitivity into account when comparing paths, default is <c>false</c>.</param>
        /// <returns></returns>
        public static bool IsChildOf(DirectoryPath directory, DirectoryPath ancestorDirectory, bool ignoreCase = false)
        {
            return ignoreCase
                ? directory.FullName.IndexOf(ancestorDirectory.FullName, StringComparison.OrdinalIgnoreCase) == 0
                : directory.FullName.IndexOf(ancestorDirectory.FullName, StringComparison.Ordinal) == 0;
        }

        /// <summary>
        /// Tests whether <c>file</c> is a descendant of <c>ancestorDirectory</c>.  The test is a simple
        /// path string comparison.
        /// </summary>
        /// <param name="file">A file.</param>
        /// <param name="ancestorDirectory">A directory of whom <c>file</c> might be a descendant.</param>
        /// <param name="ignoreCase">If <c>true</c>, does not take sensitivity into account when comparing paths, default is <c>false</c>.</param>
        /// <returns></returns>
        public static bool IsChildOf(FilePath file, DirectoryPath ancestorDirectory, bool ignoreCase = false)
        {
            return ignoreCase
                ? file.DirectoryName.IndexOf(ancestorDirectory.FullName, StringComparison.OrdinalIgnoreCase) == 0
                : file.DirectoryName.IndexOf(ancestorDirectory.FullName, StringComparison.Ordinal) == 0;
        }

        /// <summary>
        /// Creates a virtual directory path from a root directory in its tree.
        /// <para>
        /// Example
        /// </para>
        /// <code>
        /// Root      C : \ r o o t     (len = 7)
        /// AltRoot   C : \ r o o t \   (len = 8)
        /// Dir       C : \ r o o t \ s u b d i r 1 \ s u b d i r 2
        /// Cutoff                 └───────... (cutoff = index = len = 7)
        /// Index     0 1 2 3 4 5 6 7
        /// Virtual               ~ \ s u b d i r 1 \ s u b d i r 2
        /// Prepend Indicator ────┘
        /// </code>
        /// </summary>
        /// <param name="directory">A directory</param>
        /// <param name="root">An ancestor directory of <c>directory</c> and the virtual root.</param>
        /// <param name="ignoreCase">If <c>true</c>, does not take sensitivity into account when comparing paths, default is <c>false</c>.</param>
        /// <param name="strict">If <c>true</c>, throws an exception for non ancestor roots, default is <c>false</c>.</param>
        /// <param name="strb">You can optionally supply a <c>StringBuilder</c> instance for GC/heap optimization. The default is to construct a new one on each method call.</param>
        /// <exception cref="ValidationException"></exception>
        public static string DisplayAsVirtualPathFromRoot(DirectoryPath directory, DirectoryPath root, bool ignoreCase = false, bool strict = false, StringBuilder strb = null)
        {
            if (strict && !IsChildOf(directory, root, ignoreCase: ignoreCase))
                throw new ValidationException(directory.Name + " is not a child of " + root.FullName);

            if (strb == null)
                strb = new StringBuilder();
            else
                strb.Clear();

            strb.Append('~');
            if (directory == root)
                strb.Append(Path.DirectorySeparatorChar);
            else
                strb.Append(directory.FullName.Substring(root.FullName.Length + (root.FullName.Last() == Path.DirectorySeparatorChar ? -1 : 0)));
           
            return strb.ToString();
        }

        /// <summary>
        /// Creates a virtual directory path from a root directory in its tree.
        /// <para>
        /// Example
        /// </para>
        /// <code>
        /// Root      C : \ r o o t     (len = 7)
        /// AltRoot   C : \ r o o t \   (len = 8)
        /// Dir       C : \ r o o t \ s u b d i r 1 \ s u b d i r 2 \ f i l e . t x t
        /// Cutoff                 └───────... (cutoff = index = len = 7)
        /// Index     0 1 2 3 4 5 6 7
        /// Virtual               ~ \ s u b d i r 1 \ s u b d i r 2 \ f i l e . t x t
        /// Prepend Indicator ────┘
        /// </code>
        /// </summary>
        /// <param name="file">A file</param>
        /// <param name="root">An ancestor directory of <c>file</c> and the virtual root.</param>
        /// <param name="ignoreCase">If <c>true</c>, does not take sensitivity into account when comparing paths, default is <c>false</c>.</param>
        /// <param name="strict">If <c>true</c>, throws an exception for non ancestor roots, default is <c>false</c>.</param>
        /// <param name="strb">You can optionally supply a <c>StringBuilder</c> instance for GC/heap optimization. The default is to construct a new one on each method call.</param>
        /// <exception cref="ValidationException"></exception>
        public static string DisplayAsVirtualPathFromRoot(FilePath file, DirectoryPath root, bool ignoreCase = false, bool strict = false, StringBuilder strb = null)
        {
            if (strict && !IsChildOf(file, root, ignoreCase: ignoreCase))
                throw new ValidationException(file.Name + " is not a child of " + root.FullName);

            if (strb == null)
                strb = new StringBuilder();
            else
                strb.Clear();

            strb.Append('~')
                .Append(file.FullName.Substring(root.FullName.Length + (root.FullName.Last() == Path.DirectorySeparatorChar ? -1 : 0)));
        
            return strb.ToString();
        }

        /// <summary>
        /// Returns the multiplier associated with the file size unit; e.g. KiB -> 1024, MB -> 1000000, etc.
        /// </summary>
        /// <param name="unit">A file size unit.</param>
        /// <returns><c>true</c> or <c>false</c></returns>
        public static double ToMultiplier(FileSizeUnit unit)
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
        /// Tests if a file size unit is a binary file size unit, e.g. KiB (kibibytes), MiB (mebibytes), etc.
        /// </summary>
        /// <param name="unit">A file size unit</param>
        /// <returns><c>true</c> or <c>false</c></returns>
        public static bool IsBi(FileSizeUnit unit)
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
    }
}
