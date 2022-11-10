using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace Horseshoe.NET.Crypto
{
    public static class Encrypt
    {
        public static byte[] Bytes(byte[] plainBytes, CryptoOptions options = null)
        {
            options = options ?? new CryptoOptions();

            var algorithm = options.UseEmbeddedKIV
                ? CryptoUtil.BuildSymmetricAlgorithmForEncryptionEmbeddedKIV(options)
                : CryptoUtil.BuildSymmetricAlgorithm(options);

            MemoryStream memoryStream;

            // encrypt
            using (memoryStream = new MemoryStream())
            {
                var cryptoTransform = algorithm.CreateEncryptor();
                using (var cryptoStream = new CryptoStream(memoryStream, cryptoTransform, CryptoStreamMode.Write))
                {
                    cryptoStream.Write(plainBytes, 0, plainBytes.Length);
                    cryptoStream.FlushFinalBlock();
                }
            }
            var cipherBytes = memoryStream.ToArray();

            // append key and iv to encrypted output for future decrypting, if applicable
            if (options.UseEmbeddedKIV)
            {
                cipherBytes = cipherBytes
                    .Concat(algorithm.Key)
                    .Concat(algorithm.IV)
                    .ToArray();
            }
            return cipherBytes;
        }

        public static void BytesToStream(byte[] plainBytes, Stream outputStream, CryptoOptions options = null)
        {
            var cipherBytes = Bytes(plainBytes, options: options);
            outputStream.Write(cipherBytes, 0, cipherBytes.Length);
            outputStream.Flush();
            outputStream.Close();
        }

        public static string BytesToString(byte[] plainBytes, CryptoOptions options = null)
        {
            var cipherBytes = Bytes(plainBytes, options: options);
            var cipherText = Convert.ToBase64String(cipherBytes);
            return cipherText;
        }

        public static void Stream(Stream inputStream, Stream outputStream, CryptoOptions options = null)
        {
            var cipherBytes = StreamToBytes(inputStream, options: options);
            outputStream.Write(cipherBytes, 0, cipherBytes.Length);
            outputStream.Flush();
            outputStream.Close();
        }

        public static byte[] StreamToBytes(Stream inputStream, CryptoOptions options = null)
        {
            var plainBytes = new byte[inputStream.Length];
            inputStream.Read(plainBytes, 0, plainBytes.Length);
            inputStream.Close();
            var cipherBytes = Bytes(plainBytes, options: options);
            return cipherBytes;
        }

        public static string String(string plainText, CryptoOptions options = null)
        {
            var cipherBytes = StringToBytes(plainText, options: options);
            var cipherText = Convert.ToBase64String(cipherBytes);
            return cipherText;
        }

        public static byte[] StringToBytes(string plainText, CryptoOptions options = null)
        {
            var plainBytes = (options?.Encoding ?? CryptoSettings.DefaultEncoding).GetBytes(plainText);
            var cipherBytes = Bytes(plainBytes, options: options);
            return cipherBytes;
        }

        public static byte[] FileToBytes(FileInfo file, CryptoOptions options = null)
        {
            return StreamToBytes(file.OpenRead(), options: options);
        }

        public static byte[] FileToBytes(string path, CryptoOptions options = null)
        {
            return StreamToBytes(File.OpenRead(path), options: options);
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

        public static void FileToStream(FileInfo file, Stream outputStream, CryptoOptions options = null)
        {
            Stream(file.OpenRead(), outputStream, options: options);
        }

        public static void FileToStream(string path, Stream outputStream, CryptoOptions options = null)
        {
            Stream(File.OpenRead(path), outputStream, options: options);
        }
    }
}
