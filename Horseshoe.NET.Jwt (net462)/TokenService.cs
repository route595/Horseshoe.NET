using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using Horseshoe.NET.Crypto;
using Horseshoe.NET.Text;

namespace Horseshoe.NET.Jwt
{
    public static class TokenService
    {
        static Regex TokenPattern { get; } = new Regex(@"^[0-9a-z_-]+\.[0-9a-z_-]+(\.[0-9a-z_-]+)?$", RegexOptions.IgnoreCase);
        static Regex NotTokenPattern { get; } = new Regex("[^0-9a-z._-]", RegexOptions.IgnoreCase);

        /// <summary>
        /// Separates, decodes and deserializes a Base64 encoded OAuth token
        /// </summary>
        /// <param name="encodedToken"></param>
        /// <returns>A <c>Token</c></returns>
        public static AccessToken ParseToken(string encodedToken)
        {
            if (encodedToken == null)
            {
                throw new ValidationException("encoded token cannot be null");
            }
            if (encodedToken.StartsWith("Bearer"))
            {
                encodedToken = encodedToken.Substring(6).Trim();
            }
            if (!TokenPattern.IsMatch(encodedToken))
            {
                var invalidChars = NotTokenPattern.Matches(encodedToken)
                    .Cast<Match>()
                    .Select(m => m.Value)
                    .Distinct();
                throw invalidChars.Any()
                    ? new ValidationException("encoded token contains invalid characters: [" + string.Join("", invalidChars) + "]")
                    : new ValidationException("invalid encoded token");
            }
            var token = new AccessToken { EncodedToken = encodedToken };
            var split = encodedToken.Split('.');
            split[0] = Decode.Base64(split[0]);
            split[1] = Decode.Base64(split[1]);
            token.Header = DeserializeTokenHeader(split[0]);
            token.Body = DeserializeTokenBody(split[1]);
            token.RawDigitalSignature = split.Length > 2 ? split[2] : null;
            return token;
        }

        public static AccessTokenHeader DeserializeTokenHeader(string tokenHeaderString)
        {
            try
            {
                var rawHeader = Deserialize.Json<AccessTokenHeader.Raw>(tokenHeaderString);
                return rawHeader.ToTokenHeader();
            }
            catch (Exception ex)
            {
                throw new ValidationException("Error deserializing token header: " + ex.Message, ex);
            }
        }

        public static AccessTokenBody DeserializeTokenBody(string tokenBodyString)
        {
            Exception deserializeJsonArrayException = null;
            try
            {
                var rawBody_roleArray = Deserialize.Json<AccessTokenBody.Raw_RoleArray>(tokenBodyString);
                return rawBody_roleArray.ToTokenBody();
            }
            catch (Exception ex)
            {
                deserializeJsonArrayException = ex;
            }

            try
            {
                var rawBody_roleString = Deserialize.Json<AccessTokenBody.Raw_RoleString>(tokenBodyString);
                return rawBody_roleString.ToTokenBody();
            }
            catch
            {
                throw deserializeJsonArrayException;
            }
        }

        public static byte[] GetKeyFromBase64(string keyTextBase64)
        {
            return Convert.FromBase64String(keyTextBase64);
        }

        public static byte[] GetKeyFromText(string keyText)
        {
            return Encoding.ASCII.GetBytes(keyText);
        }

        public static byte[] GenerateKey(int size = 32)
        {
            var now = DateTime.Now;
            var rand = new Random(now.Year + now.Month + now.Day + now.Hour + now.Minute + now.Second + now.Millisecond);
            var array = new byte[size];
            for (int i = 0; i < size; i++)
            {
                //array[i] = i == 0
                //    ? GetRandomAlphanumericByte(rand, setId: 1)
                //    : GetRandomAlphanumericByte(rand);
                array[i] = GetRandomAlphanumericByte(rand);
            }
            return array;
        }

        static byte GetRandomAlphanumericByte(Random rand, int? setId = default)
        {
            var set = setId ?? rand.Next(0, 3);
            switch(set)
            {
                case 0:
                    return (byte)rand.Next(48, 58);  // num
                case 1:
                    return (byte)rand.Next(65, 91);  // ucase
                case 2:
                    return (byte)rand.Next(97, 123); // lcase
                default:
                    return (byte)'#';
            };
        }

        public static string KeyToText(byte[] key)
        {
            return Encoding.ASCII.GetString(key);
        }

        public static string KeyToBase64(byte[] key)
        {
            return Convert.ToBase64String(key);
        }
    }
}
