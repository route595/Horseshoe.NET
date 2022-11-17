using System.IO;
using System.Security.Cryptography;
using System.Text;

using Horseshoe.NET.Collections;
using Horseshoe.NET.IO;

namespace Horseshoe.NET.Crypto
{
    /// <summary>
    /// Factory methods for generating hashes
    /// </summary>
    public static class Hash
    {
        /// <summary>
        /// Generate hash bytes from source text bytes
        /// </summary>
        /// <param name="plainBytes">source text bytes</param>
        /// <param name="options">hash options</param>
        /// <returns>hash bytes</returns>
        public static byte[] Bytes(byte[] plainBytes, HashOptions options = null)
        {
            options = options ?? new HashOptions();
            HashAlgorithm algorithm;
            byte? salt;
            if (options.Algorithm != null)
            {
                algorithm = options.Algorithm;
                salt = options.Salt;
            }
            else
            {
                algorithm = CryptoSettings.DefaultHashAlgorithm;
                salt = CryptoSettings.DefaultHashSalt;
            }

            if (salt.HasValue)
            {
                plainBytes = ArrayUtil.Prepend(plainBytes, salt.Value);
            }

            var cipherBytes = algorithm.ComputeHash(plainBytes);
            return cipherBytes;
        }

        /// <summary>
        /// Generate hash from source text bytes
        /// </summary>
        /// <param name="plainBytes">source text bytes</param>
        /// <param name="options">hash options</param>
        /// <returns>hash</returns>
        public static string String(byte[] plainBytes, HashOptions options = null)
        {
            var sb = new StringBuilder();
            var cipherBytes = Bytes(plainBytes, options: options);
            foreach (var b in cipherBytes)
            {
                sb.Append(b.ToString("x2"));
            }
            return sb.ToString();
        }

        /// <summary>
        /// Generate hash from source text
        /// </summary>
        /// <param name="plainText">source text</param>
        /// <param name="options">hash options</param>
        /// <returns>hash</returns>
        public static string String(string plainText, HashOptions options = null)
        {
            options = options ?? new HashOptions();
            var encoding = options.Encoding ?? CryptoSettings.DefaultEncoding;
            var plainBytes = encoding.GetBytes(plainText);
            return String(plainBytes, options);
        }

        /// <summary>
        /// Generate hash from source text stream
        /// </summary>
        /// <param name="inputStream">source text stream</param>
        /// <param name="options">hash options</param>
        /// <returns>hash</returns>
        public static string String(Stream inputStream, HashOptions options = null)
        {
            options = options ?? new HashOptions();
            var plainBytes = new byte[inputStream.Length];
            inputStream.Read(plainBytes, 0, plainBytes.Length);
            inputStream.Close();
            return String(plainBytes, options);
        }

        /// <summary>
        /// Generate hash from source text file
        /// </summary>
        /// <param name="filePath">source text file</param>
        /// <param name="options">hash options</param>
        /// <returns>hash</returns>
        public static string File(FilePath filePath, HashOptions options = null)
        {
            return String(filePath.OpenRead(), options);
        }
    }
}
