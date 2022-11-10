using System;

namespace Horseshoe.NET.IO.DirectoryCrawler
{
    public class DirectoryArgs<T>
    {
        public DirectoryPath Directory { get; set; }
        public int Level { get; set; }
        public DirectoryCrawler<T> DirectoryCrawler { get; }
        //public bool SkipDirectory { get; set; }  // does not affect Exiting
        public bool DryRun { get; set; }  // does not affect Exiting
        public SkipReason SkipReason { get; }
        public Exception Exception { get; }

        public DirectoryArgs(DirectoryPath directory, int level, DirectoryCrawler<T> directoryCrawler, bool dryRun = false, SkipReason skipReason = default, Exception exception = null)
        {
            Directory = directory;
            Level = level;
            DirectoryCrawler = directoryCrawler;
            DryRun = dryRun;
            Exception = exception;
            SkipReason = skipReason;
        }

        /// <summary>
        /// In the "directory entered" phase client code can choose to stop processing the current directory effectively skipping to the next.
        /// </summary>
        /// <param name="reason"></param>
        public void SkipThisDirectory(SkipReason reason = SkipReason.ClientSkipped)
        {
            throw new DirectorySkippedException(reason);
        }

        /// <summary>
        /// End file recursion immediately
        /// </summary>
        public void HaltDirectoryCrawler()
        {
            throw new DirectoryCrawlHaltedException();
        }
    }
}
