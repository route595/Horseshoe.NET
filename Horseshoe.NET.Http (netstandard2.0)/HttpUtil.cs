using System;

using Horseshoe.NET.IO;

namespace Horseshoe.NET.Http
{
    public static class HttpUtil
    {
        public static string ParseFileNameFromURL(string url, string nameIfBlank, FileType? fileType)
        {
            return ParseFileNameFromURL(url, nameIfBlank, fileType?.ToString().ToLower());
        }

        public static string ParseFileNameFromURL(string url, string nameIfBlank, string extension)
        {
            if (url == null) return null;
            if (url.Contains(":///"))                             // file:///share/dir/subdir/file.txt
            {
                url = url.Substring(url.IndexOf(":///") + 4);     // -> share/dir/subdir/file.txt
            }
            else if (url.Contains("://"))                         // http://example.com/FileSelect.aspx?filepath=dir/subdir/file.txt
            {
                url = url.Substring(url.IndexOf("://") + 3);      // -> example.com/FileSelect.aspx?filepath=dir/subdir/file.txt
            }
            if (url.Contains("?"))
            {
                url = url.Substring(0, url.IndexOf("?"));         // -> example.com/FileSelect.aspx
            }
            if (url.EndsWith("/"))
            {
                url = url.Substring(0, url.Length - 1);           // example.com/apps/FileSelect/ -> example.com/apps/FileSelect
            }
            if (url.Contains("/"))
            {
                url = url.Substring(url.LastIndexOf("/") + 1);    // example.com/FileSelect.aspx -> FileSelect.aspx
            }
            if (url.Length == 0)
            {
                if (string.IsNullOrWhiteSpace(nameIfBlank)) return "";
                url = nameIfBlank;
            }
            if (url.Length > 0 && !string.IsNullOrWhiteSpace(extension))
            {
                var loext = extension.ToLower();
                var lourl = url.ToLower();
                switch (loext)
                {
                    case "jpg":
                    case "jpeg":
                        if (!(lourl.EndsWith(".jpg") || lourl.EndsWith(".jpeg")))
                        {
                            url += "." + loext;
                        }
                        break;
                    case "mpg":
                    case "mpeg":
                        if (!(lourl.EndsWith(".mpg") || lourl.EndsWith(".mpeg")))
                        {
                            url += "." + loext;
                        }
                        break;
                    case "tif":
                    case "tiff":
                        if (!(lourl.EndsWith(".tif") || lourl.EndsWith(".tiff")))
                        {
                            url += "." + loext;
                        }
                        break;
                    default:
                        if (!lourl.EndsWith("." + loext))
                        {
                            url += "." + loext;
                        }
                        break;
                }
            }
            return url;
        }
    }
}
