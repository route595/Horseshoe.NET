using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;

using Horseshoe.NET.Collections;
using Horseshoe.NET.Crypto;
using Horseshoe.NET.Text;

namespace Horseshoe.NET.Http
{
    public static class Extensions
    {
        public static IDictionary<string, string> ToDictionary(this WebHeaderCollection collection)
        {
            var dict = new Dictionary<string, string>();
            foreach (var key in collection.AllKeys)
            {
                var values = collection[key];
                dict.Add(key, values);
            }
            return dict;
        }

        public static IDictionary<string, string[]> ToOwinDictionary(this WebHeaderCollection collection)
        {
            var dict = new Dictionary<string, string[]>();
            foreach (var key in collection.AllKeys)
            {
                var values = collection.GetValues(key);
                dict.Add(key, values);
            }
            return dict;
        }

        public static bool IsHttps(this Uri uri)
        {
            return string.Equals(uri.Scheme, "https", StringComparison.OrdinalIgnoreCase);
        }

        public static string GetContentAsString(this WebResponse response)
        {
            var stream = response.GetResponseStream();
            string stringResult;
            using (var reader = new StreamReader(stream))
            {
                stringResult = reader.ReadToEnd();
            }
            return stringResult;
        }

        public static async Task<string> GetContentAsStringAsync(this WebResponse response)
        {
            var stream = response.GetResponseStream();
            string stringResult;
            using (var reader = new StreamReader(stream))
            {
                stringResult = await reader.ReadToEndAsync();
            }
            return stringResult;
        }

        public static string GetResponseContentAsString(this WebException wex)
        {
            return GetContentAsString(wex.Response);
        }

        public static async Task<string> GetResponseContentAsStringAsync(this WebException wex)
        {
            return await GetContentAsStringAsync(wex.Response);
        }

        public static byte[] GetContentAsBytes(this WebResponse response, int bufferSize = 4096)
        {
            var list = new List<byte>();
            var buf = new byte[bufferSize];
            int numBytesRead = 0;
            int timesZeroBytesRead = -1;
            using (var stream = response.GetResponseStream())
            {
                numBytesRead = stream.Read(buf, 0, bufferSize);
                while (numBytesRead > -1)
                {
                    if (numBytesRead == bufferSize)
                    {
                        list.AddRange(buf);
                    }
                    else if (numBytesRead == 0)
                    {
                        if (++timesZeroBytesRead == 20)
                            throw new Exception("Repeatedly read 0 bytes from stream");
                        continue;
                    }
                    else
                    {
                        var shortBuf = new byte[numBytesRead];
                        Array.Copy(buf, shortBuf, numBytesRead);
                        list.AddRange(shortBuf);
                    }
                    numBytesRead = stream.Read(buf, 0, bufferSize);
                }
            }
            return list.ToArray();
        }

        public static async Task<byte[]> GetContentAsBytesAsync(this WebResponse response, int bufferSize = 4096)
        {
            var list = new List<byte>();
            var buf = new byte[bufferSize];
            int numBytesRead = 0;
            int timesZeroBytesRead = -1;
            using (var stream = response.GetResponseStream())
            {
                numBytesRead = await stream.ReadAsync(buf, 0, bufferSize);
                while (numBytesRead > -1)
                {
                    if (numBytesRead == bufferSize)
                    {
                        list.AddRange(buf);
                    }
                    else if (numBytesRead == 0)
                    {
                        if (++timesZeroBytesRead == 20)
                            throw new Exception("Repeatedly read 0 bytes from stream");
                        continue;
                    }
                    else
                    {
                        var shortBuf = new byte[numBytesRead];
                        Array.Copy(buf, shortBuf, numBytesRead);
                        list.AddRange(shortBuf);
                    }
                    numBytesRead = stream.Read(buf, 0, bufferSize);
                }
            }
            return list.ToArray();
        }

        public static HttpStatusCode GetHttpStatusCode(this WebException wex)
        {
            return (wex.Response as HttpWebResponse)?.StatusCode ?? HttpStatusCode.NotImplemented;
        }

        public static string GetBodyAsString(this HttpRequestBase request)
        {
            using (var reader = new StreamReader(request.InputStream))
            {
                return reader.ReadToEnd();
            }
        }

        public static string GetOriginalRequestBody(this HttpRequestBase request)
        {
            using (var stream = new MemoryStream())
            {
                request.InputStream.CopyTo(stream);
                var rawBody = (request.ContentEncoding ?? Encoding.UTF8).GetString(stream.ToArray());
                return rawBody;
            }
        }

