﻿using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using Horseshoe.NET.IO;
using Horseshoe.NET.IO.Ftp;
using Horseshoe.NET.Text;

using Renci.SshNet;

namespace Horseshoe.NET.SecureIO.Sftp
{
    // ref: https://www.codeproject.com/Tips/1111060/Upload-File-to-SFTP-Site-with-Csharp-in-Visual-Stu
    public static class Sftp
    {
        //public static event FileUploaded FileUploaded;
        //public static event FileDownloaded FileDownloaded;
        //public static event DirectoryContentsListed DirectoryContentsListed;
        //public static event FileDeleted FileDeleted;

        public static void UploadFile
        (
            FilePath sourceFile,
            SftpConnectionInfo connectionInfo = null,
            string serverPath = null,
            string serverFileName = null,
            bool isBinary = false,
            Encoding encoding = null,
            Action<string, long, int, string> onFileUploaded = null
        )
        {
            byte[] fileContents;

            // Get the file bytes
            if (isBinary)
            {
                fileContents = File.ReadAllBytes(sourceFile);
            }
            else
            {
                using (StreamReader sourceStream = new StreamReader(sourceFile))
                {
                    fileContents = (encoding ?? Encoding.Default).GetBytes(sourceStream.ReadToEnd());
                }
            }

            CreateFile(serverFileName ?? Path.GetFileName(sourceFile), fileContents, connectionInfo: connectionInfo, serverPath: serverPath, onFileUploaded: onFileUploaded);
        }

        public static void CreateFile
        (
            string destFileName,
            string contents,
            SftpConnectionInfo connectionInfo = null,
            string serverPath = null,
            Encoding encoding = null,
            Action<string, long, int, string> onFileUploaded = null
        )
        {
            // Get the content bytes
            byte[] fileContents = (encoding ?? Encoding.Default).GetBytes(contents);

            CreateFile(destFileName, fileContents, connectionInfo: connectionInfo, serverPath: serverPath, onFileUploaded: onFileUploaded);
        }

        public static void CreateFile
        (
            NamedMemoryStream namedStream,
            SftpConnectionInfo connectionInfo = null,
            string serverPath = null,
            Action<string, long, int, string> onFileUploaded = null
        )
        {
            CreateFile(namedStream.Name, namedStream, connectionInfo: connectionInfo, serverPath: serverPath, onFileUploaded: onFileUploaded);
        }

        public static void CreateFile
        (
            string destFileName,
            byte[] contents,
            SftpConnectionInfo connectionInfo = null,
            string serverPath = null,
            Action<string, long, int, string> onFileUploaded = null
        )
        {
            var memoryStream = new MemoryStream();
            memoryStream.Write(contents, 0, contents.Length);
            memoryStream.Position = 0;
            CreateFile(destFileName, memoryStream, connectionInfo: connectionInfo, serverPath: serverPath, onFileUploaded: onFileUploaded);
        }

        public static void CreateFile
        (
            string destFileName,
            Stream contents,
            SftpConnectionInfo connectionInfo = null,
            string serverPath = null,
            Action<string, long, int, string> onFileUploaded = null
        )
        {
            ConnectAndDoAction
            (
                connectionInfo,
                serverPath,
                action: (server, resultantServerPath) =>
                {
                    long bytesUploaded = 0L;
                    if (resultantServerPath.Length > 0)
                    {
                        server.ChangeDirectory(resultantServerPath);
                    }
                    server.BufferSize = 4 * 1024;
                    server.UploadFile(contents, destFileName, (ulongValue) => bytesUploaded = (long)ulongValue);
                    onFileUploaded?.Invoke(TextUtil.Reveal(destFileName), bytesUploaded, 0, "File upload action raised no errors");
                }
            );
        }

        public static void DownloadFile
        (
            string serverFileName,
            string downloadFilePath,
            bool overwrite = false,
            SftpConnectionInfo connectionInfo = null,
            string serverPath = null,
            Action<string, long, int, string> fileDownloaded = null
        )
        {
            var stream = DownloadFile
            (
                serverFileName,
                connectionInfo: connectionInfo,
                serverPath: serverPath,
                fileDownloaded: fileDownloaded
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
            SftpConnectionInfo connectionInfo = null,
            string serverPath = null,
            Action<string, long, int, string> fileDownloaded = null
        )
        {
            var memoryStream = new NamedMemoryStream(serverFileName);

            ConnectAndDoAction
            (
                connectionInfo,
                serverPath,
                action: (server, resultantServerPath) =>
                {
                    if (resultantServerPath.Length > 0)
                    {
                        server.ChangeDirectory(resultantServerPath);
                    }
                    server.DownloadFile(serverFileName, memoryStream);
                    fileDownloaded?.Invoke(serverFileName, memoryStream.Length, 0, "File download action raised no errors");
                }
            );

            return memoryStream;
        }


        public static string[] ListDirectoryContents
        (
            string fileMask = null,
            SftpConnectionInfo connectionInfo = null,
            string serverPath = null,
            Action<int, int, string> directoryContentsListed = null
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
                serverPath,
                action: (server, resultantServerPath) =>
                {
                    var files = server.ListDirectory(resultantServerPath);
                    contents = files
                        .Select(f => f.Name)
                        .Where(s => !s.In(".", "..") && (filter?.IsMatch(s) ?? true))  // filter out '.' and '..' and apply file mask filter 
                        .OrderBy(s => s.Contains(".") ? 1 : 0)
                        .ThenBy(s => s)
                        .ToArray();
                    directoryContentsListed?.Invoke(contents.Length, 0, "List directory action raised no errors");
                }
            );

            return contents;
        }

        public static void DeleteFile
        (
            string serverFileName,
            SftpConnectionInfo connectionInfo = null,
            string serverPath = null,
            Action<string, int, string> fileDeleted = null
        )
        {
            ConnectAndDoAction
            (
                connectionInfo,
                serverPath,
                action: (server, resultantServerPath) =>
                {
                    if (resultantServerPath.Length > 0)
                    {
                        server.ChangeDirectory(resultantServerPath);
                    }
                    server.DeleteFile(serverFileName);
                    fileDeleted?.Invoke(serverFileName, 0, "Delete action raised no errors");
                }
            );
        }

        static void ConnectAndDoAction
        (
            SftpConnectionInfo connectionInfo,
            string serverPath,
            Action<SftpClient, string> action    // server, resultantServerPath
        )
        {
            string server;
            int port;
            Credential credentials;

            if (connectionInfo != null)
            {
                server = connectionInfo.Server;
                port = connectionInfo.Port ?? 22;
                credentials = connectionInfo.Credentials ?? new Credential("anonymous", "ftpuser");
                serverPath = connectionInfo.ServerPath ?? serverPath;
            }
            else
            {
                server = SftpSettings.DefaultFtpServer;
                port = SftpSettings.DefaultPort ?? 22;
                credentials = SftpSettings.DefaultCredentials ?? new Credential("anonymous", "ftpuser");
                serverPath = serverPath ?? SftpSettings.DefaultServerPath;
            }

            // Get the object used to communicate with the server
            using (var client = new SftpClient(server, port, credentials.UserName, credentials.Password.ToUnsecurePassword()))
            {
                client.Connect();
                action.Invoke(client, serverPath);
            }
        }
    }
}
