using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Horseshoe.NET.IO
{
    public static class Extensions
    {
        public static bool IsEmpty(this DirectoryInfo dir)
        {
            return !(dir.GetDirectories().Any() || dir.GetFiles().Any());
        }

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
    }
}
