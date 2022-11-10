using System;
using System.IO;
using System.Linq;
using System.Security;
using System.Security.Cryptography;

using Horseshoe.NET.Objects;
using Horseshoe.NET.Text;
using Horseshoe.NET.Text.TextClean;

namespace Horseshoe.NET.Crypto
{
    public static class Decrypt
    {
        public static byte[] Bytes(byte[] cipherBytes, CryptoOptions options = null)
        {
            options = options ?? new CryptoOptions();

            var algorithm = options.UseEmbeddedKIV
                ? CryptoUtil.BuildSymmetricAlgorithmForDecryptionEmbeddedKIV(options, ref cipherBytes)
                : CryptoUtil.BuildSymmetricAlgorithm(options);
            var plainBytes = new byte[cipherBytes.Length];

            // decrypt
            using (var memoryStream = new MemoryStream(cipherBytes))
            {
                var cryptoTransform = algorithm.CreateDecryptor();
                using (var cryptoStream = new CryptoStream(memoryStream, cryptoTransform, CryptoStreamMode.Read))
                {
                    cryptoStream.Read(plainBytes, 0, plainBytes.Length);
                }
            }

            return plainBytes;
        }

        public static void BytesToStream(byte[] cipherBytes, Stream outputStream, CryptoOptions options = null)
        {
            var plainBytes = Bytes(cipherBytes, options: options);
            outputStream.Write(plainBytes, 0, plainBytes.Length);
            outputStream.Flush();
            outputStream.Close();
        }

        public static string BytesToString(byte[] cipherBytes, CryptoOptions options = null)
        {
            var plainBytes = Bytes(cipherBytes, options: options);
            var plainText = (options?.Encoding ?? CryptoSettings.DefaultEncoding).GetString(plainBytes);
            return TextClean.Remove(plainText.Trim(), CharLib.AllNonprintables);
        }

        public static SecureString BytesToSecureString(byte[] cipherBytes, CryptoOptions options = null)
        {
            var secureString = new SecureString();
            var plainBytes = Bytes(cipherBytes, options: options);
            var plainChars = (options?.Encoding ?? CryptoSettings.DefaultEncoding).GetChars(plainBytes);
            plainBytes.Clear();
            foreach (char c in plainChars)
            {
                if (CharLib.AllNonprintables.Contains(c)) continue;
                secureString.AppendChar(c);
            }
            plainChars.Clear();
            secureString.MakeReadOnly();
            return secureString;
        }

        public static void Stream(Stream inputStream, Stream outputStream, CryptoOptions options = null)
        {
            var plainBytes = StreamToBytes(inputStream, options: options);
            outputStream.Write(plainBytes, 0, plainBytes.Length);
            outputStream.Flush();
            outputStream.Close();
        }

        public static byte[] StreamToBytes(Stream inputStream, CryptoOptions options = null)
        {
            var cipherBytes = new byte[inputStream.Length];
            inputStream.Read(cipherBytes, 0, cipherBytes.Length);
            inputStream.Close();
            var plainBytes = Bytes(cipherBytes, options: options);
            return plainBytes;
        }

        public static string String(string cipherText, CryptoOptions options = null)
        {
            var cipherBytes = Convert.FromBase64String(cipherText);
            return BytesToString(cipherBytes, options: options);
        }

        public static SecureString SecureString(string cipherText, CryptoOptions options = null)
        {
            var cipherBytes = Convert.FromBase64String(cipherText);
            return BytesToSecureString(cipherBytes, options: options);
        }

        public static byte[] StringToBytes(string cipherText, CryptoOptions options = null)
        {
            var cipherBytes = Convert.FromBase64String(cipherText);
            var plainBytes = Bytes(cipherBytes, options: options);
            return plainBytes;
        }

        public static byte[] FileToBytes(FileInfo src, CryptoOptions options = null)
        {
            return StreamToBytes(src.OpenRead(), options: options);
        }

        public static byte[] FileToBytes(string srcPath, CryptoOptions options = null)
        {
            return StreamToBytes(File.OpenRead(srcPath), options: options);
        }

        public static FileInfo FileToFile(FileInfo src, FileInfo dest, CryptoOptions options = null)
        {
            FileToStream(src, dest.OpenWrite(), options: options);
            return dest;
        }

        public static FileInfo FileToFile(FileInfo src, string destPath, CryptoOptions options = null)
        {
            var dest = new FileInfo(destPath);
            FileToStream(src, dest.OpenWrite(), options: options);
            return dest;
        }

        public static FileInfo FileToFile(string srcPath, FileInfo dest, CryptoOptions options = null)
        {
            FileToStream(new FileInfo(srcPath), dest.OpenWrite(), options: options);
            return dest;
        }

        public static FileInfo FileToFile(string srcPath, string destPath, CryptoOptions options = null)
        {
            var dest = new FileInfo(destPath);
            FileToStream(new FileInfo(srcPath), dest.OpenWrite(), options: options);
            return dest;
        }

        public static void FileToStream(FileInfo src, Stream outputStream, CryptoOptions options = null)
        {
            Stream(src.OpenRead(), outputStream, options: options);
        }

        public static void FileToStream(string srcPath, Stream outputStream, CryptoOptions options = null)
        {
            Stream(File.OpenRead(srcPath), outputStream, options: options);
        }
    }
}