        public static string GetAbsoluteApplicationPath(this HttpRequestBase request, string virtualSubpath = null, bool includeQueryString = false, string overrideScheme = null, string overrideHost = null, int? overridePort = null, string overrideApplicationPath = null)
        {
            var uri = request.Url;
            var sb = new StringBuilder(overrideScheme ?? uri.Scheme)          // http
                .Append("://")
                .Append(overrideHost ?? uri.Host)                             // dev-web01.dev.local
                .AppendIf
                (
                    overridePort.HasValue,
                    ":" + overridePort                                        // :8080
                )
                .AppendIf
                (
                    !overridePort.HasValue && uri.Port != 0 && !((overrideScheme ?? uri.Scheme).ToLower() + uri.Port).In("http80", "https443"),
                    ":" + uri.Port                                            // :8080
                )
                .Append(overrideApplicationPath ?? request.ApplicationPath);  // /test_props

            if (virtualSubpath != null)                                       // /api
            {
                if (!sb.ToString().EndsWith("/"))
                {
                    sb.Append("/");
                }
                if (virtualSubpath.StartsWith("/"))
                {
                    sb.Append(virtualSubpath.Substring(1));
                }
                else
                {
                    sb.Append(virtualSubpath);
                }
            }

            if (includeQueryString && uri.Query.Length > 0)
            {
                if (virtualSubpath == null && (overrideApplicationPath ?? request.ApplicationPath).Length == 0)
                {
                    sb.Append("/");
                }
                sb.Append(uri.Query);
            }

            return sb.ToString();
        }

        public static string GetRemoteIPAddress(this HttpContext httpContext)
        {
            return GetRemoteIPAddress(httpContext.Request);
        }

        public static string GetRemoteIPAddress(this HttpRequest request)
        {
            return Zap.String(request.UserHostAddress);
        }

        public static string GetRemoteIPAddress(this HttpRequestBase request)
        {
            return Zap.String(request.UserHostAddress);
        }

        public static string GetRemoteMachineName(this HttpContext httpContext)
        {
            return GetRemoteMachineName(httpContext.Request);
        }

        public static string GetRemoteMachineName(this HttpRequest request)
        {
            if (string.Equals(request.UserHostName, GetRemoteIPAddress(request))) return null;
            return Zap.String(request.UserHostName);
        }

        public static string GetRemoteMachineName(this HttpRequestBase request)
        {
            if (string.Equals(request.UserHostName, GetRemoteIPAddress(request))) return null;
            return Zap.String(request.UserHostName);
        }

        public static string GetUserName(this HttpContext httpContext)
        {
            return Zap.String(httpContext.User?.Identity.Name) ?? Zap.String(httpContext.Request?.ServerVariables["UNMAPPED_REMOTE_USER"]);
        }

        // see https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/Expires
        private static string ConvertToHttpHeaderDate(DateTime dateTime)
        {
            dateTime = dateTime.Kind == DateTimeKind.Utc
                ? dateTime
                : dateTime.ToUniversalTime();
            return dateTime.ToString("ddd, dd MMM yyyy HH:mm:ss 'GMT'");
        }

        /// <summary>
        /// Sets up NTLM authentication using previously supplied URI and credentials.  An exception will occur if the domain is not specified (can also optionally be set in credential constructor or HttpSettings.DefaultDomain).
        /// </summary>
        /// <param name="request"></param>
        /// <param name="domain"></param>
        public static void UseNtlm(this HttpWebRequest request, string domain = null)
        {
            // validation
            if (request == null)
                throw new ValidationException("request cannot be null");  // this should never happen
            if (request.Credentials == null)
                throw new ValidationException("credentials not supplied");
            if (request.Credentials is NetworkCredential networkCredentials)
            {
                if (networkCredentials.Domain == null) 
                {
                    if (domain == null && HttpSettings.DefaultDomain == null)
                        throw new ValidationException("domain not supplied -- try UseNtlm(domain: \"mydomain\") or credential constructor or HttpSettings.DefaultDomain");
                    networkCredentials = new NetworkCredential(networkCredentials.UserName, networkCredentials.Password, domain ?? HttpSettings.DefaultDomain);
                }
            }
            else
            {
                throw new ValidationException("unsupported credential type: " + request.Credentials.GetType());  // this should never happen
            }

            // enable NTLM
            var credCache = new CredentialCache
            {
                { request.RequestUri, "NTLM", networkCredentials }
            };
            request.Credentials = credCache;
        }

        /// <summary>
        /// Specify the MIME types that are acceptable for the response
        /// </summary>
        /// <param name="hdrs"></param>
        /// <param name="mimeTypes">A comma separated list of MIME types</param>
        public static void AddAccept(this WebHeaderCollection hdrs, string mimeTypes)
        {
            hdrs.Add(HttpRequestHeader.Accept, mimeTypes);
        }

