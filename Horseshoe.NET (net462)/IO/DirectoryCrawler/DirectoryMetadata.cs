using System;

using Horseshoe.NET.ObjectsAndTypes;

namespace Horseshoe.NET.IO.DirectoryCrawler
{
    /// <summary>
    /// Information useful for consumers of the <c>DirectoryCrawler</c> API.
    /// </summary>
    /// <typeparam name="T">matches <c>DirectoryCrawler</c> type param</typeparam>
    public class DirectoryMetadata<T>
    {
        /// <summary>
        /// How far the current directory is up the directory tree compared to the root directory. 
        /// </summary>
        public int Level { get; set; }

        /// <summary>
        /// A reference to the directory traversal engine
        /// </summary>
        public DirectoryCrawler<T> DirectoryCrawler { get; }

        /// <summary>
        /// Whether the <c>DirectoryCrawler</c> is operating in dry run mode
        /// </summary>
        public bool DryRun { get; set; }  // does not affect Exiting

        /// <summary>
        /// The reason the current directory was skipped, if applicable
        /// </summary>
        public SkipReason SkipReason { get; set; }

        /// <summary>
        /// Additional context about the current directory being skipped, if applicable
        /// </summary>
        public string SkipComment { get; set; }

        /// <summary>
        /// The exception, if any, that was thrown by the <c>DirectoryCrawler</c> consumer
        /// </summary>
        public Exception Exception { get; set; }

        /// <summary>
        /// A set of basic file and directory statistics gathered by the directory traversal engine
        /// </summary>
        public DirectoryCrawlStatistics Statistics { get; set; }

        /// <summary>
        /// Creates a new <c>DirectoryMetadata</c> from another instance
        /// </summary>
        /// <param name="metadata">another instance</param>
        public DirectoryMetadata(DirectoryMetadata<T> metadata)
        {
            ObjectUtil.MapProperties(metadata, this);
        }

        /// <summary>
        /// Creates a new <c>DirectoryMetadata</c>
        /// </summary>
        /// <param name="level">how far the current directory is up the directory tree compared to the root directory</param>
        /// <param name="directoryCrawler">a reference to the directory traversal engine</param>
        /// <param name="dryRun">whether the <c>DirectoryCrawler</c> is operating in dry run mode</param>
        /// <param name="skipReason">the reason the current directory was skipped, if applicable</param>
        /// <param name="exception">the exception, if any, that was thrown by the <c>DirectoryCrawler</c> consumer</param>
        /// <param name="statistics">a set of basic file and directory statistics gathered by the directory traversal engine</param>
        public DirectoryMetadata(int level, DirectoryCrawler<T> directoryCrawler, bool dryRun = false, SkipReason skipReason = default, Exception exception = null, DirectoryCrawlStatistics statistics = null)
        {
            Level = level;
            DirectoryCrawler = directoryCrawler;
            DryRun = dryRun;
            Exception = exception;
            SkipReason = skipReason;
            Statistics = statistics;
        }

        /// <summary>
        /// In the "directory entered" phase, a consumer can call this to stop processing the current directory
        /// </summary>
        public void SkipThisDirectory()
        {
            throw new DirectorySkippedException();
        }

        /// <summary>
        /// Stops the directory traversal engine
        /// </summary>
        public void Halt()
        {
            throw new DirectoryCrawlHaltedException();
        }
    }
}
