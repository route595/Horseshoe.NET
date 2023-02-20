using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

using Horseshoe.NET.Collections;

namespace Horseshoe.NET.Crypto
{
    /// <summary>
    /// A collection of utility methods for cryptographic operations in Horseshoe.NET
    /// </summary>
    public static class CryptoUtil
    {
        /// <summary>
        /// Builds a symmetric encryption algorithm for a KIV that is embedded in the final result.
        /// </summary>
        /// <param name="options">A <c>CryptoOptions</c> instance.</param>
        /// <returns></returns>
        public static SymmetricAlgorithm BuildSymmetricAlgorithmForEncryptionEmbeddedKIV(CryptoOptions options)
        {
            var algorithm = BuildSymmetricAlgorithm(options);

            algorithm.GenerateKey();
            algorithm.GenerateIV();

            return algorithm;
        }

        /// <summary>
        /// Builds a symmetric decryption algorithm for a KIV that is embedded in the final result.
        /// </summary>
        /// <param name="options">A <c>CryptoOptions</c> instance.</param>
        /// <param name="cipherBytes">The data from which to extract the KIV decrypt the rest.</param>
        /// <returns></returns>
        public static SymmetricAlgorithm BuildSymmetricAlgorithmForDecryptionEmbeddedKIV(CryptoOptions options, ref byte[] cipherBytes)
        {
            var algorithm = BuildSymmetricAlgorithm(options);

            var keyLength = algorithm.KeySize / 8;
            var ivLength = algorithm.BlockSize / 8;

            algorithm.IV = ArrayUtil.ScoopOffTheEnd(ref cipherBytes, ivLength);
            algorithm.Key = ArrayUtil.ScoopOffTheEnd(ref cipherBytes, keyLength);

            return algorithm;
        }

        /// <summary>
        /// Builds a symmetric algorithm.
        /// </summary>
        /// <param name="options">A <c>CryptoOptions</c> instance.</param>
        /// <returns></returns>
        public static SymmetricAlgorithm BuildSymmetricAlgorithm(CryptoOptions options)
        {
            options = options ?? new CryptoOptions();

            if (options.UseEmbeddedKIV && options.Key != null)
            {
                throw new UtilityException("Please leave the symmetric key null when using embedded mode");
            }

            // priority 1 - user-supplied (via 'options')
            // priority 2 - settings (app|web.config / default) - any single default setting can be substituted for a user-supplied setting
            var algorithm = BuildSymmetricAlgorithm
            (
                options.Algorithm ?? CryptoSettings.DefaultSymmetricAlgorithm,
                options.Key,
                options.AutoPadKey,
                options.IV,
                options.AutoPopulateIVFromKey,
                options.BlockSize,
                options.Mode,
                options.Padding
            );

            return algorithm;
        }

        /// <summary>
        /// Builds a symmectric algorithm from its constituent parts.
        /// </summary>
        /// <param name="algorithm">An instance of a symmetric algorithm.</param>
        /// <param name="key">The symmetric key <c>bytes</c>.</param>
        /// <param name="autoPadKey">Automatically grow the key to a valid size by prepending null bytes, if applicable.</param>
        /// <param name="iv">The symmetric initialization vector (defaults to <c>key</c> if not suppplied).</param>
        /// <param name="autoPopulateIVFromKey"></param>
        /// <param name="blockSize"></param>
        /// <param name="mode"></param>
        /// <param name="padding"></param>
        /// <returns></returns>
        /// <exception cref="ValidationException"></exception>
        public static SymmetricAlgorithm BuildSymmetricAlgorithm(SymmetricAlgorithm algorithm, byte[] key, bool autoPadKey, byte[] iv, bool autoPopulateIVFromKey, int? blockSize, CipherMode? mode, PaddingMode? padding)
        {
            if (algorithm == null)
                return null;
            var validKeySizes = algorithm.GetValidKeySizes();
            var validBlockSizes = algorithm.GetValidBlockSizes();
            if (key != null)
            {
                var keySize = key.Length * 8;  // convert to bits
                if (autoPadKey)
                {
                    if (keySize > validKeySizes.Max())
                    {
                        throw new ValidationException("Key exceeds max allowable size.  Detected: " + keySize + " bits.  Valid sizes: " + string.Join(", ", validKeySizes));
                    }
                    var targetLength = validKeySizes.First(ln => ln >= keySize) / 8;
                    key = key.Pad(targetLength, boundary: CollectionBoundary.Start);
                }
                else if (!keySize.In(validKeySizes))
                {
                    throw new ValidationException("Invalid key size.  Detected: " + keySize + " bits.  Valid sizes: " + string.Join(", ", validKeySizes));
                }
                algorithm.Key = key;
            }
            if (iv != null)
            {
                var ivSize = iv.Length * 8;  // convert to bits
                if (!ivSize.In(validBlockSizes))
                {
                    throw new ValidationException("Invalid IV size.  Detected: " + ivSize + " bits.  Valid sizes: " + string.Join(", ", validBlockSizes));
                }
                algorithm.IV = iv;
            }
            else if (key != null && autoPopulateIVFromKey)
            {
                algorithm.IV = key.Pad(algorithm.BlockSize / 8, boundary: CollectionBoundary.Start);
            }
            if (blockSize.HasValue)
            {
                if (!blockSize.Value.In(validBlockSizes))
                {
                    throw new ValidationException("Invalid block size: " + blockSize.Value + ".  Valid sizes: " + string.Join(", ", validBlockSizes));
                }
                algorithm.BlockSize = blockSize.Value;
            }
            if (mode.HasValue) algorithm.Mode = mode.Value;
            if (padding.HasValue) algorithm.Padding = padding.Value;
            return algorithm;
        }

        /// <summary>
        /// Lists an algorithm's valid key sizes in bits.
        /// </summary>
        /// <param name="symmetricAlgorithm">A <c>SymmetricAlgorithm</c>.</param>
        /// <returns></returns>
        public static IEnumerable<int> GetValidKeySizes(SymmetricAlgorithm symmetricAlgorithm)
        {
            return GetValidKeySizes(symmetricAlgorithm.LegalKeySizes);
        }

        /// <summary>
        /// Lists an algorithm's valid key sizes in bits.
        /// </summary>
        /// <param name="keySizes">The valid <c>KeySizes</c> corresponding to a <c>SymmetricAlgorithm</c>.</param>
        /// <returns></returns>
        public static IEnumerable<int> GetValidKeySizes(KeySizes[] keySizes)
        {
            var list = new List<int>();
            foreach (var keySize in keySizes)
            {
                if (keySize.MinSize == keySize.MaxSize)
                {
                    list.Add(keySize.MinSize);
                }
                else if (keySize.SkipSize == 0)   // this should never happen
                {
                    list.Add(keySize.MinSize);
                    list.Add(keySize.MaxSize);
                }
                else
                {
                    for (int i = keySize.MinSize; i <= keySize.MaxSize; i += keySize.SkipSize)
                    {
                        list.Add(i);
                    }
                }
            }
            list = list
                .Distinct()
                .OrderBy(i => i)
                .ToList();
            return list;
        }

        /// <summary>
        /// Utility method for getting bytes from a text key or iv. 
        /// </summary>
        /// <param name="key">A text key or iv.</param>
        /// <param name="encoding">An optional text encoding.</param>
        /// <returns></returns>
        public static byte[] KeyFromText(string key, Encoding encoding = null)
        {
            return (encoding ?? CryptoSettings.DefaultEncoding).GetBytes(key);
        }
    }
}
