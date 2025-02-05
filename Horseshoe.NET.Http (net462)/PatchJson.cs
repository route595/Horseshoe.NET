using System;
using System.Net;
using System.Threading.Tasks;

using Horseshoe.NET.RelayMessages;
using Horseshoe.NET.Text;

namespace Horseshoe.NET.Http
{
    /// <summary>
    /// Methods for posting JSON formatted data to PATCH-enabled REST services
    /// </summary>
    public static class PatchJson
    {
        public static string MessageRelayGroup => HttpConstants.MessageRelayGroup;

        public static string AsString
        (
            UriString uri,
            object content,
            string contentType = "application/json",
            Func<object, string> contentSerializer = null,
            SecurityProtocolType? securityProtocol = null,
            Action<WebHeaderCollection> alterHeaders = null,
            NetworkCredential credentials = null,
            UriString? proxyAddress = null,
            int? proxyPort = null,
            NetworkCredential proxyCredentials = null,
            Action<HttpWebRequest> alterRequest = null,
            Action<HttpWebResponse, ConsumerResponseEnvelope> handleResponse = null
        )
        {
            SystemMessageRelay.RelayMethodInfo(group: MessageRelayGroup);

            var result = Post.AsString
            (
                uri,
                content,
                method: "PATCH",
                contentType: contentType,
                contentSerializer: contentSerializer ?? ((obj) => Serialize.Json(obj)),
                securityProtocol: securityProtocol,
                alterHeaders: alterHeaders,
                credentials: credentials,
                proxyAddress: proxyAddress,
                proxyPort: proxyPort,
                proxyCredentials: proxyCredentials,
                alterRequest: alterRequest,
                handleResponse: handleResponse
            );

            SystemMessageRelay.RelayMethodReturnValue(result, group: MessageRelayGroup);
            return result;
        }

        public async static Task<string> AsStringAsync
        (
            UriString uri,
            object content,
            string contentType = "application/json",
            Func<object, string> contentSerializer = null,
            SecurityProtocolType? securityProtocol = null,
            Action<WebHeaderCollection> alterHeaders = null,
            NetworkCredential credentials = null,
            UriString? proxyAddress = null,
            int? proxyPort = null,
            NetworkCredential proxyCredentials = null,
            Action<HttpWebRequest> alterRequest = null,
            Action<HttpWebResponse, ConsumerResponseEnvelope> handleResponse = null
        )
        {
            SystemMessageRelay.RelayMethodInfo(group: MessageRelayGroup);

            var result = await Post.AsStringAsync
            (
                uri,
                content,
                method: "PATCH",
                contentType: contentType,
                contentSerializer: contentSerializer ?? ((obj) => Serialize.Json(obj)),
                securityProtocol: securityProtocol,
                alterHeaders: alterHeaders,
                credentials: credentials,
                proxyAddress: proxyAddress,
                proxyPort: proxyPort,
                proxyCredentials: proxyCredentials,
                alterRequest: alterRequest,
                handleResponse: handleResponse
            );

            SystemMessageRelay.RelayMethodReturnValue(result, group: MessageRelayGroup);
            return result;
        }

        public static T AsValue<T>
        (
            UriString uri,
            object content,
            string contentType = "application/json",
            Func<object, string> contentSerializer = null,
            SecurityProtocolType? securityProtocol = null,
            Action<WebHeaderCollection> alterHeaders = null,
            NetworkCredential credentials = null,
            UriString? proxyAddress = null,
            int? proxyPort = null,
            NetworkCredential proxyCredentials = null,
            Action<HttpWebRequest> alterRequest = null,
            Action<HttpWebResponse, ConsumerResponseEnvelope> handleResponse = null,
            Action<string> getRawResponse = null,
            Func<string, T> responseParser = null
        )
        {
            SystemMessageRelay.RelayMethodInfo(group: MessageRelayGroup);

            var result = Post.AsValue<T>
            (
                uri,
                content,
                method: "PATCH",
                contentType: contentType,
                contentSerializer: contentSerializer ?? ((obj) => Serialize.Json(obj)),
                securityProtocol: securityProtocol,
                alterHeaders: alterHeaders,
                credentials: credentials,
                proxyAddress: proxyAddress,
                proxyPort: proxyPort,
                proxyCredentials: proxyCredentials,
                alterRequest: alterRequest,
                handleResponse: handleResponse,
                getRawResponse: getRawResponse,
                responseParser: responseParser
            );

            SystemMessageRelay.RelayMethodReturnValue(result, group: MessageRelayGroup);
            return result;
        }