        /// <summary>
        /// Specify the MIME types that are acceptable for the response
        /// </summary>
        /// <param name="hdrs"></param>
        /// <param name="mimeTypes">An array of MIME types</param>
        public static void AddAccept(this WebHeaderCollection hdrs, params string[] mimeTypes)
        {
            AddAccept(hdrs, string.Join(", ", mimeTypes ?? Array.Empty<string>()));
        }

        /// <summary>
        /// Specify the set of HTTP methods supported
        /// </summary>
        /// <param name="response"></param>
        /// <param name="methods">A comma separated list of HTTP methods</param>
        public static void AddHeader_Allow(this HttpResponseBase response, string methods)
        {
            response.Headers.Add("Allow", methods);
        }

        /// <summary>
        /// Specify the set of HTTP methods supported
        /// </summary>
        /// <param name="response"></param>
        /// <param name="methods">An array of HTTP methods</param>
        public static void AddHeader_Allow(this HttpResponseBase response, params string[] methods)
        {
            AddHeader_Allow(response, string.Join(", ", methods ?? Array.Empty<string>()));
        }

        /// <summary>
        /// Present the client's credentials in order to authenticate it to the server
        /// </summary>
        /// <param name="hdrs"></param>
        /// <param name="authorization"></param>
        public static void AddAuthorization(this WebHeaderCollection hdrs, string authorization)
        {
            if (authorization == null)
                return;
            hdrs.Add(HttpRequestHeader.Authorization, authorization);
        }

        /// <summary>
        /// Present the client's credentials in order to authenticate it to the server
        /// </summary>
        /// <param name="hdrs"></param>
        /// <param name="basicAuthorization">Base64 encoded username:password</param>
        public static void AddBasicAuthorization(this WebHeaderCollection hdrs, string basicAuthorization)
        {
            if (basicAuthorization == null)
                return;
            AddAuthorization(hdrs, "Basic " + basicAuthorization);
        }

        /// <summary>
        /// Present the client's credentials in order to authenticate it to the server
        /// </summary>
        /// <param name="hdrs"></param>
        /// <param name="userName">a user name</param>
        /// <param name="password">a password</param>
        public static void AddBasicAuthorization(this WebHeaderCollection hdrs, string userName, string password)
        {
            if (userName == null || password == null)
                return;
            AddBasicAuthorization(hdrs, Encode.Base64(userName + ":" + password));
        }

        /// <summary>
        /// Specify the directives that must be obeyed by all cache control mechanisms along the request/response chain
        /// </summary>
        /// <param name="hdrs"></param>
        /// <param name="cacheControl"></param>
        public static void AddCacheControl(this WebHeaderCollection hdrs, string cacheControl)
        {
            hdrs.Add(HttpRequestHeader.CacheControl, cacheControl);
        }

        /// <summary>
        /// Specify the directives that must be obeyed by all cache control mechanisms along the request/response chain
        /// </summary>
        /// <param name="hdrs"></param>
        /// <param name="maxAge"></param>
        /// <param name="maxStale"></param>
        /// <param name="minFresh"></param>
        /// <param name="noCache"></param>
        /// <param name="noStore"></param>
        /// <param name="noTransform"></param>
        /// <param name="onlyIfCached"></param>
        /// <param name="staleIfError"></param>
        public static void AddCacheControl(this WebHeaderCollection hdrs, int? maxAge = null, int? maxStale = null, int? minFresh = null, bool noCache = false, bool noStore = false, bool noTransform = false, bool onlyIfCached = false, int? staleIfError = null)
        {
            var list = new List<string>();
            if (maxAge.HasValue) list.Add("max-age=" + maxAge);
            if (maxStale.HasValue) list.Add("max-stale=" + maxStale);
            if (minFresh.HasValue) list.Add("min-fresh=" + minFresh);
            if (noCache) list.Add("no-cache");
            if (noStore) list.Add("no-store");
            if (noTransform) list.Add("no-transform");
            if (onlyIfCached) list.Add("only-if-cached");
            if (staleIfError.HasValue) list.Add("stale-if-error=" + staleIfError);
            hdrs.Add(HttpRequestHeader.CacheControl, string.Join(",", list));
        }

        /// <summary>
        /// Specify the directives that must be obeyed by all cache control mechanisms along the request/response chain
        /// </summary>
        /// <param name="hdrs"></param>
        public static void AddCacheControl_MaxAge1Year(this WebHeaderCollection hdrs)
        {
            AddCacheControl(hdrs, maxAge: 31536000);
        }

