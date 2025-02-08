![Horseshoe.NET icon](https://raw.githubusercontent.com/route595/Horseshoe.NET/refs/heads/main/assets/images/horseshoe-icon-128x128.png)

# Horseshoe.NET.SecureIO

Adds SFTP to the Horseshoe.NET suite (uses SSH.NET)

## Code Examples

```c#
pseudoConnectionString = "sftp://username@11.22.33.44//root/subdir?password=PA$$w0rd";

var dirContents = Sftp.ListDirectoryContents
(
    fileMask: FtpFileMasks.Txt,           // or "*.txt"
    connectionInfo: pseudoConnectionString
);
foreach (var fileName in dirContents)
    Console.WriteLine(fileName);

Sftp.CreateFile
(
    "hello.txt",                          // remote dest file name
    "Hello World!",                       // file contents
    connectionInfo: pseudoConnectionString
);

Sftp.UploadFile
(
    "C:\\hello.txt",                      // local src file path
    connectionInfo: pseudoConnectionString
);

var stream = Sftp.DownloadFile
(
    "hello.txt",                          // remote src file name
    connectionInfo: pseudoConnectionString
);
Console.WriteLine("File length: " + stream.Length);
Console.WriteLine("File contents: " + encoding.GetString(stream.ToArray()));

Sftp.DeleteFile
(
    "hello.txt",                          // remote src file name
    connectionInfo: pseudoConnectionString
);

```
