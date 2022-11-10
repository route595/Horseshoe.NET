using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Horseshoe.NET.Http
{
    /// <summary>
    /// Methods for posting to PATCH-enabled REST services
    /// </summary>
    public static class Patch
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
            journal.WriteEntry("Patch.AsString()");
            journal.Level++;

            // pass the buck
            var response = Post.AsString
            (
                uri,
                content,
                method: "PATCH",
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
            journal.WriteEntry("Patch.AsStringAsync()");
            journal.Level++;

            // pass the buck
            var response = await Post.AsStringAsync
            (
                uri,
                content,
                method: "PATCH",
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

        public static E AsValue<E>
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
            Func<string, E> responseParser = null,
            TraceJournal journal = null
        )
        {
            // journaling
            if (journal == null)
            {
                journal = TraceJournal.ResetDefault();
            }
            journal.WriteEntry("Patch.AsValue()");
            journal.Level++;

            // pass the buck
            var result = Post.AsValue<E>
            (
                uri,
                content,
                method: "PATCH",
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

        public async static Task<E> AsValueAsync<E>
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
            Func<string, E> responseParser = null,
            TraceJournal journal = null
        )
        {
            // journaling
            if (journal == null)
            {
                journal = TraceJournal.ResetDefault();
            }
            journal.WriteEntry("Patch.AsValueAsync()");
            journal.Level++;

            // pass the buck
            var result = await Post.AsValueAsync<E>
            (
                uri,
                content,
                method: "PATCH",
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

        public static E AsJson<E>
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
            journal.WriteEntry("Patch.AsJson()");
            journal.Level++;

            // pass the buck
            var result = Post.AsJson<E>
            (
                uri,
                content,
                method: "PATCH",
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

        public async static Task<E> AsJsonAsync<E>
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
            journal.WriteEntry("Patch.AsJsonAsync()");
            journal.Level++;

            // pass the buck
            var result = await Post.AsJsonAsync<E>
            (
                uri,
                content,
                method: "PATCH",
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
