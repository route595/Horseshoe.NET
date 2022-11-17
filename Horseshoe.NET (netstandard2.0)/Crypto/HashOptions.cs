using System.Collections.Generic;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace Horseshoe.NET.Crypto
{
    /// <summary>
    /// Configure the hashing mechanism here
    /// </summary>
    public class HashOptions
    {
        /// <summary>
        /// the hashing algorithm
        /// </summary>
        public HashAlgorithm Algorithm { get; set; }

        /// <summary>
        /// Write-only preoperty accepting a hex string representing a single <c>byte</c> to be used as the hash salt
        /// </summary>
        /// <seealso cref="Salt"/>
        public string SaltText
        {
            set
            {
                Salt = byte.Parse(value, NumberStyles.HexNumber);
            }
        }

        /// <summary>
        /// The hash salt
        /// </summary>
        public byte? Salt { get; set; }

        /// <summary>
        /// The character encoding to use for text-to-byte conversions
        /// </summary>
        public Encoding Encoding { get; set; }
    }
}
