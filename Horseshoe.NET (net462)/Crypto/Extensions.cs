using System.Collections.Generic;
using System.Security.Cryptography;

namespace Horseshoe.NET.Crypto
{
    /// <summary>
    /// Extension methods for crypography
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Zero out a <c>byte[]</c>
        /// </summary>
        /// <param name="bytes">a <c>byte[]</c></param>
        public static void SecureClear(this byte[] bytes)
        {
            for (int i = 0; i < bytes.Length; i++)
            {
                bytes[i] = byte.MinValue;
            }
        }

        /// <summary>
        /// Zero out a <c>char[]</c>
        /// </summary>
        /// <param name="chars">a <c>char[]</c></param>
        public static void SecureClear(this char[] chars)
        {
            for (int i = 0; i < chars.Length; i++)
            {
                chars[i] = char.MinValue;
            }
        }

        /// <summary>
        /// Gets valid key sizes
        /// </summary>
        /// <param name="algorithm">a symmetric algorithm</param>
        /// <returns></returns>
        public static IEnumerable<int> GetValidKeySizes(this SymmetricAlgorithm algorithm)
        {
            return CryptoUtil.GetValidKeySizes(algorithm.LegalKeySizes);
        }

        /// <summary>
        /// Gets valid block sizes
        /// </summary>
        /// <param name="algorithm">a symmetric algorithm</param>
        /// <returns></returns>
        public static IEnumerable<int> GetValidBlockSizes(this SymmetricAlgorithm algorithm)
        {
            return CryptoUtil.GetValidKeySizes(algorithm.LegalBlockSizes);
        }
    }
}