        /// <summary>
        /// Specify the directives that must be obeyed by all cache control mechanisms along the request/response chain
        /// </summary>
        /// <param name="hdrs"></param>
        public static void AddCacheControl_NoStore(this WebHeaderCollection hdrs)
        {
            AddCacheControl(hdrs, noStore: true);
        }

        /// <summary>
        /// Specify the directives that must be obeyed by all cache control mechanisms along the request/response chain
        /// </summary>
        /// <param name="response"></param>
        /// <param name="cacheControl"></param>
        public static void AddHeader_CacheControl(this HttpResponseBase response, string cacheControl)
        {
            response.Headers.Add("Cache-Control", cacheControl);
        }

        /// <summary>
        /// Specify the directives that must be obeyed by all cache control mechanisms along the request/response chain
        /// </summary>
        /// <param name="response"></param>
        /// <param name="maxAge"></param>
        /// <param name="sMaxAge"></param>
        /// <param name="noCache"></param>
        /// <param name="noStore"></param>
        /// <param name="noTransform"></param>
        /// <param name="mustRevalidate"></param>
        /// <param name="proxyRevalidate"></param>
        /// <param name="mustUnderstand"></param>
        /// <param name="private"></param>
        /// <param name="public"></param>
        /// <param name="immutable"></param>
        /// <param name="staleWhileRevalidate"></param>
        /// <param name="staleIfError"></param>
        public static void AddHeader_CacheControl(this HttpResponseBase response, int? maxAge = null, int? sMaxAge = null, bool noCache = false, bool noStore = false, bool noTransform = false, bool mustRevalidate = false, bool proxyRevalidate = false, bool mustUnderstand = false, bool @private = false, bool @public = false, bool immutable = false, int? staleWhileRevalidate = null, int? staleIfError = null)
        {
            var list = new List<string>();
            if (maxAge.HasValue) list.Add("max-age=" + maxAge);
            if (sMaxAge.HasValue) list.Add("s-maxage=" + sMaxAge);
            if (noCache) list.Add("no-cache");
            if (noStore) list.Add("no-store");
            if (noTransform) list.Add("no-transform");
            if (mustRevalidate) list.Add("must-revalidate");
            if (proxyRevalidate) list.Add("proxy-revalidate");
            if (mustUnderstand) list.Add("must-understand");
            if (@private) list.Add("private");
            if (@public) list.Add("public");
            if (immutable) list.Add("immutable");
            if (staleWhileRevalidate.HasValue) list.Add("stale-while-revalidate=" + staleWhileRevalidate);
            if (staleIfError.HasValue) list.Add("stale-if-error=" + staleIfError);
            AddHeader_CacheControl(response, string.Join(",", list));
        }

        /// <summary>
        /// Specify the directives that must be obeyed by all cache control mechanisms along the request/response chain
        /// </summary>
        /// <param name="response"></param>
        public static void AddHeader_CacheControl_MaxAge1Year(this HttpResponseBase response)
        {
            AddHeader_CacheControl(response, maxAge: 31536000);
        }

        /// <summary>
        /// Specify the directives that must be obeyed by all cache control mechanisms along the request/response chain
        /// </summary>
        /// <param name="response"></param>
        public static void AddHeader_CacheControl_NoStore(this HttpResponseBase response)
        {
            AddHeader_CacheControl(response, noStore: true);
        }

        /// <summary>
        /// Specifiy the MIME type of the accompanying body of data (alternative to setting the HTTP request content type)
        /// </summary>
        /// <param name="hdrs"></param>
        /// <param name="contentType">A MIME type</param>
        public static void AddContentType(this WebHeaderCollection hdrs, string contentType)
        {
            hdrs.Add(HttpRequestHeader.ContentType, contentType);
        }

        /// <summary>
        /// Specifiy the MIME type of the accompanying body data (alternative to setting the HTTP response content type)
        /// </summary>
        /// <param name="response"></param>
        /// <param name="contentType">A MIME type</param>
        public static void AddHeader_ContentType(this HttpResponseBase response, string contentType)
        {
            response.Headers.Add("Content-Type", contentType);
        }

        /// <summary>
        /// Specify the date and time after which the accompanying body data should be considered stale
        /// </summary>
        /// <param name="response"></param>
        /// <param name="expires">A date/time formatted thus: Wed, 21 Oct 2015 07:28:00 GMT (only use GMT)</param>
        public static void AddHeader_Expires(this HttpResponseBase response, string expires)
        {
            response.Headers.Add("Expires", expires);
        }

        /// <summary>
        /// Specify the date/time after which the accompanying body data should be considered stale.  This method converts date/times to UTC (GMT) taking the <c>DateTimeKind</c> into account.
        /// </summary>
        /// <param name="response"></param>
        /// <param name="expires">A date/time</param>
        public static void AddHeader_Expires(this HttpResponseBase response, DateTime expires)
        {
            AddHeader_Expires(response, ConvertToHttpHeaderDate(expires));
        }

