using System;
using System.Net;
using System.Threading.Tasks;

using Horseshoe.NET.Text;

namespace Horseshoe.NET.Http
{
    /// <summary>
    /// Methods for posting JSON formatted data to REST services
    /// </summary>
    public static class PostJson
    {
        public static string AsString
        (
            UriString uri,
            object content,
            string method = "POST",
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
            TraceJournal journal = null
        )
        {
            // journaling
            if (journal == null)
            {
                journal = TraceJournal.ResetDefault();
            }
            journal.WriteEntry("PostJson.AsString()");
            journal.Level++;

            // pass the buck
            var response = Post.AsString
            (
                uri,
                content,
                method: method,
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
                journal: journal
            );

            // finalize
            journal.Level--;
            return response;
        }

        public async static Task<string> AsStringAsync
        (
            UriString uri,
            object content,
            string method = "POST",
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
            TraceJournal journal = null
        )
        {
            // journaling
            if (journal == null)
            {
                journal = TraceJournal.ResetDefault();
            }
            journal.WriteEntry("PostJson.AsStringAsync()");
            journal.Level++;

            // pass the buck
            var response = await Post.AsStringAsync
            (
                uri,
                content,
                method: method,
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
                journal: journal
            );

            // finalize
            journal.Level--;
            return response;
        }

        public static E AsValue<E>
        (
            UriString uri,
            object content,
            string method = "POST",
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
            Func<string, E> responseParser = null,
            TraceJournal journal = null
        )
        {
            // journaling
            if (journal == null)
            {
                journal = TraceJournal.ResetDefault();
            }
            journal.WriteEntry("PostJson.AsValue()");
            journal.Level++;

            // pass the buck
            var stringResult = AsString
            (
                uri,
                content,
                method: method,
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
            object content,
            string method = "POST",
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
            Func<string, E> responseParser = null,
            TraceJournal journal = null
        )
        {
            // journaling
            if (journal == null)
            {
                journal = TraceJournal.ResetDefault();
            }
            journal.WriteEntry("PostJson.AsValueAsync()");
            journal.Level++;

            // pass the buck
            var stringResult = await AsStringAsync
            (
                uri,
                content,
                method: method,
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
            object content,
            string method = "POST",
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
            journal.WriteEntry("PostJson.AsJson()");
            journal.Level++;

            // pass the buck
            var result = AsValue<E>
            (
                uri,
                content,
                method: method,
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
            object content,
            string method = "POST",
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
            journal.WriteEntry("PostJson.AsJsonAsync()");
            journal.Level++;

            // pass the buck
            var result = await AsValueAsync<E>
            (
                uri,
                content,
                method: method,
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
                responseParser: responseParser ?? WebResponseFactory.GetJsonDeserializer<E>(zapBackingFields),
                journal: journal
            );

            // finalize
            journal.Level--;
            return result;
        }
    }
}
