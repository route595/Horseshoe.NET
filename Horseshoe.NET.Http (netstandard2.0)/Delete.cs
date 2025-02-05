using System;
using System.Net;
using System.Threading.Tasks;

using Horseshoe.NET.RelayMessages;

namespace Horseshoe.NET.Http
{
    public static class Delete
    {
        public static string MessageRelayGroup => HttpConstants.MessageRelayGroup;

        public static string AsString
        (
            UriString uri,
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

            var response = Get.AsString
            (
                uri,
                method: "DELETE",
                securityProtocol: securityProtocol,
                alterHeaders: alterHeaders,
                credentials: credentials,
                proxyAddress: proxyAddress,
                proxyPort: proxyPort,
                proxyCredentials: proxyCredentials,
                alterRequest: alterRequest,
                handleResponse: handleResponse
            );

            SystemMessageRelay.RelayMethodReturnValue(response, group: MessageRelayGroup);
            return response;
        }

        public async static Task<string> AsStringAsync
        (
            UriString uri,
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

            var response = await Get.AsStringAsync
            (
                uri,
                method: "DELETE",
                securityProtocol: securityProtocol,
                alterHeaders: alterHeaders,
                credentials: credentials,
                proxyAddress: proxyAddress,
                proxyPort: proxyPort,
                proxyCredentials: proxyCredentials,
                alterRequest: alterRequest,
                handleResponse: handleResponse
            );

            SystemMessageRelay.RelayMethodReturnValue(response, group: MessageRelayGroup);
            return response;
        }

        public static T AsValue<T>
        (
            UriString uri,
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

            var result = Get.AsValue<T>
            (
                uri,
                method: "DELETE",
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

            var result = await Get.AsValueAsync<T>
            (
                uri,
                method: "DELETE",
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

            var result = Get.AsJson<T>
            (
                uri,
                method: "DELETE",
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

            var result = await Get.AsJsonAsync<T>
            (
                uri,
                method: "DELETE",
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