        /// <summary>
        /// Specify the date/time at which the accompanying body data was last modified
        /// </summary>
        /// <param name="response"></param>
        /// <param name="lastModified">A date/time formatted thus: Wed, 21 Oct 2015 07:28:00 GMT (only use GMT)</param>
        public static void AddHeader_LastModified(this HttpResponseBase response, string lastModified)
        {
            response.Headers.Add("Last-Modified", lastModified);
        }

        /// <summary>
        /// Specify the date/time at which the accompanying body data was last modified.  This method converts date/times to UTC (GMT) taking the <c>DateTimeKind</c> into account.
        /// </summary>
        /// <param name="response"></param>
        /// <param name="lastModified">A date/time</param>
        public static void AddHeader_LastModified(this HttpResponseBase response, DateTime lastModified)
        {
            AddHeader_LastModified(response, ConvertToHttpHeaderDate(lastModified));
        }

        /// <summary>
        /// Specify a URI to which the client is redirected to obtain the requested resource
        /// </summary>
        /// <param name="response"></param>
        /// <param name="location">A URI</param>
        public static void AddHeader_Location(this HttpResponseBase response, string location)
        {
            response.Headers.Add("Location", location);
        }

        /// <summary>
        /// Specify implementation-specific directives that might apply to any agent along the request/response chain
        /// </summary>
        /// <param name="hdrs"></param>
        /// <param name="pragma"></param>
        [Obsolete("This feature is no longer recommended. Though some browsers might still support it, it may have already been removed from the relevant web standards, may be in the process of being dropped, or may only be kept for compatibility purposes. Avoid using it, and update existing code if possible.", false)]
        public static void AddPragma(this WebHeaderCollection hdrs, string pragma)
        {
            hdrs.Add(HttpRequestHeader.Pragma, pragma);
        }

        /// <summary>
        /// Specify implementation-specific directives that might apply to any agent along the request/response chain
        /// </summary>
        /// <param name="response"></param>
        /// <param name="pragma"></param>
        [Obsolete("This feature is no longer recommended. Though some browsers might still support it, it may have already been removed from the relevant web standards, may be in the process of being dropped, or may only be kept for compatibility purposes. Avoid using it, and update existing code if possible.", false)]
        public static void AddHeader_Pragma(this HttpResponseBase response, string pragma)
        {
            response.Headers.Add("Pragma", pragma);
        }

        /// <summary>
        /// Specify the credentials that the client presents in order to authenticate itself to the proxy (alternative to setting the proxy on the request)
        /// </summary>
        /// <param name="hdrs"></param>
        /// <param name="authorization"></param>
        public static void AddProxyAuthorization(this WebHeaderCollection hdrs, string authorization)
        {
            hdrs.Add(HttpRequestHeader.ProxyAuthorization, authorization);
        }

        /// <summary>
        /// Specify the credentials that the client presents in order to authenticate itself to the proxy (alternative to setting the proxy on the request)
        /// </summary>
        /// <param name="hdrs"></param>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        public static void AddBasicProxyAuthorization(this WebHeaderCollection hdrs, string userName, string password)
        {
            AddProxyAuthorization(hdrs, "Basic " + Encode.Base64(userName + ":" + password));
        }

        /// <summary>
        /// Specify the URI of the resource from which the request URI was obtained
        /// </summary>
        /// <param name="hdrs"></param>
        /// <param name="referrer"></param>
        public static void AddReferer(this WebHeaderCollection hdrs, string referrer)
        {
            hdrs.Add(HttpRequestHeader.Referer, referrer);
        }

        /// <summary>
        /// Specify information about the client agent
        /// </summary>
        /// <param name="hdrs"></param>
        /// <param name="userAgent"></param>
        public static void AddUserAgent(this WebHeaderCollection hdrs, string userAgent)
        {
            hdrs.Add(HttpRequestHeader.UserAgent, userAgent);
        }

        /// <summary>
        /// Specify that the client must authenticate itself to the server
        /// </summary>
        /// <param name="response"></param>
        /// <param name="authenticate"></param>
        public static void AddHeader_WwwAuthenticate(this HttpResponseBase response, string authenticate)
        {
            response.Headers.Add("WWW-Authenticate", authenticate);
        }

        /// <summary>
        /// Specify the MIME types that are acceptable for the response
        /// </summary>
        /// <param name="hdrs"></param>
        /// <param name="mimeTypes">A comma separated list of MIME types</param>
        public static void SetAccept(this WebHeaderCollection hdrs, string mimeTypes)
        {
            hdrs.Set(HttpRequestHeader.Accept, mimeTypes);
        }

