using System.Collections.Generic;

using Horseshoe.NET.Collections;

namespace Horseshoe.NET.IO.DirectoryCrawler
{
    /// <summary>
    /// A set of extenion methods for <c>DirectoryCrawler</c>
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Generates a printable list of the statistics
        /// </summary>
        /// <param name="stats">a set of basic file and directory statistics gathered by the directory traversal engine</param>
        /// <returns></returns>
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
