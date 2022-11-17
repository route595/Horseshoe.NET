using System.IO;
using System.Runtime.CompilerServices;
using System.Security;
using System.Security.Cryptography;
using System.Threading.Tasks;

using Horseshoe.NET.IO;

namespace Horseshoe.NET.Crypto
{
    /// <summary>
    /// A collection of decryption methods
    /// </summary>
    public static class Decrypt
    {
        /// <summary>
        /// Decrypts cipher bytes to plain bytes
        /// </summary>
        /// <param name="cipherBytes">source cipher <c>byte[]</c></param>
        /// <param name="options">crypto options</param>
        /// <returns>plain <c>byte[]</c></returns>
        public static byte[] Bytes(byte[] cipherBytes, CryptoOptions options = null)
        {
            options = options ?? new CryptoOptions();

            var algorithm = options.UseEmbeddedKIV
                ? CryptoUtil.BuildSymmetricAlgorithmForDecryptionEmbeddedKIV(options, ref cipherBytes)
                : CryptoUtil.BuildSymmetricAlgorithm(options);

            // decrypt
            using (var memoryStream = new MemoryStream(cipherBytes))
            {
                var cryptoTransform = algorithm.CreateDecryptor();
                using (var cryptoStream = new CryptoStream(memoryStream, cryptoTransform, CryptoStreamMode.Read))
                {
                    return cryptoStream.ReadAllBytes();
                }
            }
        }

        /// <summary>
        /// Decrypts cipher <c>string</c> to plain bytes
        /// </summary>
        /// <param name="ciphertext">source cipher <c>string</c></param>
        /// <param name="options"></param>
        /// <returns>plain <c>byte[]</c></returns>
        public static byte[] Bytes(string ciphertext, CryptoOptions options = null)
        {
            var cipherBytes = (options?.Encoding ?? CryptoSettings.DefaultEncoding).GetBytes(ciphertext);
            return Bytes(cipherBytes, options);
        }

        /// <summary>
        /// Decrypts cipher stream to plain bytes
        /// </summary>
        /// <param name="inputStreamOfEncryptedText">source encrypted stream</param>
        /// <param name="options">crypto options</param>
        /// <returns>plain <c>byte[]</c></returns>
        public static byte[] Bytes(Stream inputStreamOfEncryptedText, CryptoOptions options = null)
        {
            var cipherBytes = inputStreamOfEncryptedText.ReadAllBytes();
            return Bytes(cipherBytes, options);
        }

        /// <summary>
        /// Decrypts cipher stream to plain bytes
        /// </summary>
        /// <param name="inputStreamOfEncryptedText">source encrypted stream</param>
        /// <param name="options">crypto options</param>
        /// <returns>plain <c>byte[]</c></returns>
        public static async Task<byte[]> BytesAsync(Stream inputStreamOfEncryptedText, CryptoOptions options = null)
        {
            var cipherBytes = await inputStreamOfEncryptedText.ReadAllBytesAsync();
            return Bytes(cipherBytes, options);
        }

        /// <summary>
        /// Decrypts file to plain bytes
        /// </summary>
        /// <param name="file">source encrypted file</param>
        /// <param name="options">crypto options</param>
        /// <returns>plain <c>byte[]</c></returns>
        public static byte[] Bytes(FilePath file, CryptoOptions options = null)
        {
            using (var stream = file.OpenRead())
            {
                return Bytes(stream, options);
            }
        }

        /// <summary>
        /// Decrypts file to plain bytes
        /// </summary>
        /// <param name="file">source encrypted file</param>
        /// <param name="options">crypto options</param>
        /// <returns>plain <c>byte[]</c></returns>
        public static async Task<byte[]> BytesAsync(FilePath file, CryptoOptions options = null)
        {
            using (var stream = file.OpenRead())
            {
                return await BytesAsync(stream, options);
            }
        }

        /// <summary>
        /// Decrypts Base64 encoded cipher <c>string</c> to plain bytes
        /// </summary>
        /// <param name="base64EncodedCiphertext">source Base64 encoded cipher <c>string</c></param>
        /// <param name="options">crypto options</param>
        /// <returns>plain <c>byte[]</c></returns>
        public static byte[] BytesFromBase64(string base64EncodedCiphertext, CryptoOptions options = null)
        {
            var cipherBytes = Decode.Base64.Bytes(base64EncodedCiphertext);
            return Bytes(cipherBytes, options);
        }

