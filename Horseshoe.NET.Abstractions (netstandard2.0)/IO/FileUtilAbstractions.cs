using System.Globalization;
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
            return GetDisplayFileSize(file.Exists ? file.Size : -1L, minDecimalPlaces: minDecimalPlaces, maxDecimalPlaces: maxDecimalPlaces, addSeparators: addSeparators, unit: unit, bi: bi);
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
