using System.IO;
using System.Linq;

namespace Horseshoe.NET.IO
{
    public static class Extensions
    {
        public static bool IsEmpty(this DirectoryInfo dir)
        {
            return !(dir.GetDirectories().Any() || dir.GetFiles().Any());
        }
    }
}
