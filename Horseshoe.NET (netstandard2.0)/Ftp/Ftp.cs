using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

using Horseshoe.NET.IO;
using Horseshoe.NET.Text;

namespace Horseshoe.NET.Ftp
{
    /// <summary>
    /// A collection of factory methods for FTP operations
    /// </summary>
    /// <remarks>
    /// Reference: https://docs.microsoft.com/en-us/dotnet/framework/network-programming/how-to-upload-files-with-ftp
    /// </remarks>
    public static class Ftp
    {
        /// <summary>
        /// Uploads a file to an FTP server
        /// </summary>
        /// <param name="filePath">The source file path</param>
        /// <param name="connectionInfo">FTP connection info</param>
        /// <param name="serverPath">The upload destination virtual path, overrides <c>ServerPath</c> in <c>connectionInfo</c></param>
        /// <param name="destRemoteFileName">What to name the uploaded file (default is the file's current name)</param>
        /// <param name="isBinary">Affects how the file is read into memory before sending to the FTP server, default is <c>false</c></param>
        /// <param name="encoding">An optional text encoding used when <c>isBinary == false</c> to read the file from storage and convert it to bytes</param>
        /// <param name="onFileUploaded">Action to perform on file upload, e.g. <c>onFileUploaded(string fileName, long fileSize, int statusCode, string statusDescription)</c></param>
        /// <param name="requestUriCreated">View the request URI, e.g. <c>requestUriCreated(string requestUri)</c></param>
        public static void UploadFile
        (
            FilePath filePath,
            FtpConnectionInfo connectionInfo = null,
            string serverPath = null,
            string destRemoteFileName = null,
            bool isBinary = false,
            Encoding encoding = null,
            Action<string, long, int, string> onFileUploaded = null,
            Action<string> requestUriCreated = null
        )
        {
            byte[] fileContents;

            // Get the file bytes
            if (isBinary)
            {
                fileContents = filePath.ReadAllBytes();
            }
            else
            {
                using (StreamReader sourceStream = encoding != null ? new StreamReader(filePath, encoding) : new StreamReader(filePath))
                {
                    fileContents = (encoding ?? Encoding.Default).GetBytes(sourceStream.ReadToEnd());
                }
            }

            CreateFile(destRemoteFileName ?? filePath.Name, fileContents, connectionInfo: connectionInfo, serverPath: serverPath, onFileUploaded: onFileUploaded, requestUriCreated: requestUriCreated);
        }

        /// <summary>
        /// Uploads a file to an FTP server
        /// </summary>
        /// <param name="destRemoteFileName">What to name the uploaded file</param>
        /// <param name="contents">Contents as a <c>string</c></param>
        /// <param name="connectionInfo">FTP connection info</param>
        /// <param name="serverPath">The upload destination virtual path, overrides <c>ServerPath</c> in <c>connectionInfo</c></param>
        /// <param name="encoding">An optional text encoding to convert it to bytes</param>
        /// <param name="onFileUploaded">Action to perform on file upload, e.g. <c>onFileUploaded(string fileName, long fileSize, int statusCode, string statusDescription)</c></param>
        /// <param name="requestUriCreated">View the request URI, e.g. <c>requestUriCreated(string requestUri)</c></param>
        public static void CreateFile
        (
            string destRemoteFileName,
            string contents,
            FtpConnectionInfo connectionInfo = null,
            string serverPath = null,
            Encoding encoding = null,
            Action<string, long, int, string> onFileUploaded = null,
            Action<string> requestUriCreated = null
        )
        {
            // Get the content bytes
            byte[] fileContents = (encoding ?? Encoding.Default).GetBytes(contents);

            CreateFile(destRemoteFileName, fileContents, connectionInfo: connectionInfo, serverPath: serverPath, onFileUploaded: onFileUploaded, requestUriCreated: requestUriCreated);
        }

        /// <summary>
        /// Uploads a file to an FTP server
        /// </summary>
        /// <param name="namedStream">A named memory stream</param>
        /// <param name="connectionInfo">FTP connection info</param>
        /// <param name="serverPath">The upload destination virtual path, overrides <c>ServerPath</c> in <c>connectionInfo</c></param>
        /// <param name="onFileUploaded">Action to perform on file upload, e.g. <c>onFileUploaded(string fileName, long fileSize, int statusCode, string statusDescription)</c></param>
        /// <param name="requestUriCreated">View the request URI, e.g. <c>requestUriCreated(string requestUri)</c></param>
        public static void CreateFile
        (
            NamedMemoryStream namedStream,
            FtpConnectionInfo connectionInfo = null,
            string serverPath = null,
            Action<string, long, int, string> onFileUploaded = null,
            Action<string> requestUriCreated = null
        )
        {
            CreateFile(namedStream.Name, namedStream.ToArray(), connectionInfo: connectionInfo, serverPath: serverPath, onFileUploaded: onFileUploaded, requestUriCreated: requestUriCreated);
        }

