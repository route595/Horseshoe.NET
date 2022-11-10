using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

using Horseshoe.NET.Text;

namespace Horseshoe.NET.IO
{
    public static class FileUtil
    {
        private static Regex FullPathIndicator { get; } = new Regex(@"^(\/|[A-Z]:\\|\\\\|[a-z]+:\/\/).*", RegexOptions.IgnoreCase);

        public static string AppendExtension(string fileName, FileType extension, bool preferUpperCase = false)
        {
            return AppendExtension(fileName, preferUpperCase ? extension.ToString() : extension.ToString().ToLower());
        }

        public static string AppendExtension(string fileName, string extension)
        {
            if (!extension.StartsWith("."))
            {
                extension = "." + extension;
            }
            return TextUtil.AppendIf(!fileName.ToLower().EndsWith(extension.ToLower()), fileName, extension);
        }

        public static string Create(byte[] contents, string targetDirectory = null, string targetFileName = null, FileType? fileType = null)
        {
            var targetPath = Path.Combine
            (
                targetDirectory ?? Path.GetTempPath(),
                targetFileName ?? "file" + (fileType.HasValue ? "." + fileType.ToString().ToLower() : "")
            );
            File.WriteAllBytes(targetPath, contents);
            return targetPath;
        }

        public static string Create(string contents, string targetDirectory = null, string targetFileName = null, FileType? fileType = null, Encoding encoding = null)
        {
            var bytes = (encoding ?? Encoding.Default).GetBytes(contents);
            return Create(bytes, targetDirectory: targetDirectory, targetFileName: targetFileName, fileType: fileType);
        }

        public static string GetDisplayFileSize(FilePath file, int? minDecimalPlaces = null, int? maxDecimalPlaces = null, bool addSeparators = true, FileSize.Unit? unit = null, bool? bi = null)
        {
            return GetDisplayFileSize(file.Exists ? file.Length : -1L, minDecimalPlaces: minDecimalPlaces, maxDecimalPlaces: maxDecimalPlaces, addSeparators: addSeparators, unit: unit, bi: bi);
        }

        public static string GetDisplayFileSize(long size, int? minDecimalPlaces = null, int? maxDecimalPlaces = null, bool addSeparators = true, FileSize.Unit? unit = null, bool? bi = null)
        {
            if (size < 0L) return size + (unit.HasValue ? " " + unit : "");
            if (size == 0L) return size + " " + (unit ?? FileSize.Unit.B);

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

            if (unit.HasValue && bi.HasValue)
            {
                if (bi.Value && !unit.Value.IsBI())
                {
                    throw new ValidationException(unit + " is a non-'bi' unit");
                }
                if (!bi.Value && unit.Value.IsBI())
                {
                    throw new ValidationException(unit + " is a 'bi' unit");
                }
            }

            if (!unit.HasValue)
            {
                unit = FileSize.DeriveUnit(size, bi ?? false);
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

        public static DirectoryInfo ParseDirectory(string path)
        {
            if (path == null) throw new ArgumentNullException(nameof(path));
            path = path.Trim();
            if (path.Length == 0) throw new Exception("path cannot be blank");
            return new DirectoryInfo(path);
        }

        static readonly string LinuxRootDirectory = "/";
        static readonly Regex WindowRootDirectoryPattern = new Regex(@"^[A-Z]\:\\$", RegexOptions.IgnoreCase);

        /// <summary>
        /// Determines whether two directories are on the same local drive for easier file moving.
        /// </summary>
        /// <returns>true if both the same Windows root (e.g. C:\) or Linux root (e.g. /), false otherwise</returns>
        public static bool IsOnSameLocalDrive(DirectoryInfo dir1, DirectoryInfo dir2)
        {
            if (dir1.Root.Equals(dir2.Root))
            {
                switch (Environment.OSVersion.Platform)
                {
                    case PlatformID.Win32NT:
                        return WindowRootDirectoryPattern.IsMatch(dir1.Root.FullName);
                    case PlatformID.Unix:
                    case PlatformID.MacOSX:
                        return dir1.Root.FullName.Equals(LinuxRootDirectory);
                    default:
                        throw new PlatformNotSupportedException(Environment.OSVersion.Platform + " is not supported");
                }
            }
            return false;
        }
    }
}
