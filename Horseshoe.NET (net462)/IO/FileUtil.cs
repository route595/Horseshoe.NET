using System.Globalization;
using System.IO;
using System.Text;

using Horseshoe.NET.Text;

namespace Horseshoe.NET.IO
{
    /// <summary>
    /// Utility methods for files and directories
    /// </summary>
    public static class FileUtil
    {
        /// <summary>
        /// Appends an extension to a file name
        /// </summary>
        /// <param name="fileName">A file name</param>
        /// <param name="extension">A common file type extension</param>
        /// <param name="preferUpperCase">Optional, set to <c>true</c> to append the extension in upper case, default is <c>false</c></param>
        /// <returns></returns>
        public static string AppendExtension(string fileName, FileType extension, bool preferUpperCase = false)
        {
            return AppendExtension(fileName, preferUpperCase ? extension.ToString() : extension.ToString().ToLower());
        }

        /// <summary>
        /// Appends an extension to a file name
        /// </summary>
        /// <param name="fileName">A file name</param>
        /// <param name="extension">An extension</param>
        /// <returns></returns>
        public static string AppendExtension(string fileName, string extension)
        {
            if (!extension.StartsWith("."))
                extension = "." + extension;
            if (fileName.ToLower().EndsWith(extension.ToLower()))
                return fileName;
            return fileName + extension;
        }

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
            return GetDisplayFileSize(file.Exists ? file.Length : -1L, minDecimalPlaces: minDecimalPlaces, maxDecimalPlaces: maxDecimalPlaces, addSeparators: addSeparators, unit: unit, bi: bi);
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
        public static string GetDisplayFileSize(long size, int? minDecimalPlaces = null, int? maxDecimalPlaces = null, bool addSeparators = true, FileSizeUnit? unit = null, bool bi = false)
        {
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
                if (bi && !unit.Value.IsBi())
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
                sb.Append("#" + groupSep + "#".Repeat(groupSizes[0]));
                sb.Length -= 1;
            }
            sb.Append("0");
            if (maxDecimalPlaces > 0)
            {
                var decimSep = NumberFormatInfo.CurrentInfo.NumberDecimalSeparator;
                sb.Append(decimSep);
                sb.Append("0".Repeat(minDecimalPlaces.Value));
                sb.Append("#".Repeat(maxDecimalPlaces.Value));
            }

            var format = "{0:" + sb + " \"" + unit.Value + "\"}";
            var result = size / unit.Value.ToMultiplier();

            return string.Format(format, result);
        }

        /// <summary>
        /// Ensures paths end with path separator - internal method
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string NormalizePath(string path)
        {
            path = path.Trim();
            if (path[path.Length - 1] != Path.DirectorySeparatorChar)
            {
                path += Path.DirectorySeparatorChar;
            }
            return path;
        }
    }
}