        /// <summary>
        /// Uploads a file to an FTP server
        /// </summary>
        /// <param name="destRemoteFileName">What to name the uploaded file</param>
        /// <param name="contents">Contents as a <c>Stream</c></param>
        /// <param name="connectionInfo">FTP connection info</param>
        /// <param name="serverPath">The upload destination virtual path, overrides <c>ServerPath</c> in <c>connectionInfo</c></param>
        /// <param name="onFileUploaded">Action to perform on file upload, e.g. <c>onFileUploaded(string fileName, long fileSize, int statusCode, string statusDescription)</c></param>
        /// <param name="requestUriCreated">View the request URI, e.g. <c>requestUriCreated(string requestUri)</c></param>
        public static void CreateFile
        (
            string destRemoteFileName,
            Stream contents,
            FtpConnectionInfo connectionInfo = null,
            string serverPath = null,
            Action<string, long, int, string> onFileUploaded = null,
            Action<string> requestUriCreated = null
        )
        {
            var memoryStream = new MemoryStream();
            contents.CopyTo(memoryStream);
            CreateFile(destRemoteFileName, memoryStream.ToArray(), connectionInfo: connectionInfo, serverPath: serverPath, onFileUploaded: onFileUploaded, requestUriCreated: requestUriCreated);
        }

        /// <summary>
        /// Uploads a file to an FTP server
        /// </summary>
        /// <param name="destRemoteFileName">What to name the uploaded file</param>
        /// <param name="contents">Contents as a <c>byte[]</c></param>
        /// <param name="connectionInfo">FTP connection info</param>
        /// <param name="serverPath">The upload destination virtual path, overrides <c>ServerPath</c> in <c>connectionInfo</c></param>
        /// <param name="onFileUploaded">Action to perform on file upload, e.g. <c>onFileUploaded(string fileName, long fileSize, int statusCode, string statusDescription)</c></param>
        /// <param name="requestUriCreated">View the request URI, e.g. <c>requestUriCreated(string requestUri)</c></param>
        public static void CreateFile
        (
            string destRemoteFileName,
            byte[] contents,
            FtpConnectionInfo connectionInfo = null,
            string serverPath = null,
            Action<string, long, int, string> onFileUploaded = null,
            Action<string> requestUriCreated = null
        )
        {
            ConnectAndDoAction
            (
                connectionInfo,
                WebRequestMethods.Ftp.UploadFile,
                serverPath,
                destRemoteFileName,
                requestAction: (request) =>
                {
                    request.ContentLength = contents.LongLength;
                    using (Stream requestStream = request.GetRequestStream())
                    {
                        requestStream.Write(contents, 0, contents.Length);
                    }
                },
                responseAction: (response) =>
                {
                    onFileUploaded?.Invoke(TextUtil.Reveal(destRemoteFileName), contents.LongLength, (int)response.StatusCode, response.StatusDescription);
                },
                requestUriCreated
            );
        }

        /// <summary>
        /// Downloads a file from an FTP server
        /// </summary>
        /// <param name="srcRemoteFileName">The file to download from the FTP server</param>
        /// <param name="localDirectory"></param>
        /// <param name="destFileName">Optionally rename the downloaded file</param>
        /// <param name="overwrite">Set to <c>true</c> to overwrite an existing file if applicable, default is <c>false</c></param>
        /// <param name="connectionInfo">FTP connection info</param>
        /// <param name="serverPath">The upload destination virtual path, overrides <c>ServerPath</c> in <c>connectionInfo</c></param>
        /// <param name="onFileDownloaded">Action to perform on file download, e.g. <c>onFileDownloaded(string fileName, long fileSize, int statusCode, string statusDescription)</c></param>
        /// <param name="requestUriCreated">View the request URI, e.g. <c>requestUriCreated(string requestUri)</c></param>
        /// <exception cref="FtpOperationException"></exception>
        public static void DownloadFile
        (
            string srcRemoteFileName,
            DirectoryPath localDirectory,
            string destFileName = null,
            bool overwrite = false,
            FtpConnectionInfo connectionInfo = null,
            string serverPath = null,
            Action<string, long, int, string> onFileDownloaded = null,
            Action<string> requestUriCreated = null
        )
        {
            if (!localDirectory.Exists)
                localDirectory.Create();

            var stream = DownloadFile
            (
                srcRemoteFileName,
                connectionInfo: connectionInfo,
                serverPath: serverPath,
                onFileDownloaded: onFileDownloaded,
                requestUriCreated: requestUriCreated
            );

            FilePath downloadedFilePath = Path.Combine(localDirectory, destFileName ?? srcRemoteFileName);
            if (downloadedFilePath.Exists && !overwrite)
            {
                throw new FtpOperationException("A file named '" + downloadedFilePath.Name + "' already exists in '" + localDirectory.FullName + "'");
            }
            downloadedFilePath.WriteAllBytes(stream.ToArray());
        }

