using System;
using System.Net;
using System.Threading.Tasks;

using Horseshoe.NET.RelayMessages;

namespace Horseshoe.NET.Http
{
    /// <summary>
    /// Methods for posting to REST services
    /// </summary>
    public static class Post
    {
        public static string MessageRelayGroup => HttpConstants.MessageRelayGroup;

        public static string AsString
        (
            UriString uri,
            object content,
            string method = "POST",
            string contentType = null,
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

            var request = WebRequestFactory.GetWebRequest
            (
                uri,
                content,
                method,
                contentType,
                contentSerializer,
                securityProtocol,
                alterHeaders,
                credentials,
                proxyAddress,
                proxyPort,
                proxyCredentials,
                alterRequest
            );

            var response = WebResponseFactory.ProcessResponse
            (
                request,
                handleResponse
            );

            SystemMessageRelay.RelayMethodReturnValue(response, group: MessageRelayGroup);
            return response;
        }

        public async static Task<string> AsStringAsync
        (
            UriString uri,
            object content,
            string method = "POST",
            string contentType = null,
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

            var request = WebRequestFactory.GetWebRequest
            (
                uri,
                content,
                method,
                contentType,
                contentSerializer,
                securityProtocol,
                alterHeaders,
                credentials,
                proxyAddress,
                proxyPort,
                proxyCredentials,
                alterRequest
            );

            var response = await WebResponseFactory.ProcessResponseAsync
            (
                request,
                handleResponse
            );

            SystemMessageRelay.RelayMethodReturnValue(response, group: MessageRelayGroup);
            return response;
        }

        public static T AsValue<T>
        (
            UriString uri,
            object content,
            string method = "POST",
            string contentType = null,
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

            var stringResult = AsString
            (
                uri,
                content,
                method: method,
                contentType: contentType,
                contentSerializer: contentSerializer,
                securityProtocol: securityProtocol,
                alterHeaders: alterHeaders,
                credentials: credentials,
                proxyAddress: proxyAddress,
                proxyPort: proxyPort,
                proxyCredentials: proxyCredentials,
                alterRequest: alterRequest,
                handleResponse: handleResponse
            );

            getRawResponse?.Invoke(stringResult);

            T result;
            if (responseParser != null)
            {
                SystemMessageRelay.RelayMessage("using user-supplied response parser", group: MessageRelayGroup);
                result = responseParser.Invoke(stringResult);
            }
            else
            {
                SystemMessageRelay.RelayMessage("attempting built-in parser for response", group: MessageRelayGroup);
                result = Zap.To<T>(stringResult);
            }
            SystemMessageRelay.RelayMessage("parser appears to have succeeded", group: MessageRelayGroup);

            SystemMessageRelay.RelayMethodReturnValue(result, group: MessageRelayGroup);
            return result;
        }

        public async static Task<T> AsValueAsync<T>
        (
            UriString uri,
            object content,
            string method = "POST",
            string contentType = null,
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

            var stringResult = await AsStringAsync
            (
                uri,
                content,
                method: method,
                contentType: contentType,
                contentSerializer: contentSerializer,
                securityProtocol: securityProtocol,
                alterHeaders: alterHeaders,
                credentials: credentials,
                proxyAddress: proxyAddress,
                proxyPort: proxyPort,
                proxyCredentials: proxyCredentials,
                alterRequest: alterRequest,
                handleResponse: handleResponse
            );

            getRawResponse?.Invoke(stringResult);

            T result;
            if (responseParser != null)
            {
                SystemMessageRelay.RelayMessage("using user-supplied response parser", group: MessageRelayGroup);
                result = responseParser.Invoke(stringResult);
            }
            else
            {
                SystemMessageRelay.RelayMessage("attempting built-in parser for response", group: MessageRelayGroup);
                result = Zap.To<T>(stringResult);
            }
            SystemMessageRelay.RelayMessage("parser appears to have succeeded", group: MessageRelayGroup);

            SystemMessageRelay.RelayMethodReturnValue(result, group: MessageRelayGroup);
            return result;
        }

        public static T AsJson<T>
        (
            UriString uri,
            object content,
            string method = "POST",
            string contentType = null,
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

            var result = AsValue<T>
            (
                uri,
                content,
                method: method,
                contentType: contentType,
                contentSerializer: contentSerializer,
                securityProtocol: securityProtocol,
                alterHeaders: alterHeaders,
                credentials: credentials,
                proxyAddress: proxyAddress,
                proxyPort: proxyPort,
                proxyCredentials: proxyCredentials,
                alterRequest: alterRequest,
                handleResponse: handleResponse,
                getRawResponse: getRawResponse,
                responseParser: responseParser ?? WebResponseFactory.GetJsonDeserializer<T>(zapBackingFields)
            );

            SystemMessageRelay.RelayMethodReturnValue(result, group: MessageRelayGroup);
            return result;
        }

        public async static Task<T> AsJsonAsync<T>
        (
            UriString uri,
            object content,
            string method = "POST",
            string contentType = null,
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

            var result = await AsValueAsync<T>
            (
                uri,
                content,
                method: method,
                contentType: contentType,
                contentSerializer: contentSerializer,
                securityProtocol: securityProtocol,
                alterHeaders: alterHeaders,
                credentials: credentials,
                proxyAddress: proxyAddress,
                proxyPort: proxyPort,
                proxyCredentials: proxyCredentials,
                alterRequest: alterRequest,
                handleResponse: handleResponse,
                getRawResponse: getRawResponse,
                responseParser: responseParser ?? WebResponseFactory.GetJsonDeserializer<T>(zapBackingFields)
            );

            SystemMessageRelay.RelayMethodReturnValue(result, group: MessageRelayGroup);
            return result;
        }
    }
}
