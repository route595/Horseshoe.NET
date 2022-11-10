using System;
using System.Net;
using System.Threading.Tasks;

namespace Horseshoe.NET.Http
{
    public static class Delete
    {
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
            Action<HttpWebResponse, ConsumerResponseEnvelope> handleResponse = null,
            TraceJournal journal = null
        )
        {
            // journaling
            if (journal == null)
            {
                journal = TraceJournal.ResetDefault();
            }
            journal.WriteEntry("Delete.AsString()");
            journal.Level++;

            // pass the buck
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
            journal.WriteEntry("Delete.AsStringAsync()");
            journal.Level++;

            // pass the buck
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
            journal.WriteEntry("Delete.AsValue()");
            journal.Level++;

            // pass the buck
            var result = Get.AsValue<E>
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
                journal: journal
            );

            // finalize
            journal.Level--;
            return result;
        }

        public async static Task<E> AsValueAsync<E>
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
            Func<string, E> responseParser = null,
            TraceJournal journal = null
        )
        {
            // journaling
            if (journal == null)
            {
                journal = TraceJournal.ResetDefault();
            }
            journal.WriteEntry("Delete.AsValueAsync()");
            journal.Level++;

            // pass the buck
            var result = await Get.AsValueAsync<E>
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
                journal: journal
            );

            // finalize
            journal.Level--;
            return result;
        }

        public static E AsJson<E>
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
            journal.WriteEntry("Delete.AsJson()");
            journal.Level++;

            // pass the buck
            var result = Get.AsJson<E>
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
            journal.WriteEntry("Delete.AsJsonAsync()");
            journal.Level++;

            // pass the buck
            var result = await Get.AsJsonAsync<E>
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
                zapBackingFields: zapBackingFields,
                journal: journal
            );

            // finalize
            journal.Level--;
            return result;
        }
    }
}
