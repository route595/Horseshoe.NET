using System;
using System.Net;
using System.Threading.Tasks;

using Horseshoe.NET.RelayMessages;

// alt content type: application/json
// alt content type: application/x-www-form-urlencoded

namespace Horseshoe.NET.Http
{
    public static class Get
    {
        public static string MessageRelayGroup => HttpConstants.MessageRelayGroup;

        public static string AsString
        (
            UriString uri,
            string method = "GET",
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
                null, //content,
                method,
                null, //contentType,
                null, //contentSerializer,
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
            string method = "GET",
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
                null, //content,
                method,
                null, //contentType,
                null, //contentSerializer,
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
            string method = "GET",
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
                method: method,
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
            string method = "GET",
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
                method: method,
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
            string method = "GET",
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
                method: method,
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
            string method = "GET",
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
                method: method,
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
