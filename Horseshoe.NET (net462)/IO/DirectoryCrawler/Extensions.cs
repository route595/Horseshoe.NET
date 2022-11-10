using System.Collections.Generic;
using System.IO;
using System.Linq;

using Horseshoe.NET.Collections;

namespace Horseshoe.NET.IO.DirectoryCrawler
{
    public static class Extensions
    {
        public static IEnumerable<string> Display(this DirectoryCrawlStatistics stats)
        {
            return new List<string>()
                .Append(                                "Directories Crawled: " + stats.DirectoriesCrawled)
                .AppendIf(stats.DirectoriesSkipped > 0, "Directories Skipped: " + stats.DirectoriesSkipped)
                .Append(                                "Files Crawled:       " + stats.FilesCrawled)
                .AppendIf(stats.FilesSkipped > 0,       "Files Skipped:       " + stats.FilesSkipped)
                .AppendIf(stats.FilesErrored > 0,       "Files Errored:       " + stats.FilesErrored)
                .Append(                                "Total File Size:     " + FileUtil.GetDisplayFileSize(stats.SizeOfFilesCrawled));
        }
    }
}
