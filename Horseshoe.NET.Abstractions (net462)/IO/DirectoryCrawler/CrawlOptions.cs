using Horseshoe.NET.IO.FileFilter;
using Horseshoe.NET.ObjectsAndTypes;

namespace Horseshoe.NET.IO.DirectoryCrawler
{
    /// <summary>
    /// Settings specifying traversal engine behavior.
    /// </summary>
    public class CrawlOptions
    {
        /// <summary>
        /// Dictates the basic behavior of the current traversal session.  Clients can filter the affected files and directories and 
        /// client code can interact with metadata to alter this behavior on a per-directory basis. 
        /// </summary>
        public CrawlMode CrawlMode { get; set; }

        /// <summary>
        /// If <c>true</c>, the traversal engine will keep track of file and folder counts and file sizes, default is <c>false</c>.
        /// </summary>
        /// <remarks>
        /// Note: The directory and file crawl events always include a <see cref="TraversalStatistics"/> argument.
        /// If statistics is not on it will simply contain zeroes.
        /// </remarks>
        public bool StatisticsOn { get; set; }

        /// <summary>
        /// A quick and dirty file filter used by the traversal engine.  
        /// Be advised, this filtering method precludes skipped files from being tallied.
        /// </summary>
        public string FileSearchPattern { get; set; }

        /// <summary>
        /// A file filter.
        /// </summary>
        /// <remarks>
        /// When files are skipped using this method they are still tallied if statistics is turned on.
        /// </remarks>
        public FileFilter.FileFilter FileFilter { get; set; }

        /// <summary>
        /// A quick and dirty directory filter used by the traversal engine.  
        /// Be advised, this filtering method precludes skipped directories from being tallied.
        /// </summary>
        public string DirectorySearchPattern { get; set; }

        /// <summary>
        /// A directory filter.
        /// </summary>
        /// <remarks>
        /// When directories are skipped using this method they are still tallied if statistics is turned on.
        /// </remarks>
        public DirectoryFilter DirectoryFilter { get; set; }

        /// <summary>
        /// This optimizes the traversal engine by only scanning directories.
        /// </summary>
        public bool DirectoriesOnly { get; set; }

        /// <summary>
        /// Whether the <c>DirectoryCrawler</c> is operating in dry run mode, e.g. prevents files and directories from being deleted and implies <c>DirectoryCralwer</c> implementations should not process files and directories.
        /// </summary>
        public bool DryRun { get; set; }

        /// <summary>
        /// Precludes exceptions from halting the traversal engine.
        /// </summary>
        public bool ReportErrorsAndContinue { get; set; }

        /// <summary>
        /// Creates a new <c>CrawlOptions</c>.
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
                ObjectUtilAbstractions.MapProperties(options, this);
            }
        }
    }
}

