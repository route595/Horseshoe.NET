using System;
using System.Net;
using System.Text;
using System.Web;

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
    }
}
