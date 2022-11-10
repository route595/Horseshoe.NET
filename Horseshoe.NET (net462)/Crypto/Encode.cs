using System;
using System.Text;

namespace Horseshoe.NET.Crypto
{
    public static class Encode
    {
        public static string Base64(string plaintext, Encoding encoding = null)
        {
            var bytes = (encoding ?? Encoding.Default).GetBytes(plaintext);
            return Convert.ToBase64String(bytes);
        }
    }
}
