using System;
using System.IO;
using System.Security;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Horseshoe.NET.Db;
using Horseshoe.NET.IO;

namespace Horseshoe.NET.Crypto
{
    /// <summary>
    /// A collection of decryption methods
    /// </summary>
    public static class Decrypt
    {
        /// <summary>
        /// Decrypts encrypted bytes to plain bytes
        /// </summary>
        /// <param name="bytes">Encrypted <c>byte[]</c></param>
        /// <param name="options">Optional options for the encode/decode and encrypt/decrypt steps of these methods</param>
        /// <returns>plain <c>byte[]</c></returns>
        public static byte[] Bytes(byte[] bytes, CryptoOptions options = null)
        {
            options = options ?? new CryptoOptions();

            var algorithm = options.UseEmbeddedKIV
                ? CryptoUtil.BuildSymmetricAlgorithmForDecryptionEmbeddedKIV(options, ref bytes)
                : CryptoUtil.BuildSymmetricAlgorithm(options);

            // decrypt
            using (var memoryStream = new MemoryStream(bytes))
            {
                var cryptoTransform = algorithm.CreateDecryptor();
                using (var cryptoStream = new CryptoStream(memoryStream, cryptoTransform, CryptoStreamMode.Read))
                {
                    return cryptoStream.ReadAllBytes();
                }
            }
        }

        /// <summary>
        /// Decrypts encrypted <c>string</c> to plain bytes
        /// </summary>
        /// <param name="ciphertext">Encrypted <c>string</c></param>
        /// <param name="options">Optional options for the encode/decode and encrypt/decrypt steps of these methods</param>
        /// <returns>plain <c>byte[]</c></returns>
        public static byte[] Bytes(string ciphertext, CryptoOptions options = null)
        {
            byte[] cipherBytes = CryptoUtil.DecodeCipherbytes(ciphertext, options);
            return Bytes(cipherBytes, options);
        }

        /// <summary>
        /// Decrypts encrypted <c>Stream</c> to plain bytes
        /// </summary>
        /// <param name="inputStream">Encrypted <c>byte</c> stream</param>
        /// <param name="options">Optional options for the encode/decode and encrypt/decrypt steps of these methods</param>
        /// <returns>plain <c>byte[]</c></returns>
        public static byte[] Bytes(Stream inputStream, CryptoOptions options = null)
        {
            var cipherBytes = inputStream.ReadAllBytes();
            return Bytes(cipherBytes, options);
        }

        /// <summary>
        /// Decrypts encrypted <c>Stream</c> to plain bytes
        /// </summary>
        /// <param name="inputStream">Encrypted <c>byte</c> stream</param>
        /// <param name="options">Optional options for the encode/decode and encrypt/decrypt steps of these methods</param>
        /// <returns>plain <c>byte[]</c></returns>
        public static async Task<byte[]> BytesAsync(Stream inputStream, CryptoOptions options = null)
        {
            var cipherBytes = await inputStream.ReadAllBytesAsync();
            return Bytes(cipherBytes, options);
        }

        /// <summary>
        /// Decrypts encrypted file to plain bytes
        /// </summary>
        /// <param name="file">Encrypted file</param>
        /// <param name="options">Optional options for the encode/decode and encrypt/decrypt steps of these methods</param>
        /// <returns>plain <c>byte[]</c></returns>
        public static byte[] Bytes(FilePath file, CryptoOptions options = null)
        {
            using (var stream = file.OpenRead())
            {
                return Bytes(stream, options);
            }
        }

        /// <summary>
        /// Decrypts encrypted file to plain bytes
        /// </summary>
        /// <param name="file">Encrypted file</param>
        /// <param name="options">Optional options for the encode/decode and encrypt/decrypt steps of these methods</param>
        /// <returns>plain <c>byte[]</c></returns>
        public static async Task<byte[]> BytesAsync(FilePath file, CryptoOptions options = null)
        {
            using (var stream = file.OpenRead())
            {
                return await BytesAsync(stream, options);
            }
        }

        /// <summary>
        /// Decrypts encrypted bytes to plaintext <c>string</c>
        /// </summary>
        /// <param name="bytes">Encrypted <b>byte[]</b></param>
        /// <param name="options">Optional options for the encode/decode and encrypt/decrypt steps of these methods</param>
        /// <returns>plaintext</returns>
        public static string String(byte[] bytes, CryptoOptions options = null)
        {
            var plainBytes = Bytes(bytes, options: options);
            var plainText = (options?.Encoding ?? CryptoSettings.DefaultEncoding).GetString(plainBytes);
            return plainText;
        }

        /// <summary>
        /// Decrypts encrypted <c>string</c> to plaintext <c>string</c>
        /// </summary>
        /// <param name="ciphertext">Encrypted <c>string</c></param>
        /// <param name="options">Optional options for the encode/decode and encrypt/decrypt steps of these methods</param>
        /// <returns>plaintext</returns>
        public static string String(string ciphertext, CryptoOptions options = null)
        {
            var plainBytes = Bytes(ciphertext, options: options);
            var plainText = (options?.Encoding ?? CryptoSettings.DefaultEncoding).GetString(plainBytes);
            return plainText;
        }

        /// <summary>
        /// Decrypts encrypted <c>Stream</c> to plaintext <c>string</c>
        /// </summary>
        /// <param name="inputStream">Encrypted <c>Stream</c></param>
        /// <param name="options">Optional options for the encode/decode and encrypt/decrypt steps of these methods</param>
        /// <returns>plaintext</returns>
        public static string String(Stream inputStream, CryptoOptions options = null)
        {
            var plainBytes = Bytes(inputStream, options: options);
            var plainText = (options?.Encoding ?? CryptoSettings.DefaultEncoding).GetString(plainBytes);
            return plainText;
        }

