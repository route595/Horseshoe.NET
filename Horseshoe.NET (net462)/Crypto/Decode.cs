using System;
using System.Text;

namespace Horseshoe.NET.Crypto
{
    public static class Decode
    {
        public static byte[] Base64ToBytes(string base64Encoded)
        {
            return Convert.FromBase64String(base64Encoded);
        }

        public static string Base64(string base64Encoded, Encoding encoding = null)
        {
            var pad = base64Encoded.EndsWith("==") ? 2 : (base64Encoded.EndsWith("=") ? 1 : 0);
            Exception _ex = null;
            for (int i = pad; i <= 2; i++)
            {
                try
                {
                    return (encoding ?? Encoding.Default).GetString(Convert.FromBase64String(base64Encoded));
                }
                catch(Exception ex)
                {
                    if (_ex == null)
                    {
                        _ex = ex;
                    }
                }
                base64Encoded += '=';
            }
            throw _ex;
        }
    }
}