        /// <summary>
        /// Decrypts Base64 encoded cipher stream to plain bytes
        /// </summary>
        /// <param name="inputStreamOfEncryptedText">source Base64 encoded encrypted stream</param>
        /// <param name="options">crypto options</param>
        /// <returns>plain <c>byte[]</c></returns>
        public static byte[] BytesFromBase64(Stream inputStreamOfEncryptedText, CryptoOptions options = null)
        {
            var encodedCipherBytes = inputStreamOfEncryptedText.ReadAllBytes();
            var cipherBytes = Decode.Base64.Bytes(encodedCipherBytes);
            return Bytes(cipherBytes, options);
        }

        /// <summary>
        /// Decrypts Base64 encoded cipher stream to plain bytes
        /// </summary>
        /// <param name="inputStreamOfEncryptedText">source Base64 encoded encrypted stream</param>
        /// <param name="options">crypto options</param>
        /// <returns>plain <c>byte[]</c></returns>
        public static async Task<byte[]> BytesFromBase64Async(Stream inputStreamOfEncryptedText, CryptoOptions options = null)
        {
            var encodedCipherBytes = await inputStreamOfEncryptedText.ReadAllBytesAsync();
            var cipherBytes = Decode.Base64.Bytes(encodedCipherBytes);
            return Bytes(cipherBytes, options);
        }

        /// <summary>
        /// Decrypts Base64 encoded encrypted file to plain bytes
        /// </summary>
        /// <param name="file">source Base64 encoded encrypted file</param>
        /// <param name="options">crypto options</param>
        /// <returns>plain <c>byte[]</c></returns>
        public static byte[] BytesFromBase64(FilePath file, CryptoOptions options = null)
        {
            using (var stream = file.OpenRead())
            {
                return BytesFromBase64(stream, options);
            }
        }

        /// <summary>
        /// Decrypts Base64 encoded encrypted file to plain bytes
        /// </summary>
        /// <param name="file">source Base64 encoded encrypted file</param>
        /// <param name="options">crypto options</param>
        /// <returns>plain <c>byte[]</c></returns>
        public static async Task<byte[]> BytesFromBase64Async(FilePath file, CryptoOptions options = null)
        {
            using (var stream = file.OpenRead())
            {
                return await BytesFromBase64Async(stream, options);
            }
        }

        //public static void BytesToStream(byte[] cipherBytes, Stream outputStream, CryptoOptions options = null)
        //{
        //    var plainBytes = Bytes(cipherBytes, options: options);
        //    outputStream.Write(plainBytes, 0, plainBytes.Length);
        //    outputStream.Flush();
        //    outputStream.Close();
        //}

        /// <summary>
        /// Decrypts cipher bytes to plaintext
        /// </summary>
        /// <param name="cipherBytes">source cipher <c>byte[]</c></param>
        /// <param name="options">crypto options</param>
        /// <returns>plaintext</returns>
        public static string String(byte[] cipherBytes, CryptoOptions options = null)
        {
            var plainBytes = Bytes(cipherBytes, options: options);
            var plainText = (options?.Encoding ?? CryptoSettings.DefaultEncoding).GetString(plainBytes);
            return plainText;
        }

        /// <summary>
        /// Decrypts ciphertext to plaintext
        /// </summary>
        /// <param name="ciphertext">source cipher <c>string</c></param>
        /// <param name="options">crypto options</param>
        /// <returns>plaintext</returns>
        public static string String(string ciphertext, CryptoOptions options = null)
        {
            var plainBytes = Bytes(ciphertext, options: options);
            var plainText = (options?.Encoding ?? CryptoSettings.DefaultEncoding).GetString(plainBytes);
            return plainText;
        }

        /// <summary>
        /// Decrypts cipher stream to plaintext
        /// </summary>
        /// <param name="inputStreamOfEncryptedText">source encrypted stream</param>
        /// <param name="options">crypto options</param>
        /// <returns>plaintext</returns>
        public static string String(Stream inputStreamOfEncryptedText, CryptoOptions options = null)
        {
            var plainBytes = Bytes(inputStreamOfEncryptedText, options: options);
            var plainText = (options?.Encoding ?? CryptoSettings.DefaultEncoding).GetString(plainBytes);
            return plainText;
        }

        /// <summary>
        /// Decrypts cipher stream to plaintext
        /// </summary>
        /// <param name="inputStreamOfEncryptedText">source encrypted stream</param>
        /// <param name="options">crypto options</param>
        /// <returns>plaintext</returns>
        public static async Task<string> StringAsync(Stream inputStreamOfEncryptedText, CryptoOptions options = null)
        {
            var plainBytes = await BytesAsync(inputStreamOfEncryptedText, options: options);
            var plainText = (options?.Encoding ?? CryptoSettings.DefaultEncoding).GetString(plainBytes);
            return plainText;
        }

