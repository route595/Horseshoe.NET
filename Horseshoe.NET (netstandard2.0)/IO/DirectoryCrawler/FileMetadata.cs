using System;

using Horseshoe.NET.ObjectsAndTypes;

namespace Horseshoe.NET.IO.DirectoryCrawler
{
    /// <summary>
    /// Information useful for consumers of the <c>DirectoryCrawler</c> API.
    /// </summary>
    /// <typeparam name="T">matches <c>DirectoryCrawler</c> type param</typeparam>
    public class FileMetadata<T>
    {
        /// <summary>
        /// How far the current file is up the directory tree compared to the root directory. 
        /// </summary>
        public int Level { get; }

        /// <summary>
        /// A reference to the traversal engine.
        /// </summary>
        public DirectoryCrawler<T> DirectoryCrawler { get; }

        /// <summary>
        /// Indicates whether this file should be skipped and tallies it appropriately.
        /// </summary>
        /// <remarks>
        /// Files are tallied only if statistics is turned on.
        /// </remarks>
        public bool SkipFile { get; set; }

        /// <summary>
        /// The reason the current file is being skipped, if applicable.
        /// </summary>
        public SkipReason SkipReason { get; set; }

        /// <summary>
        /// In the "file found" phase if <c>true</c>, this tells Horseshoe.NET to delete the file and tally it appropriately.
        /// </summary>
        /// <remarks>
        /// Files are tallied only if statistics is turned on.
        /// </remarks>
        public bool DeleteFile { get; set; }

        /// <summary>
        /// Whether the <c>DirectoryCrawler</c> is operating in dry run mode, e.g. prevents files and directories from being deleted and implies <c>DirectoryCralwer</c> implementations should not process files and directories.
        /// </summary>
        public bool DryRun { get; }  // does not apply to Exiting

        /// <summary>
        /// The exception, if any, that was thrown by the <c>DirectoryCrawler</c> consumer
        /// </summary>
        public Exception Exception { get; set; }

        /// <summary>
        /// A set of basic file and directory statistics gathered by the traversal engine.
        /// </summary>
        public TraversalStatistics Statistics { get; set; }

        /// <summary>
        /// Creates a new <c>FileMetadata</c> from another instance
        /// </summary>
        /// <param name="metadata">another instance</param>
        public FileMetadata(FileMetadata<T> metadata)
        {
            Level = metadata.Level;
            DirectoryCrawler = metadata.DirectoryCrawler;
            //SkipFile = metadata.SkipFile;
            //SkipReason = metadata.SkipReason;
            //DeleteFile = metadata.DeleteFile;
            DryRun = metadata.DryRun;
            // Exception = metadata.Exception;
            Statistics = metadata.Statistics;
        }

        /// <summary>
        /// Creates a new <c>FileMetadata</c>.
        /// </summary>
        /// <param name="level">How far the current file is up the directory tree compared to the root directory.</param>
        /// <param name="directoryCrawler">A reference to the traversal engine.</param>
        /// <param name="dryRun">Whether the <c>DirectoryCrawler</c> is operating in dry run mode.</param>
        /// <param name="exception">The exception, if any, that was thrown by the <c>DirectoryCrawler</c> consumer.</param>
        /// <param name="statistics">A set of basic file and directory statistics optionally gathered by the traversal engine.</param>
        public FileMetadata(int level, DirectoryCrawler<T> directoryCrawler, bool dryRun = false, Exception exception = null, TraversalStatistics statistics = null)
        {
            Level = level;
            DirectoryCrawler = directoryCrawler;
            DryRun = dryRun;
            Exception = exception;
            Statistics = statistics;
        }

        /// <summary>
        /// In the "file found" phase, this tallies the file as "skipped" instead.
        /// </summary>
        /// <param name="reason">The reason the current file was skipped, if applicable.</param>
        /// <remarks>
        /// Files are tallied only if statistics is turned on.
        /// </remarks>
        public void SkipThisFile(SkipReason reason = SkipReason.ClientSkipped)
        {
            SkipFile = true;
            SkipReason = reason;
        }

        /// <summary>
        /// In the "file processing" phase, this tells Horseshoe.NET to delete the file and tally it appropriately.
        /// </summary>
        /// <remarks>
        /// Files are tallied only if statistics is turned on.
        /// </remarks>
        public void DeleteThisFile()
        {
            DeleteFile = true;
        }

        /// <summary>
        /// Stops the traversal engine.
        /// </summary>
        public void Halt()
        {
            throw new DirectoryCrawlHaltedException();
        }

        /// <summary>
        /// Creates a shallow copy of the current <c>FileMetadata&lt;T&gt;</c>.
        /// </summary>
        /// <returns>A shallow copy of the current <c>FileMetadata&lt;T&gt;</c>.</returns>
        public FileMetadata<T> Clone()
        {
            return MemberwiseClone() as FileMetadata<T>;
        }
    }
}
