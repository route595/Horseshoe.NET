using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Horseshoe.NET.Crypto
{
    public static class Extensions
    {
        public static void Clear(this byte[] bytes)
        {
            for (int i = 0; i < bytes.Length; i++)
            {
                bytes[i] = byte.MinValue;
            }
        }

        public static void Clear(this char[] chars)
        {
            for (int i = 0; i < chars.Length; i++)
            {
                chars[i] = char.MinValue;
            }
        }

        public static IEnumerable<int> GetValidKeySizes(this SymmetricAlgorithm algorithm)
        {
            return CryptoUtil.GetValidKeySizes(algorithm.LegalKeySizes);
        }

        public static IEnumerable<int> GetValidBlockSizes(this SymmetricAlgorithm algorithm)
        {
            return CryptoUtil.GetValidKeySizes(algorithm.LegalBlockSizes);
        }
    }
}