        /// <summary>
        /// Downloads a file from an FTP server
        /// </summary>
        /// <param name="srcRemoteFileName">The file to download from the FTP server</param>
        /// <param name="connectionInfo">FTP connection info</param>
        /// <param name="serverPath">The upload destination virtual path, overrides <c>ServerPath</c> in <c>connectionInfo</c></param>
        /// <param name="onFileDownloaded">Action to perform on file download, e.g. <c>onFileDownloaded(string fileName, long fileSize, int statusCode, string statusDescription)</c></param>
        /// <param name="requestUriCreated">View the request URI, e.g. <c>requestUriCreated(string requestUri)</c></param>
        /// <returns></returns>
        public static NamedMemoryStream DownloadFile
        (
            string srcRemoteFileName,
            FtpConnectionInfo connectionInfo = null,
            string serverPath = null,
            Action<string, long, int, string> onFileDownloaded = null,
            Action<string> requestUriCreated = null
        )
        {
            var memoryStream = new NamedMemoryStream(srcRemoteFileName);

            ConnectAndDoAction
            (
                connectionInfo,
                WebRequestMethods.Ftp.DownloadFile,
                serverPath,
                srcRemoteFileName,
                requestAction: null,
                responseAction: (response) =>
                {
                    using (var stream = response.GetResponseStream())
                    {
                        stream.CopyTo(memoryStream);
                    }
                    onFileDownloaded?.Invoke(srcRemoteFileName, memoryStream.Length, (int)response.StatusCode, response.StatusDescription);
                },
                requestUriCreated
            );

            return memoryStream;
        }

        /// <summary>
        /// Lists the contents of a remote virtual directory on an FTP server
        /// </summary>
        /// <param name="fileMask">Optional file mask for filtering the list (for exmaple use *.txt)</param>
        /// <param name="connectionInfo">FTP connection info</param>
        /// <param name="serverPath">The upload destination virtual path, overrides <c>ServerPath</c> in <c>connectionInfo</c></param>
        /// <param name="directoryContentsListed">Action to perform on file download, e.g. <c>directoryContentsListed(int count, int statusCode, string statusDescription)</c></param>
        /// <param name="requestUriCreated">View the request URI, e.g. <c>requestUriCreated(string requestUri)</c></param>
        /// <returns></returns>
        public static string[] ListDirectoryContents
        (
            string fileMask = null,
            FtpConnectionInfo connectionInfo = null,
            string serverPath = null,
            Action<int, int, string> directoryContentsListed = null,
            Action<string> requestUriCreated = null
        )
        {
            string[] contents = null;

            // Convert the file mask to regex
            Regex filter = null;
            if (fileMask == FtpFileMasks.NoExtension)
            {
                filter = new Regex("^[^.]+$");
            }
            else if (fileMask != null)
            {
                filter = new Regex(fileMask.Replace(".", @"\.").Replace("*", ".*").Replace("?", "."), RegexOptions.IgnoreCase);
            }

            ConnectAndDoAction
            (
                connectionInfo,
                WebRequestMethods.Ftp.ListDirectory,
                serverPath,
                null,
                requestAction: null,
                responseAction: (response) =>
                {
                    var streamReader = new StreamReader(response.GetResponseStream());
                    contents = streamReader.ReadToEnd().Replace("\r\n", "\n").Split('\n'); if (filter != null)
                    {
                        contents = contents
                            .Where(s => filter.IsMatch(s))
                            .ToArray();
                    }

                    directoryContentsListed?.Invoke(contents.Length, (int)response.StatusCode, response.StatusDescription);
                },
                requestUriCreated
            );

            return contents;
        }

