using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

using Horseshoe.NET.Collections;

namespace Horseshoe.NET.Crypto
{
    internal static class CryptoUtil
    {
        internal static SymmetricAlgorithm BuildSymmetricAlgorithmForEncryptionEmbeddedKIV(CryptoOptions options)
        {
            var algorithm = BuildSymmetricAlgorithm(options);

            algorithm.GenerateKey();
            algorithm.GenerateIV();

            return algorithm;
        }

        internal static SymmetricAlgorithm BuildSymmetricAlgorithmForDecryptionEmbeddedKIV(CryptoOptions options, ref byte[] cipherBytes)
        {
            var algorithm = BuildSymmetricAlgorithm(options);

            var keyLength = algorithm.KeySize / 8;
            var ivLength = algorithm.BlockSize / 8;

            algorithm.IV = ArrayUtil.ScoopOffTheEnd(ref cipherBytes, ivLength);
            algorithm.Key = ArrayUtil.ScoopOffTheEnd(ref cipherBytes, keyLength);

            return algorithm;
        }

        internal static SymmetricAlgorithm BuildSymmetricAlgorithm(CryptoOptions options)
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

        public static SymmetricAlgorithm BuildSymmetricAlgorithm(SymmetricAlgorithm algorithm, byte[] key, bool autoPadKey, byte[] iv, bool autoPopulateIVFromKey, int? blockSize, CipherMode? mode, PaddingMode? padding)
        {
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
                    var targetSize = validKeySizes.First(ln => ln >= keySize);
                    key = key.Pad(targetSize, boundary: CollectionBoundary.Start);
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

        public static SymmetricAlgorithm BuildNSymmetricAlgorithm(SymmetricAlgorithm algorithm, byte[] key, bool autoPadKey, byte[] iv, bool autoPopulateIVFromKey, int? blockSize, CipherMode? mode, PaddingMode? padding)
        {
            if (algorithm == null)
                return null;
            return BuildSymmetricAlgorithm(algorithm, key, autoPadKey, iv, autoPopulateIVFromKey, blockSize, mode, padding);
        }

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
    }
}
