using System.Collections.Generic;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace Horseshoe.NET.Crypto
{
    public class HashOptions
    {
        public HashAlgorithm Algorithm { get; set; }

        public string SaltText
        {
            set
            {
                Salt = byte.Parse(value, NumberStyles.HexNumber);
            }
        }

        public byte? Salt { get; set; }

        public Encoding Encoding { get; set; }

        public IEnumerable<string> FileFilter { get; set; }

        public bool FileFilterMode { get; set; }
    }
}
