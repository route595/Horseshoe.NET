using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using Horseshoe.NET.ConsoleX;
using Horseshoe.NET.Ftp;
using Horseshoe.NET.IO;
using Horseshoe.NET.SecureIO.Sftp;

namespace TestConsole
{
    class FTPTests : RoutineX
    {
        public override IList<MenuObject> Menu => new MenuObject[]
        {
            new MenuHeader("FTP Tests"),
            BuildMenuRoutine
            (
                "Test FTP Upload",
                () =>
                {
                    Ftp.UploadFile
                    (
                        "hello.txt",
                        "Hello World!",
                        connectionInfo: FtpUtil.ParseFtpConnectionString(ftpPseudoConnectionString)
                    );
                }
            ),
            BuildMenuRoutine
            (
                "Test FTPS Upload",
                () =>
                {
                    Ftp.UploadFile
                    (
                        "hello.txt",
                        "Hello World!",
                        connectionInfo: FtpUtil.ParseFtpConnectionString(ftpPseudoConnectionString.Replace("ftp://", "ftps://"))
                    );
                }
            ),
            BuildMenuRoutine
            (
                "Test FTP Download",
                () =>
                {
                    var stream = Ftp.DownloadFile
                    (
                        "hello.txt",
                        connectionInfo: FtpUtil.ParseFtpConnectionString(ftpPseudoConnectionString)
                    );
                    Console.WriteLine("File length: " + stream.Length);
                    Console.WriteLine("File contents: " + Encoding.Default.GetString(stream.ToArray()));
                }
            ),
            BuildMenuRoutine
            (
                "Test FTP Delete",
                () =>
                {
                    Ftp.DeleteFile
                    (
                        "hello.txt",
                        connectionInfo: FtpUtil.ParseFtpConnectionString(ftpPseudoConnectionString)
                    );
                }
            ),
            BuildMenuRoutine
            (
                "List FTP Directory",
                () =>
                {
                    var dirContents = Ftp.ListDirectoryContents
                    (
                        connectionInfo: FtpUtil.ParseFtpConnectionString(ftpPseudoConnectionString)
                    );
                    Console.WriteLine("Directory contents:");
                    Console.WriteLine(dirContents.Any() ? string.Join(Environment.NewLine, dirContents) : "[0 results]");
                    Console.WriteLine();

                    dirContents = Ftp.ListDirectoryContents
                    (
                        fileMask: FtpFileMasks.Txt,
                        connectionInfo: FtpUtil.ParseFtpConnectionString(ftpPseudoConnectionString)
                    );
                    Console.WriteLine("Directory contents (.txt files only):");
                    Console.WriteLine(dirContents.Any() ? string.Join(Environment.NewLine, dirContents) : "[0 results]");
                }
            ),
            new MenuHeader("SFTP Tests"),
            BuildMenuRoutine
            (
                "Test SFTP Upload",
                () =>
                {
                    Sftp.UploadFile
                    (
                        "hello.txt",
                        "Hello World!",
                        connectionInfo: SftpUtil.ParseSftpConnectionString(sftpPseudoConnectionString)
                    );
                }
            ),
            BuildMenuRoutine
            (
                "Test SFTP Download",
                () =>
                {
                    var sstream = Sftp.DownloadFile
                    (
                        "hello.txt",
                        connectionInfo: SftpUtil.ParseSftpConnectionString(sftpPseudoConnectionString)
                    );
                    Console.WriteLine("File length: " + sstream.Length);
                    Console.WriteLine("File contents: " + Encoding.Default.GetString(sstream.ToArray()));
                }
            ),
            BuildMenuRoutine
            (
                "Test SFTP Delete",
                () =>
                {
                    Sftp.DeleteFile
                    (
                        "hello.txt",
                        connectionInfo: SftpUtil.ParseSftpConnectionString(sftpPseudoConnectionString)
                    );
                }
            ),
            BuildMenuRoutine
            (
                "List SFTP Directory",
                () =>
                {
                    var sdirContents = Sftp.ListDirectoryContents
                    (
                        connectionInfo: SftpUtil.ParseSftpConnectionString(sftpPseudoConnectionString)
                    );
                    Console.WriteLine("Directory contents:");
                    Console.WriteLine(string.Join(Environment.NewLine, sdirContents));
                    Console.WriteLine();

                    var dirContents = Sftp.ListDirectoryContents
                    (
                        fileMask: FtpFileMasks.Txt,
                        connectionInfo: SftpUtil.ParseSftpConnectionString(sftpPseudoConnectionString)
                    );
                    Console.WriteLine("Directory contents (.txt files only):");
                    Console.WriteLine(string.Join(Environment.NewLine, dirContents));
                }
            ),
            new MenuHeader("Miscellaneous Tests"),
            BuildMenuRoutine
            (
                "regex",
                () =>
                {
                    var regexes = new[] { "^[^.]+$", "^[^\\.]+$" };
                    var testStrings = new[] { "file.txt", "DIR" };
                    foreach (var regex in regexes)
                    {
                        foreach (var str in testStrings)
                        {
                            Console.WriteLine("Testing '" + str + "' against '" + regex + "' -> " + Regex.IsMatch(str, regex));
                        }
                    }
                }
            ),
            BuildMenuRoutine
            (
                "FTP connection string parse tests",
                () =>
                {
                    var connStrs = new[]
                    {
                        "ftp://george@11.22.33.44/dir/subdir?encryptedPassword=akdj$8iO(d@1sd==",
                        "ftps://george@11.22.33.44:9001//root/subdir?encryptedPassword=akdj$8iO(d@1sd==",
                        "george@11.22.33.44/dir/subdir?encryptedPassword=akdj$8iO(d@1sd==",
                    };
                    foreach(var str in connStrs)
                    {
                        Console.WriteLine("Imported:  " + str);
                        Console.Write("Exporting: ");
                        try
                        {
                            var connectionInfo = FtpUtil.ParseFtpConnectionString(str);
                            Console.WriteLine(connectionInfo);
                        }
                        catch(Exception ex)
                        {
                            Console.WriteLine("error: " + ex.Message);
                        }
                        Console.WriteLine();
                    }
                }
            )
        };

        static readonly string ftpPseudoConnectionString = "ftp://username@11.22.33.44/dir/subdir?password=password";
        static readonly string sftpPseudoConnectionString = "sftp://username@11.22.33.44//root/subdir?password=password";

        static FTPTests()
        {
            Ftp.RequestUriCreated += (uri) => Console.WriteLine("URI: " + uri);
            Ftp.FileUploaded += (fileName, fileSize, statusCode, statusDescription) => Console.WriteLine("Upload results: " + fileName + " - " + FileUtil.GetDisplayFileSize(fileSize) + " - " + statusDescription);
            Ftp.FileDeleted += (fileName, statusCode, statusDescription) => Console.WriteLine("Delete results: " + fileName + " - " + statusDescription);
            Ftp.DirectoryContentsListed += (count, statusCode, statusDescription) => Console.WriteLine("Dir listing results: x" + count + " - " + statusDescription);
        }
    }
}
