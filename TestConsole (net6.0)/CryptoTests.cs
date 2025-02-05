using System;
using System.Collections.Generic;
using System.Text;
using Horseshoe.NET;
using Horseshoe.NET.Collections;
using Horseshoe.NET.ConsoleX;
using Horseshoe.NET.Crypto;
using Horseshoe.NET.IO.FileTraversal;
using Horseshoe.NET.Text;
using TestConsole.IOTests;

namespace TestConsole
{
    class CryptoTests : RoutineX
    {
        private CryptoOptions Base64Options { get; } = new CryptoOptions { IsCiphertextBase64Encoded = true };

        public override IList<MenuObject> Menu => new[]
        {
            BuildMenuRoutine
            (
                "Hash (Default)",
                () =>
                {
                    var input = PromptX.Value<string>("Enter text to hash");
                    var hash = Hash.String(input);
                    Console.WriteLine(hash);
                }
            ),
            BuildMenuRoutine
            (
                "Hash (SHA256 + salt)",
                () =>
                {
                    var input2 = PromptX.Value<string>("Enter text to hash");
                    var hash2 = Hash.String(input2, new HashOptions { Algorithm = new System.Security.Cryptography.SHA256CryptoServiceProvider(), Salt = 112 });
                    Console.WriteLine(hash2);
                }
            ),
            BuildMenuRoutine
            (
                "Hash (Default => SHA256 + salt)",
                () =>
                {
                    CryptoSettings.DefaultHashAlgorithm = new System.Security.Cryptography.SHA256CryptoServiceProvider();
                    CryptoSettings.DefaultHashSalt = 112;
                    var input3 = PromptX.Value<string>("Enter text to hash");
                    var hash3 = Hash.String(input3);
                    Console.WriteLine(hash3);
                    CryptoSettings.DefaultHashAlgorithm = null;
                    CryptoSettings.DefaultHashSalt = null;
                }
            ),
            BuildMenuRoutine
            (
                "Hash File (MD5)",
                () =>
                {
                    var fileToHashPath = PromptX.Value<string>("Input file (or drag and drop)").Replace("\"", "");
                    var fileHash = Hash.String(fileToHashPath, new HashOptions { Algorithm = new System.Security.Cryptography.MD5CryptoServiceProvider() });
                    Console.WriteLine("Hash: " + fileHash);
                }
            ),
            BuildMenuRoutine
            (
                "Recursive hash entire directory \"animalia\"",
                () =>
                {
                    int dirLevel = 0;
                    FileTraversalTests.ResetFiles();
                    var cumulativeHash = RecursiveHash.String
                    (
                        "animalia",
                        out TraversalStatistics statistics,
                        onDirectoryHello: (dp, eng, _) => Console.WriteLine(new string(' ', dirLevel++ * 2) + dp.Name + "\\"),
                        onDirectoryGoodbye: (dp, eng, _) => dirLevel--,
                        onDirectorySkipped: (dp, eng) => {  Console.WriteLine(new string(' ', (dirLevel - 1) * 2) + "(skipping)"); },
                        onFileHello: (fp, eng, _) => Console.WriteLine(new string(' ', dirLevel * 2 + 2) + fp.Name + " (" + fp.GetDisplaySize() + ")"),
                        onFileSkipped: (fp, eng) => Console.WriteLine(new string(' ', dirLevel * 2) + fp.Name + " (skipped)"),
                        onFileHashed: (fp, hash) => Console.WriteLine(new string(' ', dirLevel * 2) + fp.Name + " " + hash)
                    );
                    Console.WriteLine("Cumulative Hash: " + cumulativeHash);
                    Console.Write(statistics.Dump());
                }
            ),
            BuildMenuRoutine
            (
                "Recursive hash mammal \".txt\" files in directory \"animalia\"",
                () =>
                {
                    int dirLevel = 0;
                    FileTraversalTests.ResetFiles();
                    var cumulativeHash = RecursiveHash.String
                    (
                        "animalia",
                        out TraversalStatistics statistics,
                        optimizations: new TraversalOptimizations { DirectoryFilter = (dp) => dp.Name.In("mammalia"), FileSearchPattern = "*.txt" },
                        onDirectoryHello: (dp, eng, _) => Console.WriteLine(new string(' ', dirLevel++ * 2) + dp.Name + "\\"),
                        onDirectoryGoodbye: (dp, eng, _) => dirLevel--,
                        onDirectorySkipped: (dp, eng) => {  Console.WriteLine(new string(' ', (dirLevel - 1) * 2) + "(skipping)"); },
                        onFileHello: (fp, eng, _) => Console.WriteLine(new string(' ', dirLevel * 2) + fp.Name + " (" + fp.GetDisplaySize() + ")"),
                        onFileSkipped: (fp, eng) => Console.WriteLine(new string(' ', dirLevel * 2) + fp.Name + " (skipped)"),
                        onFileHashed: (fp, hash) => Console.WriteLine(new string(' ', dirLevel * 2) + fp.Name + " " + hash)
                    );
                    Console.WriteLine("Cumulative Hash: " + cumulativeHash);
                    Console.Write(statistics.Dump());
                }
            ),
            BuildMenuRoutine
            (
                "Encrypt Password",
                () =>
                {
                    var passwordToEncrypt = PromptX.Value<string>("Enter password to encrypt");
                    var encryptedPassword = Encrypt.String(passwordToEncrypt);
                    Console.WriteLine("Encrypted password (len=" + encryptedPassword.Length + "): " + encryptedPassword);
                }
            ),
            BuildMenuRoutine
            (
                "Decrypt Password",
                () =>
                {
                    var stringToDecrypt = PromptX.Value<string>("Enter password to decrypt");
                    var decryptedPassword = Decrypt.String(stringToDecrypt);
                    Console.WriteLine("Decrypted password: " + decryptedPassword);
                }
            ),
            BuildMenuRoutine
            (
                "Encrypt/Decrypt Password",
                () =>
                {
                    var passwordToEncrypt = PromptX.Value<string>("Enter password to encrypt/decrypt");
                    var encryptedBytes = Encrypt.Bytes(passwordToEncrypt);
                    var encryptedPassword = Encoding.UTF8.GetString(encryptedBytes);
                    Console.WriteLine("Encrypted password (bytes=" + encryptedBytes.Length + "; len=" + encryptedPassword.Length + "): " + encryptedPassword);
                    var decryptedPassword = Decrypt.String(encryptedBytes);
                    Console.WriteLine("Decrypted password: " + decryptedPassword);
                }
            ),
            BuildMenuRoutine
            (
                "Encrypt Password (emits Base64)",
                () =>
                {
                    var passwordToEncrypt = PromptX.Value<string>("Enter password to encrypt");
                    var encryptedPassword = Encrypt.String(passwordToEncrypt, options: Base64Options);
                    Console.WriteLine("Encrypted password (len=" + encryptedPassword.Length + "): " + encryptedPassword);
                }
            ),
            BuildMenuRoutine
            (
                "Decrypt Password (expects Base64)",
                () =>
                {
                    var bytesToDecrypt = Decode.Base64.Bytes(PromptX.Value<string>("Enter password to decrypt"));
                    var decryptedPassword = Decrypt.String(bytesToDecrypt, options : Base64Options);
                    Console.WriteLine("Decrypted password: " + decryptedPassword);
                }
            ),
            BuildMenuRoutine
            (
                "Encrypt 3 Passwords (random key, IV)",
                () =>
                {
                    for (int i = 0; i < 3; i++)
                    {
                        var algorithm = new System.Security.Cryptography.RijndaelManaged();
                        var ciphertext = Encrypt.String("This is your life!", new CryptoOptions { Algorithm = algorithm });
                        Console.WriteLine("This is your life!" + " -> " + TextUtil.Crop(ciphertext, 26, truncateMarker: TruncateMarker.LongEllipsis) + " -- Key: " + TextUtil.Crop(string.Join(", ", algorithm.Key), 26, truncateMarker: TruncateMarker.LongEllipsis) + " -- IV: " + TextUtil.Crop(string.Join(", ", algorithm.IV), 26, truncateMarker: TruncateMarker.LongEllipsis));
                    }
                }
            ),
            BuildMenuRoutine
            (
                "Encrypt 3 Passwords (same key random IV)",
                () =>
                {
                    for (int i = 0; i < 3; i++)
                    {
                        var algorithm = new System.Security.Cryptography.RijndaelManaged { Key = ArrayUtil.Pad(null, 32, padWith: (byte)32) };
                        var ciphertext = Encrypt.String("This is your life!", new CryptoOptions { Algorithm = algorithm });
                        Console.WriteLine("This is your life!" + " -> " + TextUtil.Crop(ciphertext, 26, truncateMarker: TruncateMarker.LongEllipsis) + " -- Key: " + TextUtil.Crop(string.Join(", ", algorithm.Key), 26, truncateMarker: TruncateMarker.LongEllipsis) + " -- IV: " + TextUtil.Crop(string.Join(", ", algorithm.IV), 26, truncateMarker: TruncateMarker.LongEllipsis));
                    }
                }
            ),
            BuildMenuRoutine
            (
                "Encrypt 3 Passwords (same key, IV)",
                () =>
                {
                    for (int i = 0; i < 3; i++)
                    {
                        var algorithm = new System.Security.Cryptography.RijndaelManaged { Key = ArrayUtil.Pad(null, 32, padWith: (byte)32), IV = ArrayUtil.Pad(null, 16, padWith: (byte)16) };
                        var ciphertext = Encrypt.String("This is your life!", new CryptoOptions { Algorithm = algorithm });
                        Console.WriteLine("This is your life!" + " -> " + TextUtil.Crop(ciphertext, 26, truncateMarker: TruncateMarker.LongEllipsis) + " -- Key: " + TextUtil.Crop(string.Join(", ", algorithm.Key), 26, truncateMarker: TruncateMarker.LongEllipsis) + " -- IV: " + TextUtil.Crop(string.Join(", ", algorithm.IV), 26, truncateMarker: TruncateMarker.LongEllipsis));
                    }
                }
            ),
            BuildMenuRoutine
            (
                "Base64 Encode",
                () =>
                {
                    var plaintext = PromptX.Value<string>("Plaintext");
                    var base64 = Encode.Base64.String(plaintext);
                    Console.WriteLine("Encoded: " + base64);
                }
            ),
            BuildMenuRoutine
            (
                "Base64 Decode",
                () =>
                {
                    var base64 = PromptX.Value<string>("Encoded");
                    var plaintext = Decode.Base64.String(base64);
                    Console.WriteLine("Plaintext: " + plaintext);
                }
            )
        };
    }
}
