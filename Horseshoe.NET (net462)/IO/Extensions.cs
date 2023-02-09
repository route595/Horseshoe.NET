using System.Collections.Generic;
using System.IO;
using System.Linq;
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
    }
}
