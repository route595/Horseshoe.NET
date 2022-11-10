using System.IO;

namespace Horseshoe.NET.IO
{
    public class NamedMemoryStream : MemoryStream
    {
        public string Name { get; set; }

        public NamedMemoryStream(string name)
        {
            Name = name;
        }
    }
}
