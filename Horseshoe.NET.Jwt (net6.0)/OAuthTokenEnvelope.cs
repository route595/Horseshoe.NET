using System;

namespace Horseshoe.NET.Jwt
{
    public class OAuthTokenEnvelope
    {
        public AccessToken AccessToken { get; set; }
        public string TokenType { get; set; }
        public DateTime ExpirationDate { get; set; }
        public string Resource { get; set; }
        public string RefreshToken { get; set; }
        public DateTime RefreshTokenExpirationDate { get; set; }
        public string Scope { get; set; }
        public string IdToken { get; set; }

        public class Raw
        {
#pragma warning disable IDE1006 // Naming Styles
            public string access_token { get; set; }
            public string token_type { get; set; }
            public int expires_in { get; set; }
            public string resource { get; set; }
            public string refresh_token { get; set; }
            public int refresh_token_expires_in { get; set; }
            public string scope { get; set; }
            public string id_token { get; set; }
#pragma warning restore IDE1006 // Naming Styles
        }
    }
}