        /// <summary>
        /// Decrypts file to plaintext
        /// </summary>
        /// <param name="file">source encrypted file</param>
        /// <param name="options">crypto options</param>
        /// <returns>plaintext</returns>
        public static string String(FilePath file, CryptoOptions options = null)
        {
            using (var stream = file.OpenRead())
            {
                return String(stream, options);
            }
        }

        /// <summary>
        /// Decrypts file to plaintext
        /// </summary>
        /// <param name="file">source encrypted file</param>
        /// <param name="options">crypto options</param>
        /// <returns>plaintext</returns>
        public static async Task<string> StringAsync(FilePath file, CryptoOptions options = null)
        {
            using (var stream = file.OpenRead())
            {
                return await StringAsync(stream, options);
            }
        }

        /// <summary>
        /// Decrypts Base64 encoded ciphertext to plaintext
        /// </summary>
        /// <param name="base64EncodedCiphertext">Base64 encoded source cipher <c>string</c></param>
        /// <param name="options">crypto options</param>
        /// <returns>plaintext</returns>
        public static string StringFromBase64(string base64EncodedCiphertext, CryptoOptions options = null)
        {
            var plainBytes = BytesFromBase64(base64EncodedCiphertext, options: options);
            var plainText = (options?.Encoding ?? CryptoSettings.DefaultEncoding).GetString(plainBytes);
            return plainText;
        }

        /// <summary>
        /// Decrypts Base64 encoded cipher stream to plaintext
        /// </summary>
        /// <param name="inputStreamOfBase64EncodedEncryptedText">Base64 encoded source encrypted stream</param>
        /// <param name="options">crypto options</param>
        /// <returns>plaintext</returns>
        public static string StringFromBase64(Stream inputStreamOfBase64EncodedEncryptedText, CryptoOptions options = null)
        {
            var plainBytes = BytesFromBase64(inputStreamOfBase64EncodedEncryptedText, options: options);
            var plainText = (options?.Encoding ?? CryptoSettings.DefaultEncoding).GetString(plainBytes);
            return plainText;
        }

        /// <summary>
        /// Decrypts Base64 encoded cipher stream to plaintext
        /// </summary>
        /// <param name="inputStreamOfBase64EncodedEncryptedText">Base64 encoded source encrypted stream</param>
        /// <param name="options">crypto options</param>
        /// <returns>plaintext</returns>
        public static async Task<string> StringFromBase64Async(Stream inputStreamOfBase64EncodedEncryptedText, CryptoOptions options = null)
        {
            var plainBytes = await BytesFromBase64Async(inputStreamOfBase64EncodedEncryptedText, options: options);
            var plainText = (options?.Encoding ?? CryptoSettings.DefaultEncoding).GetString(plainBytes);
            return plainText;
        }

        /// <summary>
        /// Decrypts a Base64 encoded encrypted file to plaintext
        /// </summary>
        /// <param name="file">Base64 encoded source encrypted file</param>
        /// <param name="options">crypto options</param>
        /// <returns>plaintext</returns>
        public static string StringFromBase64(FilePath file, CryptoOptions options = null)
        {
            using (var stream = file.OpenRead())
            {
                return StringFromBase64(stream, options);
            }
        }

        /// <summary>
        /// Decrypts a Base64 encoded encrypted file to plaintext
        /// </summary>
        /// <param name="file">Base64 encoded source encrypted file</param>
        /// <param name="options">crypto options</param>
        /// <returns>plaintext</returns>
        public static async Task<string> StringFromBase64Async(FilePath file, CryptoOptions options = null)
        {
            using (var stream = file.OpenRead())
            {
                return await StringFromBase64Async(stream, options);
            }
        }

        /// <summary>
        /// Decrypts cipher bytes to a secure string
        /// </summary>
        /// <param name="cipherBytes">source cipher <c>byte[]</c></param>
        /// <param name="options">crypto options</param>
        /// <returns>secure string</returns>
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
        /// Decrypts ciphertext to a secure string
        /// </summary>
        /// <param name="ciphertext">encrypted string</param>
        /// <param name="options">crypto options</param>
        /// <returns>secure string</returns>
        public static SecureString SecureString(string ciphertext, CryptoOptions options = null)
        {
            var cipherBytes = (options?.Encoding ?? CryptoSettings.DefaultEncoding).GetBytes(ciphertext);
            return SecureString(cipherBytes, options: options);
        }