        /// <summary>
        /// Specify the MIME types that are acceptable for the response
        /// </summary>
        /// <param name="hdrs"></param>
        /// <param name="mimeTypes">An array of MIME types</param>
        public static void SetAccept(this WebHeaderCollection hdrs, params string[] mimeTypes)
        {
            SetAccept(hdrs, string.Join(", ", mimeTypes ?? Array.Empty<string>()));
        }

        /// <summary>
        /// Specify the set of HTTP methods supported
        /// </summary>
        /// <param name="response"></param>
        /// <param name="methods">A comma separated list of HTTP methods</param>
        public static void SetHeader_Allow(this HttpResponseBase response, string methods)
        {
            response.Headers.Set("Allow", methods);
        }

        /// <summary>
        /// Specify the set of HTTP methods supported
        /// </summary>
        /// <param name="response"></param>
        /// <param name="methods">An array of HTTP methods</param>
        public static void SetHeader_Allow(this HttpResponseBase response, params string[] methods)
        {
            SetHeader_Allow(response, string.Join(", ", methods ?? Array.Empty<string>()));
        }

        /// <summary>
        /// Present the client's credentials in order to authenticate it to the server
        /// </summary>
        /// <param name="hdrs"></param>
        /// <param name="authorization"></param>
        public static void SetAuthorization(this WebHeaderCollection hdrs, string authorization)
        {
            hdrs.Set(HttpRequestHeader.Authorization, authorization);
        }

        /// <summary>
        /// Present the client's credentials in order to authenticate it to the server
        /// </summary>
        /// <param name="hdrs"></param>
        /// <param name="basicAuthorization">Base64 encoded username:password</param>
        public static void SetBasicAuthorization(this WebHeaderCollection hdrs, string basicAuthorization)
        {
            if (basicAuthorization == null)
                return;
            SetAuthorization(hdrs, "Basic " + basicAuthorization);
        }

        /// <summary>
        /// Present the client's credentials in order to authenticate it to the server
        /// </summary>
        /// <param name="hdrs"></param>
        /// <param name="userName">a user name</param>
        /// <param name="password">a password</param>
        public static void SetBasicAuthorization(this WebHeaderCollection hdrs, string userName, string password)
        {
            if (userName == null || password == null)
                return;
            SetBasicAuthorization(hdrs, Encode.Base64(userName + ":" + password));
        }

        /// <summary>
        /// Specify the directives that must be obeyed by all cache control mechanisms along the request/response chain
        /// </summary>
        /// <param name="hdrs"></param>
        /// <param name="cacheControl"></param>
        public static void SetCacheControl(this WebHeaderCollection hdrs, string cacheControl)
        {
            hdrs.Set(HttpRequestHeader.CacheControl, cacheControl);
        }

        /// <summary>
        /// Specify the directives that must be obeyed by all cache control mechanisms along the request/response chain
        /// </summary>
        /// <param name="hdrs"></param>
        /// <param name="maxAge"></param>
        /// <param name="maxStale"></param>
        /// <param name="minFresh"></param>
        /// <param name="noCache"></param>
        /// <param name="noStore"></param>
        /// <param name="noTransform"></param>
        /// <param name="onlyIfCached"></param>
        /// <param name="staleIfError"></param>
        public static void SetCacheControl(this WebHeaderCollection hdrs, int? maxAge = null, int? maxStale = null, int? minFresh = null, bool noCache = false, bool noStore = false, bool noTransform = false, bool onlyIfCached = false, int? staleIfError = null)
        {
            var list = new List<string>();
            if (maxAge.HasValue) list.Add("max-age=" + maxAge);
            if (maxStale.HasValue) list.Add("max-stale=" + maxStale);
            if (minFresh.HasValue) list.Add("min-fresh=" + minFresh);
            if (noCache) list.Add("no-cache");
            if (noStore) list.Add("no-store");
            if (noTransform) list.Add("no-transform");
            if (onlyIfCached) list.Add("only-if-cached");
            if (staleIfError.HasValue) list.Add("stale-if-error=" + staleIfError);
            hdrs.Set(HttpRequestHeader.CacheControl, string.Join(",", list));
        }

        /// <summary>
        /// Specify the directives that must be obeyed by all cache control mechanisms along the request/response chain
        /// </summary>
        /// <param name="hdrs"></param>
        public static void SetCacheControl_MaxAge1Year(this WebHeaderCollection hdrs)
        {
            SetCacheControl(hdrs, maxAge: 31536000);
        }

