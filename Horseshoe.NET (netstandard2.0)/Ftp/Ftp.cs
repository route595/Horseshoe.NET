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
    // ref: https://docs.microsoft.com/en-us/dotnet/framework/network-programming/how-to-upload-files-with-ftp
    public static class Ftp
    {
        public static event RequestUriCreated RequestUriCreated;
        public static event FileUploaded FileUploaded;
        public static event FileDownloaded FileDownloaded;
        public static event DirectoryContentsListed DirectoryContentsListed;
        public static event FileDeleted FileDeleted;

        public static void UploadFile
        (
            string filePath,
            FtpConnectionInfo connectionInfo = null,
            string serverPath = null,
            string serverFileName = null,
            bool isBinary = false,
            Encoding encoding = null
        )
        {
            byte[] fileContents;

            // Get the file bytes
            if (isBinary)
            {
                fileContents = File.ReadAllBytes(filePath);
            }
            else
            {
                using (StreamReader sourceStream = new StreamReader(filePath))
                {
                    fileContents = (encoding ?? Encoding.Default).GetBytes(sourceStream.ReadToEnd());
                }
            }

            UploadFile(serverFileName ?? Path.GetFileName(filePath), fileContents, connectionInfo: connectionInfo, serverPath: serverPath);
        }

        public static void UploadFile
        (
            FileInfo file,
            FtpConnectionInfo connectionInfo = null,
            string serverPath = null,
            string serverFileName = null,
            bool isBinary = false,
            Encoding encoding = null
        )
        {
            UploadFile
            (
                file.FullName,
                connectionInfo: connectionInfo,
                serverPath: serverPath,
                serverFileName: serverFileName,
                isBinary: isBinary,
                encoding: encoding
            );
        }

        public static void UploadFile
        (
            string fileName,
            string contents,
            FtpConnectionInfo connectionInfo = null,
            string serverPath = null,
            Encoding encoding = null
        )
        {
            // Get the content bytes
            byte[] fileContents = (encoding ?? Encoding.Default).GetBytes(contents);

            UploadFile(fileName, fileContents, connectionInfo: connectionInfo, serverPath: serverPath);
        }

        public static void UploadFile
        (
            NamedMemoryStream namedStream,
            FtpConnectionInfo connectionInfo = null,
            string serverPath = null
        )
        {
            UploadFile(namedStream.Name, namedStream.ToArray(), connectionInfo: connectionInfo, serverPath: serverPath);
        }

        public static void UploadFile
        (
            string fileName,
            Stream contents,
            FtpConnectionInfo connectionInfo = null,
            string serverPath = null
        )
        {
            var memoryStream = new MemoryStream();
            contents.CopyTo(memoryStream);
            UploadFile(fileName, memoryStream.ToArray(), connectionInfo: connectionInfo, serverPath: serverPath);
        }

        public static void UploadFile
        (
            string fileName,
            byte[] contents,
            FtpConnectionInfo connectionInfo = null,
            string serverPath = null
        )
        {
            ConnectAndDoAction
            (
                connectionInfo,
                WebRequestMethods.Ftp.UploadFile,
                serverPath,
                fileName,
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
                    FileUploaded?.Invoke(TextUtil.Reveal(fileName), contents.LongLength, (int)response.StatusCode, response.StatusDescription);
                }
            );
        }

        public static void DownloadFile
        (
            string serverFileName,
            string downloadFilePath,
            bool overwrite = false,
            FtpConnectionInfo connectionInfo = null,
            string serverPath = null
        )
        {
            var stream = DownloadFile
            (
                serverFileName,
                connectionInfo: connectionInfo,
                serverPath: serverPath
            );
            if (Directory.Exists(downloadFilePath))
            {
                downloadFilePath = Path.Combine(downloadFilePath, serverFileName);
            }
            if (File.Exists(downloadFilePath) && !overwrite)
            {
                throw new UtilityException("A file named '" + Path.GetFileName(downloadFilePath) + "' already exists in '" + Path.GetDirectoryName(downloadFilePath) + "'");
            }
            File.WriteAllBytes(downloadFilePath, stream.ToArray());
        }

        public static NamedMemoryStream DownloadFile
        (
            string serverFileName,
            FtpConnectionInfo connectionInfo = null,
            string serverPath = null
        )
        {
            var memoryStream = new NamedMemoryStream(serverFileName);

            ConnectAndDoAction
            (
                connectionInfo,
                WebRequestMethods.Ftp.DownloadFile,
                serverPath,
                serverFileName,
                requestAction: null,
                responseAction: (response) =>
                {
                    var stream = response.GetResponseStream();
                    stream.CopyTo(memoryStream);
                    FileDownloaded?.Invoke(serverFileName, memoryStream.Length, (int)response.StatusCode, response.StatusDescription);
                }
            );

            return memoryStream;
        }

        public static string[] ListDirectoryContents
        (
            string fileMask = null,
            FtpConnectionInfo connectionInfo = null,
            string serverPath = null
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

                    DirectoryContentsListed?.Invoke(contents.Length, (int)response.StatusCode, response.StatusDescription);
                }
            );

            return contents;
        }

        public static string[] ListDetailedDirectoryContents
        (
            FtpConnectionInfo connectionInfo = null,
            string serverPath = null
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
                    DirectoryContentsListed?.Invoke(contents.Length, (int)response.StatusCode, response.StatusDescription);
                }
            );

            return contents;
        }

        public static void DeleteFile
        (
            string serverFileName,
            FtpConnectionInfo connectionInfo = null,
            string serverPath = null
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
                    FileDeleted?.Invoke(serverFileName, (int)response.StatusCode, response.StatusDescription);
                }
            );
        }

        static void ConnectAndDoAction
        (
            FtpConnectionInfo connectionInfo,
            string method,
            string serverPath,
            string serverFileName,
            Action<FtpWebRequest> requestAction,   // optional
            Action<FtpWebResponse> responseAction  // required
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
                serverPath = connectionInfo.ServerPath ?? serverPath;
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
            var uriString = CreateRequestUriString(server, port, enableSsl, serverPath, serverFileName);
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

        static string CreateRequestUriString(string server, int? port, bool enableSsl, string serverPath, string fileName)
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
            RequestUriCreated?.Invoke(sb.ToString());
            return sb.ToString();
        }
    }
}
