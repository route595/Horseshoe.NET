using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

using Horseshoe.NET.Collections;
using Horseshoe.NET.RelayMessages;

namespace Horseshoe.NET.Http
{
    internal static class WebRequestFactory
    {
        public static string MessageRelayGroup => HttpConstants.MessageRelayGroup;

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
            Action<HttpWebRequest> peekRequest
        )
        {
            SystemMessageRelay.RelayMethodInfo(group: MessageRelayGroup);
            SystemMessageRelay.RelayMethodParams
            (
                new Dictionary<string, object> 
                {
                    { nameof(uri), uri },
                    { nameof(content), content },
                    { nameof(method), method },
                    { nameof(contentType), contentType }
                }
                .AppendIfNotNullRTL(nameof(securityProtocol), securityProtocol)
                .AppendIfNotNullRTL(nameof(proxyAddress), proxyAddress)
                .AppendIfNotNullRTL(nameof(proxyPort), proxyPort),
                group: MessageRelayGroup
            );

            SecurityProtocolType _securityProtocol = default;

            // prepare request for https, if applicable
            if (uri.IsHttps())
            {
                SystemMessageRelay.RelayMessage("uri.IsHttps() = true", group: MessageRelayGroup);
                if (securityProtocol.HasValue)
                {
                    _securityProtocol = ServicePointManager.SecurityProtocol;
                    SystemMessageRelay.RelayMessage("securityProtocol = " + securityProtocol, group: MessageRelayGroup);
                    ServicePointManager.SecurityProtocol = securityProtocol.Value;
                }
            }
            else
            {
                SystemMessageRelay.RelayMessage("uri.IsHttps() = false", group: MessageRelayGroup);
            }

            // build request
            SystemMessageRelay.RelayMessage("WebRequest.Create()", group: MessageRelayGroup);
            var request = (HttpWebRequest)WebRequest.Create(uri);

            // return original https setting, if applicable
            if (uri.IsHttps() && securityProtocol.HasValue)
            {
                ServicePointManager.SecurityProtocol = _securityProtocol;
            }

            // add request features
            SystemMessageRelay.RelayMessage("processing web request features...", group: MessageRelayGroup);
            request.Method = method ?? "GET";
            if (contentType != null)
            {
                request.ContentType = contentType;
            }
            SystemMessageRelay.RelayMessage(request.Method + (request.ContentType != null ? "; content type = " + request.ContentType : ""), group: MessageRelayGroup);
            if (content != null)
            {
                ProcessContent(request, content, contentSerializer);
            }

            // allow client to alter the headers
            if (alterHeaders != null)
            {
                ProcessHeaders(request, alterHeaders);
            }

            credentials = credentials ?? HttpSettings.DefaultWebServiceCredentials;
            if (credentials != null)
            {
                SystemMessageRelay.RelayMessage("attaching network credentials (" + credentials.ToDisplayString() + " )", group: MessageRelayGroup);
                request.Credentials = credentials;
            }
            if (proxyAddress.HasValue && proxyAddress.Value.Uri != null)
            {
                ProcessProxy(request, proxyAddress, proxyPort, proxyCredentials);
            }

            // allow client to further alter the request (e.g. set up NTLM authentication)
            if (peekRequest != null)
            {
                SystemMessageRelay.RelayMessage("client peekRequest()", group: MessageRelayGroup);
                try
                {
                    peekRequest.Invoke(request);
                    SystemMessageRelay.RelayMessage("done", group: MessageRelayGroup);
                }
                catch (Exception ex)
                {
                    SystemMessageRelay.RelayException(ex, group: MessageRelayGroup);
                    throw;
                }
            }

            SystemMessageRelay.RelayMethodReturn(group: MessageRelayGroup);
            return request;
        }

        private static void ProcessContent(HttpWebRequest request, object content, Func<object, string> contentSerializer)
        {
            SystemMessageRelay.RelayMethodInfo(group: MessageRelayGroup);
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
            SystemMessageRelay.RelayMethodReturn(group: MessageRelayGroup);
        }

        private static void ProcessHeaders(HttpWebRequest request, Action<WebHeaderCollection> alterHeaders)
        {
            SystemMessageRelay.RelayMethodInfo(group: MessageRelayGroup);
            alterHeaders.Invoke(request.Headers);
            SystemMessageRelay.RelayMethodReturn(group: MessageRelayGroup);
        }

        private static void ProcessProxy(HttpWebRequest request, UriString? proxyAddress, int? proxyPort, NetworkCredential proxyCredentials)
        {
            SystemMessageRelay.RelayMethodInfo(group: MessageRelayGroup);
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
            SystemMessageRelay.RelayMethodReturn("  -> " + proxyAddress + (proxyCredentials != null ? " -> " + proxyCredentials.ToDisplayString() : ""), group: MessageRelayGroup);
            request.Proxy = proxyCredentials != null 
                ? new WebProxy(proxyAddress, false, null, proxyCredentials)
                : new WebProxy(proxyAddress, false, null);
            SystemMessageRelay.RelayMethodReturn(group: MessageRelayGroup);
        }
    }
}