        /// <summary>
        /// Specify the directives that must be obeyed by all cache control mechanisms along the request/response chain
        /// </summary>
        /// <param name="hdrs"></param>
        public static void SetCacheControl_NoStore(this WebHeaderCollection hdrs)
        {
            SetCacheControl(hdrs, noStore: true);
        }

        /// <summary>
        /// Specify the directives that must be obeyed by all cache control mechanisms along the request/response chain
        /// </summary>
        /// <param name="response"></param>
        /// <param name="cacheControl"></param>
        public static void SetHeader_CacheControl(this HttpResponseBase response, string cacheControl)
        {
            response.Headers.Set("Cache-Control", cacheControl);
        }

        /// <summary>
        /// Specify the directives that must be obeyed by all cache control mechanisms along the request/response chain
        /// </summary>
        /// <param name="response"></param>
        /// <param name="maxAge"></param>
        /// <param name="sMaxAge"></param>
        /// <param name="noCache"></param>
        /// <param name="noStore"></param>
        /// <param name="noTransform"></param>
        /// <param name="mustRevalidate"></param>
        /// <param name="proxyRevalidate"></param>
        /// <param name="mustUnderstand"></param>
        /// <param name="private"></param>
        /// <param name="public"></param>
        /// <param name="immutable"></param>
        /// <param name="staleWhileRevalidate"></param>
        /// <param name="staleIfError"></param>
        public static void SetHeader_CacheControl(this HttpResponseBase response, int? maxAge = null, int? sMaxAge = null, bool noCache = false, bool noStore = false, bool noTransform = false, bool mustRevalidate = false, bool proxyRevalidate = false, bool mustUnderstand = false, bool @private = false, bool @public = false, bool immutable = false, int? staleWhileRevalidate = null, int? staleIfError = null)
        {
            var list = new List<string>();
            if (maxAge.HasValue) list.Add("max-age=" + maxAge);
            if (sMaxAge.HasValue) list.Add("s-maxage=" + sMaxAge);
            if (noCache) list.Add("no-cache");
            if (noStore) list.Add("no-store");
            if (noTransform) list.Add("no-transform");
            if (mustRevalidate) list.Add("must-revalidate");
            if (proxyRevalidate) list.Add("proxy-revalidate");
            if (mustUnderstand) list.Add("must-understand");
            if (@private) list.Add("private");
            if (@public) list.Add("public");
            if (immutable) list.Add("immutable");
            if (staleWhileRevalidate.HasValue) list.Add("stale-while-revalidate=" + staleWhileRevalidate);
            if (staleIfError.HasValue) list.Add("stale-if-error=" + staleIfError);
            SetHeader_CacheControl(response, string.Join(",", list));
        }

        /// <summary>
        /// Specify the directives that must be obeyed by all cache control mechanisms along the request/response chain
        /// </summary>
        /// <param name="response"></param>
        public static void SetHeader_CacheControl_MaxAge1Year(this HttpResponseBase response)
        {
            SetHeader_CacheControl(response, maxAge: 31536000);
        }

        /// <summary>
        /// Specify the directives that must be obeyed by all cache control mechanisms along the request/response chain
        /// </summary>
        /// <param name="response"></param>
        public static void SetHeader_CacheControl_NoStore(this HttpResponseBase response)
        {
            SetHeader_CacheControl(response, noStore: true);
        }

        /// <summary>
        /// Specifiy the MIME type of the accompanying body of data (alternative to setting the HTTP request content type)
        /// </summary>
        /// <param name="hdrs"></param>
        /// <param name="contentType">A MIME type</param>
        public static void SetContentType(this WebHeaderCollection hdrs, string contentType)
        {
            hdrs.Set(HttpRequestHeader.ContentType, contentType);
        }

        /// <summary>
        /// Specifiy the MIME type of the accompanying body data (alternative to setting the HTTP response content type)
        /// </summary>
        /// <param name="response"></param>
        /// <param name="contentType">A MIME type</param>
        public static void SetHeader_ContentType(this HttpResponseBase response, string contentType)
        {
            response.Headers.Set("Content-Type", contentType);
        }

        /// <summary>
        /// Specify the date and time after which the accompanying body data should be considered stale
        /// </summary>
        /// <param name="response"></param>
        /// <param name="expires">A date/time formatted thus: Wed, 21 Oct 2015 07:28:00 GMT (only use GMT)</param>
        public static void SetHeader_Expires(this HttpResponseBase response, string expires)
        {
            response.Headers.Set("Expires", expires);
        }

