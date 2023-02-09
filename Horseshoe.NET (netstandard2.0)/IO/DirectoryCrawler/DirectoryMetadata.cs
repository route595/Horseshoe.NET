using System;

namespace Horseshoe.NET.IO.DirectoryCrawler
{
    /// <summary>
    /// Information useful for consumers of the <c>DirectoryCrawler</c> API.
    /// </summary>
    /// <typeparam name="T">matches <c>DirectoryCrawler</c> type param</typeparam>
    public class DirectoryMetadata<T>
    {
        private T completedResult;
        private bool completedResultWasSet;

        /// <summary>
        /// How far the current directory is up the directory tree compared to the root directory. 
        /// </summary>
        public int Level { get; set; }

        /// <summary>
        /// A reference to the traversal engine.
        /// </summary>
        public DirectoryCrawler<T> DirectoryCrawler { get; }

        /// <summary>
        /// Whether to process files in the current directory.
        /// </summary>
        public bool ProcessFiles { get; set; }

        /// <summary>
        /// Whether to delete files in the current directory.
        /// </summary>
        public bool DeleteFiles { get; set; }

        /// <summary>
        /// Whether to recursively delete the current directory and all its files and subdirectories.
        /// </summary>
        public bool DeleteDirectory { get; set; }

        /// <summary>
        /// Whether to preclude files in the current directory from being processed or deleted.
        /// </summary>
        public bool SkipDirectory { get; set; }

        /// <summary>
        /// The reason the current directory was skipped, if applicable.
        /// </summary>
        public SkipReason SkipReason { get; set; }

        /// <summary>
        /// Whether the <c>DirectoryCrawler</c> is operating in dry run mode, e.g. prevents files and directories from being deleted and implies <c>DirectoryCralwer</c> implementations should not process files and directories.
        /// </summary>
        public bool DryRun { get; set; }  // does not affect Exiting

        /// <summary>
        /// The exception, if any, that was thrown by the <c>DirectoryCrawler</c> consumer.
        /// </summary>
        public Exception Exception { get; set; }

        /// <summary>
        /// A set of basic file and directory statistics optionally gathered by the traversal engine.
        /// </summary>
        public TraversalStatistics Statistics { get; set; }

        /// <summary>
        /// The result of the traversal operation.  Only available during the "on complete" phase, otherwise throws an exception.
        /// </summary>
        public T CompletedResult
        {
            get
            {
                return completedResultWasSet
                    ? completedResult
                    : throw new DirectoryCrawlException("The completed result is not available.");
            }
            set
            {
                completedResult = value;
                completedResultWasSet = true;
            } 
        }

        /// <summary>
        /// Creates a new <c>DirectoryMetadata</c> from another instance.
        /// </summary>
        /// <param name="metadata">another instance</param>
        public DirectoryMetadata(DirectoryMetadata<T> metadata)
        {
            if (metadata != null)
            {
                Level = metadata.Level;
                DirectoryCrawler = metadata.DirectoryCrawler;
                //DeleteDirectory = metadata.DeleteDirectory;
                //SkipDirectory = metadata.SkipDirectory;
                //SkipReason = metadata.SkipReason;
                //SkipComment = metadata.SkipComment;
                DryRun = metadata.DryRun;
                Exception = metadata.Exception;
                Statistics = metadata.Statistics;
            }
        }

        /// <summary>
        /// Creates a new <c>DirectoryMetadata</c>.
        /// </summary>
        /// <param name="level">How far the current directory is up the directory tree compared to the root directory.</param>
        /// <param name="directoryCrawler">A reference to the traversal engine.</param>
        /// <param name="dryRun">Whether the <c>DirectoryCrawler</c> is operating in dry run mode, e.g. prevents files and directories from being deleted and implies <c>DirectoryCralwer</c> implementations should not process files and directories.</param>
        /// <param name="exception">The exception, if any, that was thrown by the <c>DirectoryCrawler</c> consumer.</param>
        /// <param name="statistics">A set of basic file and directory statistics optionally gathered by the traversal engine.</param>
        public DirectoryMetadata(int level, DirectoryCrawler<T> directoryCrawler, bool dryRun = false, Exception exception = null, TraversalStatistics statistics = null)
        {
            Level = level;
            DirectoryCrawler = directoryCrawler;
            //DeleteDirectory = metadata.DeleteDirectory;
            //SkipDirectory = metadata.SkipDirectory;
            //SkipReason = metadata.SkipReason;
            //SkipComment = metadata.SkipComment;
            DryRun = dryRun;
            Exception = exception;
            Statistics = statistics;
        }

        /// <summary>
        /// Signals the traversal engine to recursively delete the subdirectories and files of the current directory.
        /// </summary>
        public void DeleteThisDirectory()
        {
            DeleteDirectory = true;
        }

        /// <summary>
        /// Signals the traversal engine not process or delete this directory.
        /// </summary>
        public void SkipThisDirectory(SkipReason skipReason)
        {
            SkipDirectory = true;
            SkipReason = skipReason;
        }

        /// <summary>
        /// Stops the traversal engine.
        /// </summary>
        public void Halt()
        {
            throw new DirectoryCrawlHaltedException();
        }
    }
}
