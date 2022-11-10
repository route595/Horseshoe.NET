using System;
using System.Net;
using System.Threading.Tasks;

// alt content type: application/json
// alt content type: application/x-www-form-urlencoded

namespace Horseshoe.NET.Http
{
    public static class Get
    {
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
            Action<HttpWebResponse, ConsumerResponseEnvelope> handleResponse = null,
            TraceJournal journal = null
        )
        {
            // journaling
            if (journal == null)
            {
                journal = TraceJournal.ResetDefault();
            }
            journal.WriteEntry("Get.AsString()");
            journal.Level++;

            // pass the buck
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
                alterRequest,
                journal
            );

            var response = WebResponseFactory.ProcessResponse
            (
                request,
                handleResponse,
                journal
            );

            // finalize
            journal.Level--;
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
            Action<HttpWebResponse, ConsumerResponseEnvelope> handleResponse = null,
            TraceJournal journal = null
        )
        {
            // journaling
            if (journal == null)
            {
                journal = TraceJournal.ResetDefault();
            }
            journal.WriteEntry("Get.AsStringAsync()");
            journal.Level++;

            // pass the buck
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
                alterRequest,
                journal
            );

            var response = await WebResponseFactory.ProcessResponseAsync
            (
                request, 
                handleResponse, 
                journal
            );

            // finalize
            journal.Level--;
            return response;
        }

        public static E AsValue<E>
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
            Func<string, E> responseParser = null,
            TraceJournal journal = null
        )
        {
            // journaling
            if (journal == null)
            {
                journal = TraceJournal.ResetDefault();
            }
            journal.WriteEntry("Get.AsValue()");
            journal.Level++;

            // pass the buck
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
                handleResponse: handleResponse,
                journal: journal
            );

            getRawResponse?.Invoke(stringResult);

            var result = (responseParser ?? HttpResponseParsers.Get<E>() ?? throw new ValidationException("response parser was not supplied"))
                .Invoke(stringResult);

            // finalize
            journal.Level--;
            return result;
        }

        public async static Task<E> AsValueAsync<E>
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
            Func<string, E> responseParser = null,
            TraceJournal journal = null
        )
        {
            // journaling
            if (journal == null)
            {
                journal = TraceJournal.ResetDefault();
            }
            journal.WriteEntry("Get.AsValueAsync()");
            journal.Level++;

            // pass the buck
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
                handleResponse: handleResponse,
                journal: journal
            );

            getRawResponse?.Invoke(stringResult);

            var result = (responseParser ?? HttpResponseParsers.Get<E>() ?? throw new ValidationException("response parser was not supplied"))
                .Invoke(stringResult);

            // finalize
            journal.Level--;
            return result;
        }

        public static E AsJson<E>
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
            Func<string, E> responseParser = null,
            bool zapBackingFields = false,
            TraceJournal journal = null
        )
        {
            // journaling
            if (journal == null)
            {
                journal = TraceJournal.ResetDefault();
            }
            journal.WriteEntry("Get.AsJson()");
            journal.Level++;

            // pass the buck
            var result = AsValue<E>
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
                responseParser: responseParser ?? WebResponseFactory.GetJsonDeserializer<E>(zapBackingFields),
                journal: journal
            );

            // finalize
            journal.Level--;
            return result;
        }

        public async static Task<E> AsJsonAsync<E>
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
            Func<string, E> responseParser = null,
            bool zapBackingFields = false,
            TraceJournal journal = null
        )
        {
            // journaling
            if (journal == null)
            {
                journal = TraceJournal.ResetDefault();
            }
            journal.WriteEntry("Get.AsJsonAsync()");
            journal.Level++;

            // pass the buck
            var result = await AsValueAsync<E>
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
                responseParser: responseParser ?? WebResponseFactory.GetJsonDeserializer<E>(zapBackingFields),
                journal: journal
            );

            // finalize
            journal.Level--;
            return result;
        }
    }
}
