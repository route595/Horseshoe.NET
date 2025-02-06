![Horseshoe.NET icon](https://raw.githubusercontent.com/route595/Horseshoe.NET/refs/heads/main/assets/images/horseshoe-icon-128x128.png)

# Horseshoe.NET.SecureIO

Adds SFTP to the Horseshoe.NET suite (uses SSH.NET)

## Code examples

```
pseudoConnectionString = "sftp://username@11.22.33.44//root/subdir?password=PA$$w0rd";

var dirContents = Sftp.ListDirectoryContents
(
	fileMask: FtpFileMasks.Txt,
	connectionInfo: SftpUtil.ParseSftpConnectionString(pseudoConnectionString)
);
foreach (var fileName in dirContents)
    Console.WriteLine(fileName);

Sftp.UploadFile
(
	"hello.txt",
	"Hello World!",
	connectionInfo: SftpUtil.ParseSftpConnectionString(pseudoConnectionString)
);

var stream = Sftp.DownloadFile
(
	"hello.txt",
	connectionInfo: SftpUtil.ParseSftpConnectionString(pseudoConnectionString)
);
Console.WriteLine("File length: " + stream.Length);
Console.WriteLine("File contents: " + encoding.GetString(stream.ToArray()));

Sftp.DeleteFile
(
	"hello.txt",
	connectionInfo: SftpUtil.ParseSftpConnectionString(pseudoConnectionString)
);

```
