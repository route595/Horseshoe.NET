using System;
using System.Net;
using System.Threading.Tasks;

namespace Horseshoe.NET.Http
{
    /// <summary>
    /// Methods for posting to REST services
    /// </summary>
    public static class Post
    {
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
            Action<HttpWebResponse, ConsumerResponseEnvelope> handleResponse = null,
            TraceJournal journal = null
        )
        {
            // journaling
            journal = journal ?? new TraceJournal();
            journal.WriteEntry("Post.AsString()");
            journal.Level++;

            // pass the buck
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
            TraceJournal journal = null
        )
        {
            // journaling
            journal = journal ?? new TraceJournal();
            journal.WriteEntry("Post.AsStringAsync()");
            journal.Level++;

            // pass the buck
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
            Func<string, T> responseParser = null,
            TraceJournal journal = null
        )
        {
            // journaling
            journal = journal ?? new TraceJournal();
            journal.WriteEntry("Post.AsValue()");
            journal.Level++;

            // pass the buck
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
                handleResponse: handleResponse,
                journal: journal
            );

            getRawResponse?.Invoke(stringResult);

            T result;
            if (responseParser != null)
            {
                journal.WriteEntry("using user-supplied response parser");
                result = responseParser.Invoke(stringResult);
            }
            else
            {
                journal.WriteEntry("attempting built-in parser for response");
                result = Zap.To<T>(stringResult);
            }
            journal.WriteEntry("parser appears to have succeeded");

            // finalize
            journal.Level--;
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
            Func<string, T> responseParser = null,
            TraceJournal journal = null
        )
        {
            // journaling
            journal = journal ?? new TraceJournal();
            journal.WriteEntry("Post.AsValueAsync()");
            journal.Level++;

            // pass the buck
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
                handleResponse: handleResponse,
                journal: journal
            );

            getRawResponse?.Invoke(stringResult);

            T result;
            if (responseParser != null)
            {
                journal.WriteEntry("using user-supplied response parser");
                result = responseParser.Invoke(stringResult);
            }
            else
            {
                journal.WriteEntry("attempting built-in parser for response");
                result = Zap.To<T>(stringResult);
            }
            journal.WriteEntry("parser appears to have succeeded");

            // finalize
            journal.Level--;
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
            bool zapBackingFields = false,
            TraceJournal journal = null
        )
        {
            // journaling
            journal = journal ?? new TraceJournal();
            journal.WriteEntry("Post.AsJson()");
            journal.Level++;

            // pass the buck
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
                responseParser: responseParser ?? WebResponseFactory.GetJsonDeserializer<T>(zapBackingFields),
                journal: journal
            );

            // finalize
            journal.Level--;
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
            bool zapBackingFields = false,
            TraceJournal journal = null
        )
        {
            // journaling
            journal = journal ?? new TraceJournal();
            journal.WriteEntry("Post.AsJsonAsync()");
            journal.Level++;

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
                responseParser: responseParser ?? WebResponseFactory.GetJsonDeserializer<T>(zapBackingFields),
                journal: journal
            );

            // finalize
            journal.Level--;
            return result;
        }
    }
}
