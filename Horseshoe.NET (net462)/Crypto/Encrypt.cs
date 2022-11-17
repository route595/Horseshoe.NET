using System.IO;
using System.Security.Cryptography;
using System.Text;

using Horseshoe.NET.Collections;
using Horseshoe.NET.IO;

namespace Horseshoe.NET.Crypto
{
    /// <summary>
    /// A collection of encryption methods
    /// </summary>
    public static class Encrypt
    {
        /// <summary>
        /// Encrypts source bytes to cipher bytes
        /// </summary>
        /// <param name="plainBytes">source <c>byte[]</c></param>
        /// <param name="options">crypto options</param>
        /// <returns>cipher <c>byte[]</c></returns>
        public static byte[] Bytes(byte[] plainBytes, CryptoOptions options = null)
        {
            options = options ?? new CryptoOptions();

            var algorithm = options.UseEmbeddedKIV
                ? CryptoUtil.BuildSymmetricAlgorithmForEncryptionEmbeddedKIV(options)
                : CryptoUtil.BuildSymmetricAlgorithm(options);

            byte[] cipherBytes;

            // encrypt
            using (var memoryStream = new MemoryStream())
            {
                var cryptoTransform = algorithm.CreateEncryptor();
                using (var cryptoStream = new CryptoStream(memoryStream, cryptoTransform, CryptoStreamMode.Write))
                {
                    cryptoStream.Write(plainBytes, 0, plainBytes.Length);
                    cryptoStream.FlushFinalBlock();
                }
                cipherBytes  = memoryStream.ToArray();
            }

            // append key and iv to encrypted output for future decrypting, if applicable
            if (options.UseEmbeddedKIV)
            {
                cipherBytes = cipherBytes
                    .Append(algorithm.Key)
                    .Append(algorithm.IV);
            }
            return cipherBytes;
        }

        /// <summary>
        /// Encrypts source text to cipher bytes
        /// </summary>
        /// <param name="plaintext">source plaintext</param>
        /// <param name="options"></param>
        /// <returns>cipher <c>byte[]</c></returns>
        public static byte[] Bytes(string plaintext, CryptoOptions options = null)
        {
            var plainBytes = (options?.Encoding ?? CryptoSettings.DefaultEncoding).GetBytes(plaintext);
            return Bytes(plainBytes, options: options);
        }

        /// <summary>
        /// Encrypts source stream to cipher bytes
        /// </summary>
        /// <param name="inputStream">input stream</param>
        /// <param name="options">crypto options</param>
        /// <returns>cipher <c>byte[]</c></returns>
        public static byte[] Bytes(Stream inputStream, CryptoOptions options = null)
        {
            var plainBytes = inputStream.ReadAllBytes();
            inputStream.Read(plainBytes, 0, plainBytes.Length);
            return Bytes(plainBytes, options: options);
        }

        /// <summary>
        /// Encrypts source file contents to cipher bytes
        /// </summary>
        /// <param name="file">source file</param>
        /// <param name="options">crypto options</param>
        /// <returns>cipher <c>byte[]</c></returns>
        public static byte[] Bytes(FilePath file, CryptoOptions options = null)
        {
            using (var stream = file.OpenRead())
            {
                return Bytes(stream, options: options);
            }
        }

        //public static void BytesToStream(byte[] plainBytes, Stream outputStream, CryptoOptions options = null)
        //{
        //    var cipherBytes = Bytes(plainBytes, options: options);
        //    outputStream.Write(cipherBytes, 0, cipherBytes.Length);
        //    outputStream.Flush();
        //    outputStream.Close();
        //}

        /// <summary>
        /// Encrypt source <c>string</c> to cipher <c>string</c>
        /// </summary>
        /// <param name="plaintext">source plaintext</param>
        /// <param name="options">crypto options</param>
        /// <returns>ciphertext</returns>
        public static string String(string plaintext, CryptoOptions options = null)
        {
            var cipherBytes = Bytes(plaintext, options: options);
            var ciphertext = (options?.Encoding ?? CryptoSettings.DefaultEncoding).GetString(cipherBytes);
            return ciphertext;
        }

        /// <summary>
        /// Encrypt source bytes to <c>string</c>
        /// </summary>
        /// <param name="plainBytes">source byte[]</param>
        /// <param name="options">crypto options</param>
        /// <returns>ciphertext</returns>
        public static string String(byte[] plainBytes, CryptoOptions options = null)
        {
            var cipherBytes = Bytes(plainBytes, options: options);
            var ciphertext = (options?.Encoding ?? CryptoSettings.DefaultEncoding).GetString(cipherBytes);
            return ciphertext;
        }

        /// <summary>
        /// Encrypts source stream to cipher <c>string</c>
        /// </summary>
        /// <param name="inputStream">a stream</param>
        /// <param name="options">crypto options</param>
        /// <returns>ciphertext</returns>
        public static string String(Stream inputStream, CryptoOptions options = null)
        {
            var cipherBytes = Bytes(inputStream, options: options);
            var ciphertext = (options?.Encoding ?? CryptoSettings.DefaultEncoding).GetString(cipherBytes);
            return ciphertext;
        }

        /// <summary>
        /// Encrypts source file to cipher <c>string</c>
        /// </summary>
        /// <param name="file">a file</param>
        /// <param name="options">crypto options</param>
        /// <returns>ciphertext</returns>
        public static string String(FilePath file, CryptoOptions options = null)
        {
            using (var stream = file.OpenRead())
            {
                return String(stream, options: options);
            }
        }

        /// <summary>
        /// Encrypt source bytes to Base64 encoded cipher <c>string</c>
        /// </summary>
        /// <param name="plainBytes">source byte[]</param>
        /// <param name="options">crypto options</param>
        /// <returns></returns>
        public static string Base64String(byte[] plainBytes, CryptoOptions options = null)
        {
            var cipherBytes = Bytes(plainBytes, options: options);
            var ciphertext = Encode.Base64.String(cipherBytes);
            return ciphertext;
        }

        /// <summary>
        /// Encrypt source <c>string</c> to Base64 encoded cipher <c>string</c>
        /// </summary>
        /// <param name="plaintext">source plaintext</param>
        /// <param name="options">crypto options</param>
        /// <returns>ciphertext</returns>
        public static string Base64String(string plaintext, CryptoOptions options = null)
        {
            var cipherBytes = Bytes(plaintext, options: options);
            var ciphertext = Encode.Base64.String(cipherBytes);
            return ciphertext;
        }

        /// <summary>
        /// Encrypts source stream to Base64 encoded cipher <c>string</c>
        /// </summary>
        /// <param name="inputStream">a stream</param>
        /// <param name="options">crypto options</param>
        /// <returns>ciphertext</returns>
        public static string Base64String(Stream inputStream, CryptoOptions options = null)
        {
            var cipherBytes = Bytes(inputStream, options: options);
            var ciphertext = Encode.Base64.String(cipherBytes);
            return ciphertext;
        }

        /// <summary>
        /// Encrypts source file to Base64 encoded cipher <c>string</c>
        /// </summary>
        /// <param name="file">a file</param>
        /// <param name="options">crypto options</param>
        /// <returns>ciphertext</returns>
        public static string Base64String(FilePath file, CryptoOptions options = null)
        {
            using (var stream = file.OpenRead())
            {
                return Base64String(stream, options: options);
            }
        }
    }
}
