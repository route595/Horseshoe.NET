using Horseshoe.NET.IO.FileFilter;
using Horseshoe.NET.ObjectsAndTypes;

namespace Horseshoe.NET.IO.DirectoryCrawler
{
    /// <summary>
    /// Settings specifying directory traversal engine behavior 
    /// </summary>
    public class CrawlOptions
    {
        /// <summary>
        /// A quick and dirty file filter used by the directory traversal engine.  
        /// Be advised, this file filtering method does not reflect in the skipped file count statistic.
        /// </summary>
        public string FileSearchPattern { get; set; }

        /// <summary>
        /// A file filter option that reflects in the skipped file count statistic.
        /// </summary>
        public FileFilter.FileFilter FileFilter { get; set; }

        /// <summary>
        /// A quick and dirty directory filter used by the directory traversal engine.  
        /// Be advised, this directory filtering method does not reflect in the skipped directory count statistic.
        /// </summary>
        public string DirectorySearchPattern { get; set; }

        /// <summary>
        /// A directory filter option that reflects in the skipped directory count statistic.
        /// </summary>
        public DirectoryFilter DirectoryFilter { get; set; }

        /// <summary>
        /// This optimizes the directory traversal engine for operations that only care about directories.
        /// </summary>
        public bool DirectoriesOnly { get; set; }

        internal bool AutoSkipRootDirectoryExited { get; set; }

        /// <summary>
        /// Allows the directory traversal engine to run in theoretical mode meaning that files and directories are not actually created, modified, deleted or moved.
        /// </summary>
        public bool DryRun { get; set; }

        /// <summary>
        /// Precludes exceptions from halting the directory traveral engine
        /// </summary>
        public bool ReportErrorsAndContinue { get; set; }

        /// <summary>
        /// Creates an new <c>CrawlOptions</c>
        /// </summary>
        public CrawlOptions()
        {
        }

        /// <summary>
        /// Creates an new <c>CrawlOptions</c> from anothe instance
        /// </summary>
        public CrawlOptions(CrawlOptions options)
        {
            if (options != null)
            {
                ObjectUtil.MapProperties(options, this);
            }
        }
    }
}

