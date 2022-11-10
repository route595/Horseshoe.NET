using System;
using System.Linq;
using System.Net;
using System.Web;

using Horseshoe.NET.Http;

namespace Horseshoe.NET.Jwt
{
    public static class Extensions
    {
        /// <summary>
        /// Finds and parses the token in the request 'authorization' header, if applicable
        /// </summary>
        /// <param name="request"></param>
        /// <returns>The parsed <c>Token</c></returns>
        public static AccessToken GetAuthToken(this HttpRequest request)
        {
            var authorizationHeader = request.Headers["Authorization"];
            return authorizationHeader != null
                ? TokenService.ParseToken(authorizationHeader)
                : null;
        }

        public static OAuthTokenEnvelope ToOAuthTokenEnvelope(this OAuthTokenEnvelope.Raw rawOAuthTokenEnvelope)
        {
            return new OAuthTokenEnvelope
            {
                AccessToken = TokenService.ParseToken(rawOAuthTokenEnvelope.access_token),
                TokenType = rawOAuthTokenEnvelope.token_type,
                ExpirationDate = DateTime.Now.AddSeconds(rawOAuthTokenEnvelope.expires_in),
                Resource = rawOAuthTokenEnvelope.resource,
                RefreshToken = rawOAuthTokenEnvelope.refresh_token,
                RefreshTokenExpirationDate = DateTime.Now.AddSeconds(rawOAuthTokenEnvelope.refresh_token_expires_in),
                Scope = rawOAuthTokenEnvelope.scope,
                IdToken = rawOAuthTokenEnvelope.id_token
            };
        }

        public static string GetFullyQualifiedUserId(this NetworkCredential cred)
        {
            if (cred.UserName == null)
                return "";
            if (cred.UserName.Contains('@') || cred.UserName.Contains('\\') || cred.UserName.Contains('/') || cred.Domain == null)
                return cred.UserName;
            if (cred.Domain.Contains('.'))
                return cred.UserName + "@" + cred.Domain;
            return cred.Domain + "\\" + cred.UserName;
        }

        public static void AddBearerToken(this WebHeaderCollection hdrs, string rawToken)
        {
            hdrs.AddAuthorization("Bearer " + rawToken);
        }

        public static void AddBearerToken(this WebHeaderCollection hdrs, AccessToken token)
        {
            hdrs.AddAuthorization("Bearer " + token);
        }
    }
}
