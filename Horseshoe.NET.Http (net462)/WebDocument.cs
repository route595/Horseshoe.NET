using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace Horseshoe.NET.Http
{
    public static class WebDocument
    {
        public static Stream AsStream
        (
            UriString uri,
            string method = "GET",
            SecurityProtocolType? securityProtocol = null,
            Action<WebHeaderCollection> alterHeaders = null,
            Credential? credentials = null,
            UriString? proxyAddress = null,
            int? proxyPort = null,
            Credential? proxyCredentials = null,
            Action<HttpWebRequest> alterRequest = null,
            Action<HttpWebResponse, ConsumerResponseEnvelope> handleResponse = null
        )
        {
            Stream stream = null;
            Get.AsString
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
                handleResponse: (response, consumerResponse) =>
                {
                    handleResponse?.Invoke(response, consumerResponse);
                    stream = response.GetResponseStream();
                    consumerResponse.Flag = ConsumerResponse.SuppressFurtherResponseHandling;
                }
            );
            return stream;
        }

        public async static Task<Stream> AsStreamAsync
        (
            UriString uri,
            string method = "GET",
            SecurityProtocolType? securityProtocol = null,
            Action<WebHeaderCollection> alterHeaders = null,
            Credential? credentials = null,
            UriString? proxyAddress = null,
            int? proxyPort = null,
            Credential? proxyCredentials = null,
            Action<HttpWebRequest> alterRequest = null,
            Action<HttpWebResponse, ConsumerResponseEnvelope> handleResponse = null
        )
        {
            Stream stream = null;
            await Get.AsStringAsync
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
                handleResponse: (response, consumerResponse) =>
                {
                    handleResponse?.Invoke(response, consumerResponse);
                    stream = response.GetResponseStream();
                    consumerResponse.Flag = ConsumerResponse.SuppressFurtherResponseHandling;
                }
            );
            return stream;
        }

        public static Stream AsStream
        (
            UriString uri,
            object content,
            string method = "POST",
            string contentType = null,
            Func<object,string> contentSerializer = null,
            SecurityProtocolType? securityProtocol = null,
            Action<WebHeaderCollection> alterHeaders = null,
            Credential? credentials = null,
            UriString? proxyAddress = null,
            int? proxyPort = null,
            Credential? proxyCredentials = null,
            Action<HttpWebRequest> alterRequest = null,
            Action<HttpWebResponse, ConsumerResponseEnvelope> handleResponse = null
        )
        {
            Stream stream = null;
            Post.AsString
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
                handleResponse: (response, consumerResponse) =>
                {
                    handleResponse?.Invoke(response, consumerResponse);
                    stream = response.GetResponseStream();
                    consumerResponse.Flag = ConsumerResponse.SuppressFurtherResponseHandling;
                }
            );
            return stream;
        }

        public async static Task<Stream> AsStreamAsync
        (
            UriString uri,
            object content,
            string method = "POST",
            string contentType = null,
            Func<object, string> contentSerializer = null,
            SecurityProtocolType? securityProtocol = null,
            Action<WebHeaderCollection> alterHeaders = null,
            Credential? credentials = null,
            UriString? proxyAddress = null,
            int? proxyPort = null,
            Credential? proxyCredentials = null,
            Action<HttpWebRequest> alterRequest = null,
            Action<HttpWebResponse, ConsumerResponseEnvelope> handleResponse = null
        )
        {
            Stream stream = null;
            await Post.AsStringAsync
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
                handleResponse: (response, consumerResponse) =>
                {
                    handleResponse?.Invoke(response, consumerResponse);
                    stream = response.GetResponseStream();
                    consumerResponse.Flag = ConsumerResponse.SuppressFurtherResponseHandling;
                }
            );
            return stream;
        }

        public static byte[] AsBytes
        (
            UriString uri,
            string method = "GET",
            SecurityProtocolType? securityProtocol = null,
            Action<WebHeaderCollection> alterHeaders = null,
            Credential? credentials = null,
            UriString? proxyAddress = null,
            int? proxyPort = null,
            Credential? proxyCredentials = null,
            Action<HttpWebRequest> alterRequest = null,
            Action<HttpWebResponse, ConsumerResponseEnvelope> handleResponse = null
        )
        {
            Stream stream = AsStream
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
                handleResponse: handleResponse
            );
            var memoryStream = new MemoryStream();
            using (stream)
            {
                stream.CopyTo(memoryStream);
            }
            return memoryStream.ToArray();
        }

        public async static Task<byte[]> AsBytesAsync
        (
            UriString uri,
            string method = "GET",
            SecurityProtocolType? securityProtocol = null,
            Action<WebHeaderCollection> alterHeaders = null,
            Credential? credentials = null,
            UriString? proxyAddress = null,
            int? proxyPort = null,
            Credential? proxyCredentials = null,
            Action<HttpWebRequest> alterRequest = null,
            Action<HttpWebResponse, ConsumerResponseEnvelope> handleResponse = null
        )
        {
            Stream stream = await AsStreamAsync
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
                handleResponse: handleResponse
            );
            var memoryStream = new MemoryStream();
            using (stream)
            {
                await stream.CopyToAsync(memoryStream);
            }
            return memoryStream.ToArray();
        }

        public static byte[] AsBytes
        (
            UriString uri,
            object content,
            string method = "POST",
            string contentType = null,
            Func<object, string> contentSerializer = null,
            SecurityProtocolType? securityProtocol = null,
            Action<WebHeaderCollection> alterHeaders = null,
            Credential? credentials = null,
            UriString? proxyAddress = null,
            int? proxyPort = null,
            Credential? proxyCredentials = null,
            Action<HttpWebRequest> alterRequest = null,
            Action<HttpWebResponse, ConsumerResponseEnvelope> handleResponse = null
        )
        {
            Stream stream = AsStream
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
                handleResponse: handleResponse
            );
            var memoryStream = new MemoryStream();
            using (stream)
            {
                stream.CopyTo(memoryStream);
            }
            return memoryStream.ToArray();
        }

        public async static Task<byte[]> AsBytesAsync
        (
            UriString uri,
            object content,
            string method = "POST",
            string contentType = null,
            Func<object, string> contentSerializer = null,
            SecurityProtocolType? securityProtocol = null,
            Action<WebHeaderCollection> alterHeaders = null,
            Credential? credentials = null,
            UriString? proxyAddress = null,
            int? proxyPort = null,
            Credential? proxyCredentials = null,
            Action<HttpWebRequest> alterRequest = null,
            Action<HttpWebResponse, ConsumerResponseEnvelope> handleResponse = null
        )
        {
            Stream stream = await AsStreamAsync
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
                handleResponse: handleResponse
            );
            var memoryStream = new MemoryStream();
            using (stream)
            {
                await stream.CopyToAsync(memoryStream);
            }
            return memoryStream.ToArray();
        }

        public static bool Download
        (
            UriString uri,
            string destinationFilePath,
            string method = "GET",
            bool overwrite = false,
            SecurityProtocolType? securityProtocol = null,
            Action<WebHeaderCollection> alterHeaders = null,
            Credential? credentials = null,
            UriString? proxyAddress = null,
            int? proxyPort = null,
            Credential? proxyCredentials = null,
            Action<HttpWebRequest> alterRequest = null,
            Action<HttpWebResponse, ConsumerResponseEnvelope> handleResponse = null
        )
        {
            Stream stream = AsStream
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
                handleResponse: handleResponse
            );
            using (var writer = new FileStream(destinationFilePath, overwrite ? FileMode.Create : FileMode.CreateNew))
            {
                using (stream)
                {
                    stream.CopyTo(writer);
                }
            }
            return true;
        }

        public async static Task<bool> DownloadAsync
        (
            UriString uri,
            string destinationFilePath,
            string method = "GET",
            bool overwrite = false,
            SecurityProtocolType? securityProtocol = null,
            Action<WebHeaderCollection> alterHeaders = null,
            Credential? credentials = null,
            UriString? proxyAddress = null,
            int? proxyPort = null,
            Credential? proxyCredentials = null,
            Action<HttpWebRequest> alterRequest = null,
            Action<HttpWebResponse, ConsumerResponseEnvelope> handleResponse = null
        )
        {
            Stream stream = await AsStreamAsync
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
                handleResponse: handleResponse
            );
            using (var writer = new FileStream(destinationFilePath, overwrite ? FileMode.Create : FileMode.CreateNew))
            {
                using (stream)
                {
                    await stream.CopyToAsync(writer);
                }
            }
            return true;
        }

        public static bool Download
        (
            UriString uri,
            object content,
            string destinationFilePath,
            string method = "POST",
            bool overwrite = false,
            string contentType = null,
            Func<object, string> contentSerializer = null,
            SecurityProtocolType? securityProtocol = null,
            Action<WebHeaderCollection> alterHeaders = null,
            Credential? credentials = null,
            UriString? proxyAddress = null,
            int? proxyPort = null,
            Credential? proxyCredentials = null,
            Action<HttpWebRequest> alterRequest = null,
            Action<HttpWebResponse, ConsumerResponseEnvelope> handleResponse = null
        )
        {
            Stream stream = AsStream
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
                handleResponse: handleResponse
            );
            using (var writer = new FileStream(destinationFilePath, overwrite ? FileMode.Create : FileMode.CreateNew))
            {
                using (stream)
                {
                    stream.CopyTo(writer);
                }
            }
            return true;
        }

        public async static Task<bool> DownloadAsync
        (
            UriString uri,
            object content,
            string destinationFilePath,
            string method = "POST",
            bool overwrite = false,
            string contentType = null,
            Func<object, string> contentSerializer = null,
            SecurityProtocolType? securityProtocol = null,
            Action<WebHeaderCollection> alterHeaders = null,
            Credential? credentials = null,
            UriString? proxyAddress = null,
            int? proxyPort = null,
            Credential? proxyCredentials = null,
            Action<HttpWebRequest> alterRequest = null,
            Action<HttpWebResponse, ConsumerResponseEnvelope> handleResponse = null
        )
        {
            Stream stream = await AsStreamAsync
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
                handleResponse: handleResponse
            );
            using (var writer = new FileStream(destinationFilePath, overwrite ? FileMode.Create : FileMode.CreateNew))
            {
                using (stream)
                {
                    await stream.CopyToAsync(writer);
                }
            }
            return true;
        }

        public static string AsText
        (
            UriString uri,
            string method = "GET",
            SecurityProtocolType? securityProtocol = null,
            Action<WebHeaderCollection> alterHeaders = null,
            Credential? credentials = null,
            UriString? proxyAddress = null,
            int? proxyPort = null,
            Credential? proxyCredentials = null,
            Action<HttpWebRequest> alterRequest = null,
            Action<HttpWebResponse, ConsumerResponseEnvelope> handleResponse = null
        )
        {
            var text = Get.AsString
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
                handleResponse: handleResponse
            );
            return text;
        }

        public async static Task<string> AsTextAsync
        (
            UriString uri,
            string method = "GET",
            SecurityProtocolType? securityProtocol = null,
            Action<WebHeaderCollection> alterHeaders = null,
            Credential? credentials = null,
            UriString? proxyAddress = null,
            int? proxyPort = null,
            Credential? proxyCredentials = null,
            Action<HttpWebRequest> alterRequest = null,
            Action<HttpWebResponse, ConsumerResponseEnvelope> handleResponse = null
        )
        {
            var text = await Get.AsStringAsync
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
                handleResponse: handleResponse
            );
            return text;
        }

        public static string AsText
        (
            UriString uri,
            object content,
            string method = "POST",
            string contentType = null,
            Func<object, string> contentSerializer = null,
            SecurityProtocolType? securityProtocol = null,
            Action<WebHeaderCollection> alterHeaders = null,
            Credential? credentials = null,
            UriString? proxyAddress = null,
            int? proxyPort = null,
            Credential? proxyCredentials = null,
            Action<HttpWebRequest> alterRequest = null,
            Action<HttpWebResponse, ConsumerResponseEnvelope> handleResponse = null
        )
        {
            var text = Post.AsString
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
                handleResponse: handleResponse
            );
            return text;
        }

        public async static Task<string> AsTextAsync
        (
            UriString uri,
            object content,
            string method = "POST",
            string contentType = null,
            Func<object, string> contentSerializer = null,
            SecurityProtocolType? securityProtocol = null,
            Action<WebHeaderCollection> alterHeaders = null,
            Credential? credentials = null,
            UriString? proxyAddress = null,
            int? proxyPort = null,
            Credential? proxyCredentials = null,
            Action<HttpWebRequest> alterRequest = null,
            Action<HttpWebResponse, ConsumerResponseEnvelope> handleResponse = null
        )
        {
            var text = await Post.AsStringAsync
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
                handleResponse: handleResponse
            );
            return text;
        }
    }
}