        /// <summary>
        /// Decrypts encrypted <c>Stream</c> to plaintext <c>string</c>
        /// </summary>
        /// <param name="inputStream">Encrypted <c>Stream</c></param>
        /// <param name="options">Optional options for the encode/decode and encrypt/decrypt steps of these methods</param>
        /// <returns>plaintext</returns>
        public static async Task<string> StringAsync(Stream inputStream, CryptoOptions options = null)
        {
            var plainBytes = await BytesAsync(inputStream, options: options);
            var plainText = (options?.Encoding ?? CryptoSettings.DefaultEncoding).GetString(plainBytes);
            return plainText;
        }

        /// <summary>
        /// Decrypts encrypted file to plaintext <c>string</c>
        /// </summary>
        /// <param name="file">Encrypted file</param>
        /// <param name="options">Optional options for the encode/decode and encrypt/decrypt steps of these methods</param>
        /// <returns>plaintext</returns>
        public static string String(FilePath file, CryptoOptions options = null)
        {
            using (var stream = file.OpenRead())
            {
                return String(stream, options);
            }
        }

        /// <summary>
        /// Decrypts encrypted file to plaintext <c>string</c>
        /// </summary>
        /// <param name="file">Encrypted file</param>
        /// <param name="options">Optional options for the encode/decode and encrypt/decrypt steps of these methods</param>
        /// <returns>plaintext</returns>
        public static async Task<string> StringAsync(FilePath file, CryptoOptions options = null)
        {
            using (var stream = file.OpenRead())
            {
                return await StringAsync(stream, options);
            }
        }

        /// <summary>
        /// Decrypts encrypted bytes to a <c>SecureString</c>
        /// </summary>
        /// <param name="cipherBytes">Encrypted <c>byte[]</c></param>
        /// <param name="options">Optional options for the encode/decode and encrypt/decrypt steps of these methods</param>
        /// <returns>a <c>SecureString</c></returns>
        public static SecureString SecureString(byte[] cipherBytes, CryptoOptions options = null)
        {
            var secureString = new SecureString();
            var plainBytes = Bytes(cipherBytes, options: options);
            var plainChars = (options?.Encoding ?? CryptoSettings.DefaultEncoding).GetChars(plainBytes);
            plainBytes.SecureClear();
            foreach (char c in plainChars)
            {
                secureString.AppendChar(c);
            }
            plainChars.SecureClear();
            secureString.MakeReadOnly();
            return secureString;
        }

        /// <summary>
        /// Decrypts encrypted <c>string</c> to a <c>SecureString</c>
        /// </summary>
        /// <param name="ciphertext">Encrypted <c>string</c></param>
        /// <param name="options">Optional options for the encode/decode and encrypt/decrypt steps of these methods</param>
        /// <returns>a <c>SecureString</c></returns>
        public static SecureString SecureString(string ciphertext, CryptoOptions options = null)
        {
            byte[] cipherBytes = CryptoUtil.DecodeCipherbytes(ciphertext, options);
            return SecureString(cipherBytes, options: options);
        }

        /// <summary>
        /// Decrypts encrypted <c>Stream</c> to a <c>SecureString</c>
        /// </summary>
        /// <param name="inputStream">Encrypted <c>Stream</c></param>
        /// <param name="options">Optional options for the encode/decode and encrypt/decrypt steps of these methods</param>
        /// <returns>a <c>SecureString</c></returns>
        public static SecureString SecureString(Stream inputStream, CryptoOptions options = null)
        {
            var cipherBytes = inputStream.ReadAllBytes();
            return SecureString(cipherBytes, options: options);
        }

        /// <summary>
        /// Decrypts encrypted <c>Stream</c> to a <c>SecureString</c>
        /// </summary>
        /// <param name="inputStream">Encrypted <c>Stream</c></param>
        /// <param name="options">Optional options for the encode/decode and encrypt/decrypt steps of these methods</param>
        /// <returns>a <c>SecureString</c></returns>
        public static async Task<SecureString> SecureStringAsync(Stream inputStream, CryptoOptions options = null)
        {
            var cipherBytes = await inputStream.ReadAllBytesAsync();
            return SecureString(cipherBytes, options: options);
        }

        /// <summary>
        /// Decrypts encrypted file to a <c>SecureString</c>
        /// </summary>
        /// <param name="file">Encrypted file</param>
        /// <param name="options">Optional options for the encode/decode and encrypt/decrypt steps of these methods</param>
        /// <returns>a <c>SecureString</c></returns>
        public static SecureString SecureString(FilePath file, CryptoOptions options = null)
        {
            using (var stream = file.OpenRead())
            {
                return SecureString(stream, options: options);
            }
        }

        /// <summary>
        /// Decrypts encrypted file to a <c>SecureString</c>
        /// </summary>
        /// <param name="file">Encrypted file</param>
        /// <param name="options">Optional options for the encode/decode and encrypt/decrypt steps of these methods</param>
        /// <returns>a <c>SecureString</c></returns>
        public static async Task<SecureString> SecureStringAsync(FilePath file, CryptoOptions options = null)
        {
            using (var stream = file.OpenRead())
            {
                return await SecureStringAsync(stream, options: options);
            }
        }
    }
}
