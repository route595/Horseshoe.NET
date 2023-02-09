using System;
using System.Collections.Generic;
using System.Linq;

using Horseshoe.NET.Collections;
using Horseshoe.NET.Text;

namespace Horseshoe.NET.IO.DirectoryCrawler
{
    /// <summary>
    /// A set of extenion methods for <c>DirectoryCrawler</c>
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Generates a <c>string</c> list of the main statistics of interest.
        /// </summary>
        /// <param name="stats">A customizable set of file and directory statistics gathered by the directory traversal engine.</param>
        /// <returns></returns>
        public static IEnumerable<string> ToCuratedList(this TraversalStatistics stats)
        {
            var list = new List<string>()
                .Append(                                "Directories Entered: " + stats.DirectoriesProcessed)
                .AppendIf(stats.DirectoriesDeleted > 0, "Directories Deleted: " + stats.DirectoriesDeleted)
                .AppendIf(stats.DirectoriesErrored > 0, "Directories Errored: " + stats.DirectoriesErrored)
                .Append(                                "Total Directories:   " + stats.TotalDirectories)
                .Append(                                "Files Processed:     " + stats.FilesProcessed + " (" + FileUtil.GetDisplayFileSize(stats.SizeOfFilesProcessed) + ")")
                .AppendIf(stats.FilesDeleted > 0,       "Files Deleted:       " + stats.FilesDeleted + " (" + FileUtil.GetDisplayFileSize(stats.SizeOfFilesDeleted) + ")")
                .AppendIf(stats.FilesErrored > 0,       "Files Errored:       " + stats.FilesErrored)
                .Append(                                "Total Files:         " + stats.TotalFiles + " (" + FileUtil.GetDisplayFileSize(stats.TotalSizeOfFiles) + ")");
            list.AddRange
            (
                stats.Keys
                    .Where(k => !k.StartsWith("TraversalStatistics."))
                    .Select(k => TextUtil.Fit(TextUtil.SpaceOutTitleCase(k.Contains(".") ? k.Substring(k.LastIndexOf(".") + 1) : k) + ": ", 26) + stats[k])
            );
            return list;
        }

        /// <summary>
        /// Generates a printable, new-line separated <c>string</c> of the main statistics of interest.
        /// </summary>
        /// <param name="stats">A customizable set of file and directory statistics gathered by the directory traversal engine.</param>
        /// <param name="indent">The number of spaces to indent each line.</param>
        /// <param name="padBefore">The number of newlines to render at the beginning of the display <c>string</c>.</param>
        /// <param name="padAfter">The number of newlines to render at the end of the display <c>string</c>.</param>
        /// <returns></returns>
        public static string DisplayCurated(this TraversalStatistics stats, int indent = 0, int padBefore = 0, int padAfter = 0)
        {
            var list = ToCuratedList(stats) as List<string>;
            list.Insert(0, "Statistics");
            list.Insert(1, "----------");
            for (int i = 0; i < padBefore; i++)
            {
                list.Insert(0, "");
            }
            for (int i = 0; i < padAfter; i++)
            {
                list.Add("");
            }
            return string.Join(Environment.NewLine, indent > 0 ? list.Select(s => new string(' ', indent) + s) : list);
        }
    }
}