        /// <summary>
        /// Decrypts a cipher stream to a secure string
        /// </summary>
        /// <param name="inputStreamOfEncryptedText">encrypted stream</param>
        /// <param name="options">crypto options</param>
        /// <returns>secure string</returns>
        public static SecureString SecureString(Stream inputStreamOfEncryptedText, CryptoOptions options = null)
        {
            var cipherBytes = inputStreamOfEncryptedText.ReadAllBytes();
            return SecureString(cipherBytes, options: options);
        }

        /// <summary>
        /// Decrypts a cipher stream to a secure string
        /// </summary>
        /// <param name="inputStreamOfEncryptedText">encrypted stream</param>
        /// <param name="options">crypto options</param>
        /// <returns>secure string</returns>
        public static async Task<SecureString> SecureStringAsync(Stream inputStreamOfEncryptedText, CryptoOptions options = null)
        {
            var cipherBytes = await inputStreamOfEncryptedText.ReadAllBytesAsync();
            return SecureString(cipherBytes, options: options);
        }

        /// <summary>
        /// Decrypts an encrypted file to a secure string
        /// </summary>
        /// <param name="file">encrypted file</param>
        /// <param name="options">crypto options</param>
        /// <returns>secure string</returns>
        public static SecureString SecureString(FilePath file, CryptoOptions options = null)
        {
            using (var stream = file.OpenRead())
            {
                return SecureString(stream, options: options);
            }
        }

        /// <summary>
        /// Decrypts an encrypted file to a secure string
        /// </summary>
        /// <param name="file">encrypted file</param>
        /// <param name="options">crypto options</param>
        /// <returns>secure string</returns>
        public static async Task<SecureString> SecureStringAsync(FilePath file, CryptoOptions options = null)
        {
            using (var stream = file.OpenRead())
            {
                return await SecureStringAsync(stream, options: options);
            }
        }

        /// <summary>
        /// Decrypts Base64 encoded ciphertext to a secure string
        /// </summary>
        /// <param name="base64EncodedCiphertext">Base64 encoded encrypted string</param>
        /// <param name="options">crypto options</param>
        /// <returns>secure string</returns>
        public static SecureString SecureStringFromBase64(string base64EncodedCiphertext, CryptoOptions options = null)
        {
            var cipherBytes = BytesFromBase64(base64EncodedCiphertext);
            return SecureString(cipherBytes, options: options);
        }

        /// <summary>
        /// Decrypts a Base64 encoded cipher stream to a secure string
        /// </summary>
        /// <param name="inputStreamOfBase64EncodedEncryptedText">Base64 encoded encrypted stream</param>
        /// <param name="options">crypto options</param>
        /// <returns>secure string</returns>
        public static SecureString SecureStringFromBase64(Stream inputStreamOfBase64EncodedEncryptedText, CryptoOptions options = null)
        {
            var cipherBytes = Decode.Base64.Bytes(inputStreamOfBase64EncodedEncryptedText.ReadAllBytes());
            return SecureString(cipherBytes, options: options);
        }

        /// <summary>
        /// Decrypts a Base64 encoded cipher stream to a secure string
        /// </summary>
        /// <param name="inputStreamOfBase64EncodedEncryptedText">Base64 encoded encrypted stream</param>
        /// <param name="options">crypto options</param>
        /// <returns>secure string</returns>
        public static async Task<SecureString> SecureStringFromBase64Async(Stream inputStreamOfBase64EncodedEncryptedText, CryptoOptions options = null)
        {
            var cipherBytes = Decode.Base64.Bytes(await inputStreamOfBase64EncodedEncryptedText.ReadAllBytesAsync());
            return SecureString(cipherBytes, options: options);
        }

        /// <summary>
        /// Decrypts a Base64 encoded encrypted file to a secure string
        /// </summary>
        /// <param name="file">encrypted file</param>
        /// <param name="options">crypto options</param>
        /// <returns>secure string</returns>
        public static SecureString SecureStringFromBase64(FilePath file, CryptoOptions options = null)
        {
            using (var stream = file.OpenRead())
            {
                return SecureStringFromBase64(stream, options: options);
            }
        }

        /// <summary>
        /// Decrypts a Base64 encoded encrypted file to a secure string
        /// </summary>
        /// <param name="file">encrypted file</param>
        /// <param name="options">crypto options</param>
        /// <returns>secure string</returns>
        public static async Task<SecureString> SecureStringFromBase64Async(FilePath file, CryptoOptions options = null)
        {
            using (var stream = file.OpenRead())
            {
                return await SecureStringFromBase64Async(stream, options: options);
            }
        }
    }
}
