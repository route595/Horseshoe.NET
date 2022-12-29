using System;
using System.Net;
using System.Threading.Tasks;

namespace Horseshoe.NET.Http
{
    /// <summary>
    /// Methods for posting to PUT-enabled REST services
    /// </summary>
    public static class Put
    {
        public static string AsString
        (
            UriString uri,
            object content,
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
            if (journal == null)
            {
                journal = TraceJournal.ResetDefault();
            }
            journal.WriteEntry("Put.AsString()");
            journal.Level++;

            // pass the buck
            var response = Post.AsString
            (
                uri,
                content,
                method: "PUT",
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

            // finalize
            journal.Level--;
            return response;
        }

        public async static Task<string> AsStringAsync
        (
            UriString uri,
            object content,
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
            if (journal == null)
            {
                journal = TraceJournal.ResetDefault();
            }
            journal.WriteEntry("Put.AsStringAsync()");
            journal.Level++;

            // pass the buck
            var response = await Post.AsStringAsync
            (
                uri,
                content,
                method: "PUT",
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

            // finalize
            journal.Level--;
            return response;
        }

        public static T AsValue<T>
        (
            UriString uri,
            object content,
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
            if (journal == null)
            {
                journal = TraceJournal.ResetDefault();
            }
            journal.WriteEntry("Put.AsValue()");
            journal.Level++;

            // pass the buck
            var result = Post.AsValue<T>
            (
                uri,
                content,
                method: "PUT",
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
                responseParser: responseParser,
                journal: journal
            );

            // finalize
            journal.Level--;
            return result;
        }

        public async static Task<T> AsValueAsync<T>
        (
            UriString uri,
            object content,
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
            if (journal == null)
            {
                journal = TraceJournal.ResetDefault();
            }
            journal.WriteEntry("Put.AsValueAsync()");
            journal.Level++;

            // pass the buck
            var result = await Post.AsValueAsync<T>
            (
                uri,
                content,
                method: "PUT",
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
                responseParser: responseParser,
                journal: journal
            );

            // finalize
            journal.Level--;
            return result;
        }

        public static T AsJson<T>
        (
            UriString uri,
            object content,
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
            if (journal == null)
            {
                journal = TraceJournal.ResetDefault();
            }
            journal.WriteEntry("Put.AsJson()");
            journal.Level++;

            // pass the buck
            var result = Post.AsJson<T>
            (
                uri,
                content,
                method: "PUT",
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
                responseParser: responseParser,
                zapBackingFields: zapBackingFields,
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
            if (journal == null)
            {
                journal = TraceJournal.ResetDefault();
            }
            journal.WriteEntry("Put.AsJsonAsync()");
            journal.Level++;

            // pass the buck
            var result = await Post.AsJsonAsync<T>
            (
                uri,
                content,
                method: "PUT",
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
                responseParser: responseParser,
                zapBackingFields: zapBackingFields,
                journal: journal
            );

            // finalize
            journal.Level--;
            return result;
        }
    }
}
