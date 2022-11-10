using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Horseshoe.NET.Ftp
{
    public delegate void RequestUriCreated(string uri); 
    public delegate void FileUploaded(string fileName, long fileSize, int statusCode, string statusDescription);
    public delegate void FileDownloaded(string fileName, long fileSize, int statusCode, string statusDescription);
    public delegate void DirectoryContentsListed(int count, int statusCode, string statusDescription);
    public delegate void FileDeleted(string fileName, int statusCode, string statusDescription);
}
