using System;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace Horseshoe.NET.Http
{
    internal static class WebRequestFactory
    {
        internal static HttpWebRequest GetWebRequest
        (
            UriString uri,
            object content,
            string method,
            string contentType,
            Func<object, string> contentSerializer,
            SecurityProtocolType? securityProtocol,
            Action<WebHeaderCollection> alterHeaders,
            NetworkCredential credentials,
            UriString? proxyAddress,
            int? proxyPort,
            NetworkCredential proxyCredentials,
            Action<HttpWebRequest> alterRequest,
            TraceJournal journal
        )
        {
            SecurityProtocolType _securityProtocol = default;
            bool revertSecurityProtocolTypeToOriginalValue = false;

            // prepare request for https, if applicable
            if (uri.IsHttps() && securityProtocol.HasValue)
            {
                journal.WriteEntry("uri.IsHttps() => true");
                _securityProtocol = ServicePointManager.SecurityProtocol;
                ServicePointManager.SecurityProtocol = securityProtocol.Value;
                revertSecurityProtocolTypeToOriginalValue = true;
                if (securityProtocol.HasValue)
                {
                    journal.WriteEntry("  -> (" + securityProtocol.Value + ")");
                }
            }
            else
            {
                journal.WriteEntry("uri.IsHttps() => false");
            }

            // build request
            journal.WriteEntry("WebRequest.Create()");
            var request = (HttpWebRequest)WebRequest.Create(uri);

            // return original https setting, if applicable
            if (revertSecurityProtocolTypeToOriginalValue)
            {
                ServicePointManager.SecurityProtocol = _securityProtocol;
            }

            // add request features
            var strb = new StringBuilder();
            journal.WriteEntry("processing web request features...");
            request.Method = method ?? "GET";
            strb.Append(request.Method);
            if (contentType != null)
            {
                request.ContentType = contentType;
                strb.Append(" > " + contentType);
            }
            journal.WriteEntry(strb.ToString());
            if (content != null)
            {
                ProcessContent(request, content, contentSerializer, journal);
            }

            // allow client to alter the headers
            if (alterHeaders != null)
            {
                ProcessHeaders(request, alterHeaders, journal);
            }

            credentials = credentials ?? HttpSettings.DefaultWebServiceCredentials;
            if (credentials != null)
            {
                journal.WriteEntry("add network credentials (" + credentials.ToDisplayString() + " )");
                request.Credentials = credentials;
            }
            if (proxyAddress.HasValue && proxyAddress.Value.Uri != null)
            {
                ProcessProxy(request, proxyAddress, proxyPort, proxyCredentials, journal);
            }

            // allow client to further alter the request (e.g. set up NTLM authentication)
            if (alterRequest != null)
            {
                journal.WriteEntry("alterRequest.Invoke()");
                try
                {
                    alterRequest.Invoke(request);
                    journal.WriteEntry("  -> done");
                }
                catch (Exception ex)
                {
                    journal.WriteEntry("  -> error");
                    journal.WriteEntry("     " + ex.Message + " (" + ex.GetType().FullName + ")");
                    throw;
                }
            }

            // return completed request
            return request;
        }

        private static void ProcessContent(HttpWebRequest request, object content, Func<object, string> contentSerializer, TraceJournal journal)
        {
            journal.WriteEntry("ProcessContent()");
            string _content;
            if (contentSerializer != null)
            {
                _content = contentSerializer.Invoke(content);
            }
            else
            {
                _content = content.ToString();
            }
            using (var streamWriter = new StreamWriter(request.GetRequestStream()))
            {
                streamWriter.Write(_content);
                streamWriter.Flush();
                streamWriter.Close();
            }
        }

        private static void ProcessHeaders(HttpWebRequest request, Action<WebHeaderCollection> alterHeaders, TraceJournal journal)
        {
            journal.WriteEntry("ProcessHeaders()");
            alterHeaders.Invoke(request.Headers);
        }

        private static void ProcessProxy(HttpWebRequest request, UriString? proxyAddress, int? proxyPort, NetworkCredential proxyCredentials, TraceJournal journal)
        {
            journal.WriteEntry("ProcessProxy()");
            if (proxyPort > 0)
            {
                var hasPortMatch = Regex.Match(proxyAddress.ToString(), "[:]\\d+$");
                if (!hasPortMatch.Success)  // don't override a hardcoded port
                {
                    proxyAddress = proxyAddress.ToString().TrimEnd('/') + ":" + proxyPort;
                }
            }
            // handle in-URL credentials, e.g. http://username:password@proxyhost:port/
            var match = Regex.Match(proxyAddress.ToString(), "(?<=http[s]?://)\\w+[:].+(?=[@])", RegexOptions.IgnoreCase);
            if (match.Success)
            {
                proxyAddress = proxyAddress.ToString().Replace(match.Value + "@", "");
                var split = match.Value.Split(':');
                proxyCredentials = new Credential(split[0], split[1]);
            }
            journal.WriteEntry("  -> " + proxyAddress + (proxyCredentials != null ? " -> " + proxyCredentials.ToDisplayString() : ""));
            request.Proxy = proxyCredentials != null 
                ? new WebProxy(proxyAddress, false, null, proxyCredentials)
                : new WebProxy(proxyAddress, false, null);
        }
    }
}
