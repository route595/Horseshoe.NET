using System.IO;

namespace Horseshoe.NET.IO
{
    /// <summary>
    /// Just a memory stream with a name, useful for associating with files
    /// </summary>
    public class NamedMemoryStream : MemoryStream
    {
        /// <summary>
        /// A name, such as a file name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Creates a new <c>NamedMemoryStream</c>.
        /// </summary>
        /// <param name="name"></param>
        public NamedMemoryStream(string name)
        {
            Name = name;
        }
    }
}
