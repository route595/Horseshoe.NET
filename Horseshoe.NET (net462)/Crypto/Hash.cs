using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

using Horseshoe.NET.IO;

namespace Horseshoe.NET.Crypto
{
    public static class Hash
    {
        public static string String(byte[] plainBytes, HashOptions options = null)
        {
            options = options ?? new HashOptions();
            var sb = new StringBuilder();
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
                plainBytes = new[] { salt.Value }
                    .Concat(plainBytes)
                    .ToArray();
            }

            var cipherBytes = algorithm.ComputeHash(plainBytes);
            foreach (var b in cipherBytes)
            {
                sb.Append(b.ToString("x2"));
            }
            return sb.ToString();
        }

        public static string String(string plainText, HashOptions options = null)
        {
            options = options ?? new HashOptions();
            var encoding = options.Encoding ?? CryptoSettings.DefaultEncoding;
            var plainBytes = encoding.GetBytes(plainText);
            return String(plainBytes, options);
        }

        public static string String(Stream inputStream, HashOptions options = null)
        {
            options = options ?? new HashOptions();
            var plainBytes = new byte[inputStream.Length];
            inputStream.Read(plainBytes, 0, plainBytes.Length);
            inputStream.Close();
            return String(plainBytes, options);
        }

        public static string File(FilePath filePath, HashOptions options = null)
        {
            return String(filePath.OpenRead(), options);
        }
    }
}
