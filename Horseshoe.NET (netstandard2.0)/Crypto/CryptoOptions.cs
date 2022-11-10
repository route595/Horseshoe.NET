using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace Horseshoe.NET.Crypto
{
    public class CryptoOptions
    {
        /// <summary>
        /// The desired encryption / decryption algorithm
        /// </summary>
        public SymmetricAlgorithm Algorithm { get; set; }

        /// <summary>
        /// The encryption key
        /// </summary>
        public byte[] Key { get; set; }

        /// <summary>
        /// The encryption key in text format
        /// </summary>
        public string KeyText
        {
            set
            {
                Key = (Encoding ?? CryptoSettings.DefaultEncoding).GetBytes(value);
            }
        }

        public bool AutoPadKey { get; set; }

        /// <summary>
        /// The encryption key
        /// </summary>
        public byte[] IV { get; set; }

        /// <summary>
        /// The encryption key in text format
        /// </summary>
        public string IVText
        {
            set
            {
                IV = (Encoding ?? CryptoSettings.DefaultEncoding).GetBytes(value);
            }
        }

        public bool AutoPopulateIVFromKey { get; set; }

        /// <summary>
        /// Block size in bits (e.g. 128 aka 16 bytes)
        /// </summary>
        public int? BlockSize { get; set; }

        /// <summary>
        /// See <c>System.Security.Cryptography.PaddingMode</c>.
        /// </summary>
        public CipherMode? Mode { get; set; }

        /// <summary>
        /// See <c>System.Security.Cryptography.PaddingMode</c>.
        /// </summary>
        public PaddingMode? Padding { get; set; }

        /// <summary>
        /// Generates a random key and IV and appends them to the encryption, then extracts them prior to decrypting 
        /// </summary>
        public bool UseEmbeddedKIV { get; set; }

        public Encoding Encoding { get; set; }
    }
}
