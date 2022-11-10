using System;
using System.Net;
using System.Text;
using System.Web;
using Microsoft.IdentityModel.Tokens;

using Horseshoe.NET.Http;
using Horseshoe.NET.Text;

namespace Horseshoe.NET.Jwt
{
    public static class OAuthService
    {
        /// <summary>
        /// Request an OAuth token from the supplied URL with the supplied credentials
        /// </summary>
        /// <param name="uri">An OAuth token-issuing endpoint</param>
        /// <param name="cred">OAuth password credential</param>
        /// <param name="clientId">OAuth client ID</param>
        /// <param name="scope">OAuth scope (optional)</param>
        /// <param name="alterHeaders">Accesses the <c>WebHeadersCollections</c>, adds ("Accept", "*/*") if omitted</param>
        /// <param name="getRawResponse">Allows client code to inspect the raw HTTP response before JSON parsing is attempted. Example, getRawResponse: (str) => Console.WriteLine(str)</param>
        /// <returns>An <c>OAuthTokenEnvelope</c></returns>
        public static OAuthTokenEnvelope RequestToken(UriString uri, NetworkCredential cred, string clientId = null, string scope = "openid", Action<WebHeaderCollection> alterHeaders = null, Action<string> getRawResponse = null)
        {
            var rawEnvelope = Post.AsValue<OAuthTokenEnvelope.Raw>
            (
                uri,
                new StringBuilder("grant_type=password").Append('&')
                    .Append("username=").Append(HttpUtility.UrlEncode(cred.GetFullyQualifiedUserId())).Append('&')
                    .Append("password=").Append(HttpUtility.UrlEncode(cred.Password)).Append('&')
                    .Append("client_id=").Append(HttpUtility.UrlEncode(clientId)).Append('&')
                    .Append("scope=").Append(HttpUtility.UrlEncode(scope))
                    .ToString(),
                contentType: "application/x-www-form-urlencoded",
                alterHeaders: alterHeaders ?? ((hdrs) =>
                {
                    hdrs.AddAccept("*/*");
                }),
                getRawResponse: getRawResponse,
                responseParser: (raw) => Deserialize.Json<OAuthTokenEnvelope.Raw>(raw)
            );
            return rawEnvelope.ToOAuthTokenEnvelope();
        }

        /// <summary>
        /// Request an ADFS token converted to a custom token with a configurable duration
        /// </summary>
        /// <param name="uri">An OAuth token-issuing endpoint</param>
        /// <param name="query">OAuth credentials and role claim filter</param>
        /// <param name="tokenKey">A token key</param>
        /// <param name="clientId">OAuth client ID</param>
        /// <param name="scope">OAuth scope</param>
        /// <param name="alterHeaders">Accesses the <c>WebHeadersCollections</c>, adds ("Accept", "*/*") if omitted</param>
        /// <param name="getRawResponse">Allows client code to inspect the raw HTTP response before JSON parsing is attempted. Example, getRawResponse: (str) => Console.WriteLine(str)</param>
        /// <param name="keyId">The key Id associated with signing key (e.g. JWK, etc.)</param>
        /// <param name="lifespanInSeconds">Default is 3600 (1 hour)</param>
        /// <param name="securityAlgorithm">The key signing (hashing) algorithm</param>
        /// <param name="tokenKeyEncoding">The token key encoding</param>
        /// <returns>An encoded ADFS-based token</returns>
        public static string RequestAdfsBasedToken(UriString uri, AdfsBasedOAuthQuery query, string tokenKey, string clientId, string scope = "openid", Action<WebHeaderCollection> alterHeaders = null, Action<string> getRawResponse = null, string keyId = null, int lifespanInSeconds = 3600, string securityAlgorithm = SecurityAlgorithms.HmacSha256, Encoding tokenKeyEncoding = null)
        {
            return RequestAdfsBasedToken(uri, query: query, (tokenKeyEncoding ?? Encoding.Default).GetBytes(tokenKey), clientId, scope: scope, alterHeaders: alterHeaders, getRawResponse: getRawResponse, keyId: keyId, lifespanInSeconds: lifespanInSeconds, securityAlgorithm: securityAlgorithm);
        }

        /// <summary>
        /// Request an OAuth token converted to a custom token with a configurable duration
        /// </summary>
        /// <param name="uri">An OAuth token-issuing endpoint</param>
        /// <param name="query">OAuth credentials and role claim filter</param>
        /// <param name="tokenKeyBytes">A token key</param>
        /// <param name="clientId">OAuth client ID</param>
        /// <param name="scope">OAuth scope</param>
        /// <param name="alterHeaders">Accesses the <c>WebHeadersCollections</c>, adds ("Accept", "*/*") if omitted</param>
        /// <param name="getRawResponse">Allows client code to inspect the raw HTTP response before JSON parsing is attempted. Example, getRawResponse: (str) => Console.WriteLine(str)</param>
        /// <param name="keyId">The key Id associated with signing key (e.g. JWK, etc.)</param>
        /// <param name="lifespanInSeconds">Default is 3600 (1 hour)</param>
        /// <param name="securityAlgorithm">The key signing (hashing) algorithm</param>
        /// <returns>An encoded ADFS-based token</returns>
        public static string RequestAdfsBasedToken(UriString uri, AdfsBasedOAuthQuery query, byte[] tokenKeyBytes, string clientId, string scope = "openid", Action<WebHeaderCollection> alterHeaders = null, Action<string> getRawResponse = null, string keyId = null, int lifespanInSeconds = 3600, string securityAlgorithm = SecurityAlgorithms.HmacSha256)
        {
            var envelope = RequestToken(uri, query.Credentials, clientId: clientId, scope: scope, alterHeaders: alterHeaders, getRawResponse: getRawResponse);
            return TokenService.CreateAdfsBasedToken(envelope.AccessToken, tokenKeyBytes, roleClaimFilter: query.RoleClaimFilter, cred: query.Credentials, keyId: keyId, lifespanInSeconds: lifespanInSeconds, securityAlgorithm: securityAlgorithm);
        }
    }
}