        /// <summary>
        /// Lists the contents of a remote virtual directory on an FTP server
        /// </summary>
        /// <param name="connectionInfo">FTP connection info</param>
        /// <param name="serverPath">The upload destination virtual path, overrides <c>ServerPath</c> in <c>connectionInfo</c></param>
        /// <param name="directoryContentsListed">Action to perform on file download, e.g. <c>directoryContentsListed(int count, int statusCode, string statusDescription)</c></param>
        /// <param name="requestUriCreated">View the request URI, e.g. <c>requestUriCreated(string requestUri)</c></param>
        /// <returns></returns>
        public static string[] ListDetailedDirectoryContents
        (
            FtpConnectionInfo connectionInfo = null,
            string serverPath = null,
            Action<int, int, string> directoryContentsListed = null,
            Action<string> requestUriCreated = null
        )
        {
            string[] contents = null;

            ConnectAndDoAction
            (
                connectionInfo,
                WebRequestMethods.Ftp.ListDirectoryDetails,
                serverPath,
                null,
                requestAction: null,
                responseAction: (response) =>
                {
                    var streamReader = new StreamReader(response.GetResponseStream());
                    contents = streamReader.ReadToEnd().Replace("\r\n", "\n").Split('\n');
                    directoryContentsListed?.Invoke(contents.Length, (int)response.StatusCode, response.StatusDescription);
                },
                requestUriCreated
            );

            return contents;
        }

        /// <summary>
        /// Deletes a file remotely from an FTP server
        /// </summary>
        /// <param name="serverFileName">The file to download from the FTP server</param>
        /// <param name="connectionInfo">FTP connection info</param>
        /// <param name="serverPath">The upload destination virtual path, overrides <c>ServerPath</c> in <c>connectionInfo</c></param>
        /// <param name="fileDeleted">Action to perform on file download, e.g. <c>fileDeleted(string fileName, int statusCode, string statusDescription)</c></param>
        /// <param name="requestUriCreated">View the request URI, e.g. <c>requestUriCreated(string requestUri)</c></param>
        public static void DeleteFile
        (
            string serverFileName,
            FtpConnectionInfo connectionInfo = null,
            string serverPath = null,
            Action<string, int, string> fileDeleted = null,
            Action<string> requestUriCreated = null
        )
        {
            ConnectAndDoAction
            (
                connectionInfo,
                WebRequestMethods.Ftp.DeleteFile,
                serverPath,
                serverFileName,
                requestAction: null,
                responseAction: (response) =>
                {
                    fileDeleted?.Invoke(serverFileName, (int)response.StatusCode, response.StatusDescription);
                },
                requestUriCreated
            );
        }

        static void ConnectAndDoAction
        (
            FtpConnectionInfo connectionInfo,
            string method,
            string serverPath,
            string serverFileName,
            Action<FtpWebRequest> requestAction,   // optional 
            Action<FtpWebResponse> responseAction, // required
            Action<string> requestUriCreated
        )
        {
            string server;
            int? port;
            bool enableSsl;
            Credential? credentials;

            if (connectionInfo != null)
            {
                server = connectionInfo.Server;
                port = connectionInfo.Port;
                enableSsl = connectionInfo.EnableSsl;
                credentials = connectionInfo.Credentials;
                serverPath = serverPath ?? connectionInfo.ServerPath;
            }
            else
            {
                server = FtpSettings.DefaultFtpServer;
                port = FtpSettings.DefaultPort;
                enableSsl = FtpSettings.DefaultEnableSsl;
                credentials = FtpSettings.DefaultCredentials;
                serverPath = serverPath ?? FtpSettings.DefaultServerPath;
            }

            // Get the object used to communicate with the server
            var uriString = CreateRequestUriString(server, port, enableSsl, serverPath, serverFileName, requestUriCreated);
            var request = (FtpWebRequest)WebRequest.Create(uriString);
            if (enableSsl)
            {
                request.EnableSsl = true;
            }
            request.Method = method;
            request.Credentials = credentials?.ToNetworkCredential() ?? new NetworkCredential("anonymous", "ftpuser");
            
            requestAction?.Invoke(request);
            
            using (var response = (FtpWebResponse)request.GetResponse())
            {
                responseAction.Invoke(response);
            }
        }

        static string CreateRequestUriString
        (
            string server, 
            int? port, 
            bool enableSsl, 
            string serverPath, 
            string fileName,
            Action<string> requestUriCreated
        )
        {
            if (server == null) throw new ArgumentNullException(nameof(server));
            var sb = new StringBuilder()
                .AppendIf
                (
                    !server.Contains("://"),
                    "ftp://"
                )
                .AppendIf
                (
                    server.EndsWith("/"),
                    server.Substring(0, server.Length - 1),
                    server
                )
                .AppendIf
                (
                    port.HasValue,
                    ":" + port
                )
                .AppendIf
                (
                    !port.HasValue && enableSsl,
                    ":990"
                )
                .Append("/");
            if (serverPath != null)
            {
                sb  .Append
                    (
                        serverPath
                    )
                    .AppendIf
                    (
                        serverPath.Length > 0 && !serverPath.EndsWith("/"),
                        "/"
                    );
            }
            if (fileName != null)
            {
                sb.Append(fileName);
            }
            requestUriCreated?.Invoke(sb.ToString());
            return sb.ToString();
        }
    }
}