        /// <summary>
        /// Specify the date/time after which the accompanying body data should be considered stale.  This method converts date/times to UTC (GMT) taking the <c>DateTimeKind</c> into account.
        /// </summary>
        /// <param name="response"></param>
        /// <param name="expires">A date/time</param>
        public static void SetHeader_Expires(this HttpResponseBase response, DateTime expires)
        {
            SetHeader_Expires(response, ConvertToHttpHeaderDate(expires));
        }

        /// <summary>
        /// Specify the date/time at which the accompanying body data was last modified
        /// </summary>
        /// <param name="response"></param>
        /// <param name="lastModified">A date/time formatted thus: Wed, 21 Oct 2015 07:28:00 GMT (only use GMT)</param>
        public static void SetHeader_LastModified(this HttpResponseBase response, string lastModified)
        {
            response.Headers.Set("Last-Modified", lastModified);
        }

        /// <summary>
        /// Specify the date/time at which the accompanying body data was last modified.  This method converts date/times to UTC (GMT) taking the <c>DateTimeKind</c> into account.
        /// </summary>
        /// <param name="response"></param>
        /// <param name="lastModified">A date/time</param>
        public static void SetHeader_LastModified(this HttpResponseBase response, DateTime lastModified)
        {
            SetHeader_LastModified(response, ConvertToHttpHeaderDate(lastModified));
        }

        /// <summary>
        /// Specify a URI to which the client is redirected to obtain the requested resource
        /// </summary>
        /// <param name="response"></param>
        /// <param name="location">A URI</param>
        public static void SetHeader_Location(this HttpResponseBase response, string location)
        {
            response.Headers.Set("Location", location);
        }

        /// <summary>
        /// Specify implementation-specific directives that might apply to any agent along the request/response chain
        /// </summary>
        /// <param name="hdrs"></param>
        /// <param name="pragma"></param>
        [Obsolete("This feature is no longer recommended. Though some browsers might still support it, it may have already been removed from the relevant web standards, may be in the process of being dropped, or may only be kept for compatibility purposes. Avoid using it, and update existing code if possible.", false)]
        public static void SetPragma(this WebHeaderCollection hdrs, string pragma)
        {
            hdrs.Set(HttpRequestHeader.Pragma, pragma);
        }

        /// <summary>
        /// Specify implementation-specific directives that might apply to any agent along the request/response chain
        /// </summary>
        /// <param name="response"></param>
        /// <param name="pragma"></param>
        [Obsolete("This feature is no longer recommended. Though some browsers might still support it, it may have already been removed from the relevant web standards, may be in the process of being dropped, or may only be kept for compatibility purposes. Avoid using it, and update existing code if possible.", false)]
        public static void SetHeader_Pragma(this HttpResponseBase response, string pragma)
        {
            response.Headers.Set("Pragma", pragma);
        }

        /// <summary>
        /// Specify the credentials that the client presents in order to authenticate itself to the proxy (alternative to setting the proxy on the request)
        /// </summary>
        /// <param name="hdrs"></param>
        /// <param name="authorization"></param>
        public static void SetProxyAuthorization(this WebHeaderCollection hdrs, string authorization)
        {
            hdrs.Set(HttpRequestHeader.ProxyAuthorization, authorization);
        }

        /// <summary>
        /// Specify the credentials that the client presents in order to authenticate itself to the proxy (alternative to setting the proxy on the request)
        /// </summary>
        /// <param name="hdrs"></param>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        public static void SetBasicProxyAuthorization(this WebHeaderCollection hdrs, string userName, string password)
        {
            SetProxyAuthorization(hdrs, "Basic " + Encode.Base64(userName + ":" + password));
        }

        /// <summary>
        /// Specify the URI of the resource from which the request URI was obtained
        /// </summary>
        /// <param name="hdrs"></param>
        /// <param name="referrer"></param>
        public static void SetReferer(this WebHeaderCollection hdrs, string referrer)
        {
            hdrs.Set(HttpRequestHeader.Referer, referrer);
        }

        /// <summary>
        /// Specify information about the client agent
        /// </summary>
        /// <param name="hdrs"></param>
        /// <param name="userAgent"></param>
        public static void SetUserAgent(this WebHeaderCollection hdrs, string userAgent)
        {
            hdrs.Set(HttpRequestHeader.UserAgent, userAgent);
        }

        /// <summary>
        /// Specify that the client must authenticate itself to the server
        /// </summary>
        /// <param name="response"></param>
        /// <param name="authenticate"></param>
        public static void SetHeader_WwwAuthenticate(this HttpResponseBase response, string authenticate)
        {
            response.Headers.Set("WWW-Authenticate", authenticate);
        }

        public static string ToDisplayString(this NetworkCredential credentials)
        {
            return credentials.UserName + " [plaintext-password]" + (credentials.Domain != null ? " @" + credentials.Domain : "");
        }
    }
}
