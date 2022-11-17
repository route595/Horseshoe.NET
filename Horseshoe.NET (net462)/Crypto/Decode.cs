using System;
using System.Text;

namespace Horseshoe.NET.Crypto
{
    /// <summary>
    /// A collection of methods for decoding text
    /// </summary>
    public static class Decode
    {
        /// <summary>
        /// A collection of methods for Base64 decoding
        /// </summary>
        public static class Base64
        {
            /// <summary>
            /// Decodes Base64 encoded source <c>byte[]</c> to plain bytes
            /// </summary>
            /// <param name="encodedBytes">Base64 encoded source bytes</param>
            /// <param name="encoding">a character encoding</param>
            /// <returns>plain <c>byte[]</c></returns>
            public static byte[] Bytes(byte[] encodedBytes, Encoding encoding = null)
            {
                var encodedText = (encoding ?? Encoding.Default).GetString(encodedBytes);
                return Convert.FromBase64String(encodedText);
            }

            /// <summary>
            /// Decodes Base64 encoded source <c>string</c> to plain bytes
            /// </summary>
            /// <param name="encodedText">Base64 encoded source text</param>
            /// <returns>plain <c>byte[]</c></returns>
            public static byte[] Bytes(string encodedText)
            {
                return Convert.FromBase64String(encodedText);
            }

            /// <summary>
            /// Decodes Base64 encoded source string to plaintext <c>string</c>
            /// </summary>
            /// <param name="encodedText">Base64 encoded source text</param>
            /// <param name="encoding">a character encoding</param>
            /// <returns>plaintext <c>string</c></returns>
            public static string String(string encodedText, Encoding encoding = null)
            {
                Exception ex;

                try
                {
                    return (encoding ?? Encoding.Default).GetString(Bytes(encodedText));
                }
                catch (Exception _ex)
                {
                    if (encodedText.EndsWith("="))
                        throw;
                    ex = _ex;
                }

                for (int i = 1; i <= 2; i++)
                {
                    try
                    {
                        return (encoding ?? Encoding.Default).GetString(Bytes(encodedText + new string('=', i)));
                    }
                    catch { }
                }

                throw ex;
            }
        }
    }
}