        public async static Task<T> AsValueAsync<T>
        (
            UriString uri,
            object content,
            string contentType = "application/json",
            Func<object, string> contentSerializer = null,
            SecurityProtocolType? securityProtocol = null,
            Action<WebHeaderCollection> alterHeaders = null,
            NetworkCredential credentials = null,
            UriString? proxyAddress = null,
            int? proxyPort = null,
            NetworkCredential proxyCredentials = null,
            Action<HttpWebRequest> alterRequest = null,
            Action<HttpWebResponse, ConsumerResponseEnvelope> handleResponse = null,
            Action<string> getRawResponse = null,
            Func<string, T> responseParser = null
        )
        {
            SystemMessageRelay.RelayMethodInfo(group: MessageRelayGroup);

            var result = await Post.AsValueAsync<T>
            (
                uri,
                content,
                method: "PATCH",
                contentType: contentType,
                contentSerializer: contentSerializer ?? ((obj) => Serialize.Json(obj)),
                securityProtocol: securityProtocol,
                alterHeaders: alterHeaders,
                credentials: credentials,
                proxyAddress: proxyAddress,
                proxyPort: proxyPort,
                proxyCredentials: proxyCredentials,
                alterRequest: alterRequest,
                handleResponse: handleResponse,
                getRawResponse: getRawResponse,
                responseParser: responseParser
            );

            SystemMessageRelay.RelayMethodReturnValue(result, group: MessageRelayGroup);
            return result;
        }

        public static T AsJson<T>
        (
            UriString uri,
            object content,
            string contentType = "application/json",
            Func<object, string> contentSerializer = null,
            SecurityProtocolType? securityProtocol = null,
            Action<WebHeaderCollection> alterHeaders = null,
            NetworkCredential credentials = null,
            UriString? proxyAddress = null,
            int? proxyPort = null,
            NetworkCredential proxyCredentials = null,
            Action<HttpWebRequest> alterRequest = null,
            Action<HttpWebResponse, ConsumerResponseEnvelope> handleResponse = null,
            Action<string> getRawResponse = null,
            Func<string, T> responseParser = null,
            bool zapBackingFields = false
        )
        {
            SystemMessageRelay.RelayMethodInfo(group: MessageRelayGroup);

            var result = Post.AsJson<T>
            (
                uri,
                content,
                method: "PATCH",
                contentType: contentType,
                contentSerializer: contentSerializer ?? ((obj) => Serialize.Json(obj)),
                securityProtocol: securityProtocol,
                alterHeaders: alterHeaders,
                credentials: credentials,
                proxyAddress: proxyAddress,
                proxyPort: proxyPort,
                proxyCredentials: proxyCredentials,
                alterRequest: alterRequest,
                handleResponse: handleResponse,
                getRawResponse: getRawResponse,
                responseParser: responseParser,
                zapBackingFields: zapBackingFields
            );

            SystemMessageRelay.RelayMethodReturnValue(result, group: MessageRelayGroup);
            return result;
        }

        public async static Task<T> AsJsonAsync<T>
        (
            UriString uri,
            object content,
            string contentType = "application/json",
            Func<object, string> contentSerializer = null,
            SecurityProtocolType? securityProtocol = null,
            Action<WebHeaderCollection> alterHeaders = null,
            NetworkCredential credentials = null,
            UriString? proxyAddress = null,
            int? proxyPort = null,
            NetworkCredential proxyCredentials = null,
            Action<HttpWebRequest> alterRequest = null,
            Action<HttpWebResponse, ConsumerResponseEnvelope> handleResponse = null,
            Action<string> getRawResponse = null,
            Func<string, T> responseParser = null,
            bool zapBackingFields = false
        )
        {
            SystemMessageRelay.RelayMethodInfo(group: MessageRelayGroup);

            var result = await Post.AsJsonAsync<T>
            (
                uri,
                content,
                method: "PATCH",
                contentType: contentType,
                contentSerializer: contentSerializer ?? ((obj) => Serialize.Json(obj)),
                securityProtocol: securityProtocol,
                alterHeaders: alterHeaders,
                credentials: credentials,
                proxyAddress: proxyAddress,
                proxyPort: proxyPort,
                proxyCredentials: proxyCredentials,
                alterRequest: alterRequest,
                handleResponse: handleResponse,
                getRawResponse: getRawResponse,
                responseParser: responseParser,
                zapBackingFields: zapBackingFields
            );

            SystemMessageRelay.RelayMethodReturnValue(result, group: MessageRelayGroup);
            return result;
        }
    }
}
