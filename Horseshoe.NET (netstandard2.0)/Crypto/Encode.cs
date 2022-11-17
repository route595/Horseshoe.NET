using System;
using System.Text;

namespace Horseshoe.NET.Crypto
{
    /// <summary>
    /// A collection of methods for encoding text
    /// </summary>
    public static class Encode
    {
        /// <summary>
        /// A collection of methods for Base64 encoding
        /// </summary>
        public static class Base64
        {
            /// <summary>
            /// Encodes source bytes as a <c>char[]</c>
            /// </summary>
            /// <param name="plainBytes">source <c>byte[]</c></param>
            /// <returns></returns>
            /// <exception cref="ArgumentException"></exception>
            public static char[] Chars(byte[] plainBytes)
            {
                char[] chars = new char[0];
                int result = Convert.ToBase64CharArray(plainBytes, 0, plainBytes.Length, chars, 0);
                if (result != plainBytes.Length)
                {
                    throw new ArgumentException("encoding " + plainBytes.Length + " bytes resulted in a different output: " + result);
                }
                return chars;
            }

            /// <summary>
            /// Encodes source bytes as a <c>string</c>
            /// </summary>
            /// <param name="plainBytes">source <c>byte[]</c></param>
            /// <returns></returns>
            public static string String(byte[] plainBytes)
            {
                return Convert.ToBase64String(plainBytes);
            }


            /// <summary>
            /// Encodes source text as a <c>string</c>
            /// </summary>
            /// <param name="plaintext">source test</param>
            /// <param name="encoding">a character encoding</param>
            /// <returns></returns>
            public static string String(string plaintext, Encoding encoding = null)
            {
                return String((encoding ?? Encoding.Default).GetBytes(plaintext));
            }
        }
    }
}
