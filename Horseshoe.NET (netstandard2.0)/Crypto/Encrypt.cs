using System;
using System.IO;
using System.Security.Cryptography;

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
        /// Encrypts plaintext bytes
        /// </summary>
        /// <param name="plainBytes">A <c>byte[]</c> to encrypt</param>
        /// <param name="options">Optional options for the encode/decode and encrypt/decrypt steps of these methods</param>
        /// <returns>An encrypted <c>byte[]</c></returns>
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
        /// Encrypts a plaintext <c>string</c> to a <c>byte[]</c>
        /// </summary>
        /// <param name="plaintext">A <c>string</c> to encrypt</param>
        /// <param name="options">Optional options for the encode/decode and encrypt/decrypt steps of these methods</param>
        /// <returns>An encrypted <c>byte[]</c></returns>
        public static byte[] Bytes(string plaintext, CryptoOptions options = null)
        {
            var plainBytes = (options?.Encoding ?? CryptoSettings.DefaultEncoding).GetBytes(plaintext);
            return Bytes(plainBytes, options: options);
        }

        /// <summary>
        /// Encrypts a plaintext <c>Stream</c> to a <c>byte[]</c>
        /// </summary>
        /// <param name="inputStream">A <c>Stream</c> to encrypt</param>
        /// <param name="options">Optional options for the encode/decode and encrypt/decrypt steps of these methods</param>
        /// <returns>An encrypted <c>byte[]</c></returns>
        public static byte[] Bytes(Stream inputStream, CryptoOptions options = null)
        {
            var plainBytes = inputStream.ReadAllBytes();
            inputStream.Read(plainBytes, 0, plainBytes.Length);
            return Bytes(plainBytes, options: options);
        }

        /// <summary>
        /// Encrypts a plaintext file to a <c>byte[]</c>
        /// </summary>
        /// <param name="file">A file to encrypt</param>
        /// <param name="options">Optional options for the encode/decode and encrypt/decrypt steps of these methods</param>
        /// <returns>An encrypted <c>byte[]</c></returns>
        public static byte[] Bytes(FilePath file, CryptoOptions options = null)
        {
            using (var stream = file.OpenRead())
            {
                return Bytes(stream, options: options);
            }
        }

        /// <summary>
        /// Encrypts a <c>string</c>
        /// </summary>
        /// <param name="plaintext">A <c>string</c> to encrypt</param>
        /// <param name="options">Optional options for the encode/decode and encrypt/decrypt steps of these methods</param>
        /// <returns>An encrypted <c>string</c></returns>
        public static string String(string plaintext, CryptoOptions options = null)
        {
            var cipherBytes = Bytes(plaintext, options: options);
            var ciphertext = CryptoUtil.EncodeCiphertext(cipherBytes, options);
            return ciphertext;
        }

        /// <summary>
        /// Encrypts a <c>byte[]</c> as a <c>string</c>
        /// </summary>
        /// <param name="plainBytes">A <c>byte[]</c> to encrypt</param>
        /// <param name="options">Optional options for the encode/decode and encrypt/decrypt steps of these methods</param>
        /// <returns>An encrypted <c>string</c></returns>
        public static string String(byte[] plainBytes, CryptoOptions options = null)
        {
            var cipherBytes = Bytes(plainBytes, options: options);
            var ciphertext = CryptoUtil.EncodeCiphertext(cipherBytes, options);
            return ciphertext;
        }

        /// <summary>
        /// Encrypts a <c>Stream</c> as a <c>string</c>
        /// </summary>
        /// <param name="inputStream">A <c>Stream</c> to encrypt</param>
        /// <param name="options">Optional options for the encode/decode and encrypt/decrypt steps of these methods</param>
        /// <returns>An encrypted <c>string</c></returns>
        public static string String(Stream inputStream, CryptoOptions options = null)
        {
            var cipherBytes = Bytes(inputStream, options: options);
            var ciphertext = CryptoUtil.EncodeCiphertext(cipherBytes, options);
            return ciphertext;
        }

        /// <summary>
        /// Encrypts a file as a <c>string</c>
        /// </summary>
        /// <param name="file">A file to encrypt</param>
        /// <param name="options">Optional options for the encode/decode and encrypt/decrypt steps of these methods</param>
        /// <returns>An encrypted <c>string</c></returns>
        public static string String(FilePath file, CryptoOptions options = null)
        {
            using (var stream = file.OpenRead())
            {
                return String(stream, options: options);
            }
        }
    }
}
