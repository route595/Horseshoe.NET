using System;

namespace Horseshoe.NET.IO.DirectoryCrawler
{
    public class FileArgs<T>
    {
        public FilePath File { get; }
        public int Level { get; }
        public DirectoryCrawler<T> DirectoryCrawler { get; }
        //public bool SkipFile { get; set; }  // does not apply to Exiting
        public bool DryRun { get; }  // does not apply to Exiting
        public SkipReason SkipReason { get; }
        public Exception Exception { get; }

        public FileArgs(FilePath file, int level, DirectoryCrawler<T> directoryCrawl, bool dryRun = false, SkipReason skipReason = default, Exception exception = null)
        {
            File = file;
            Level = level;
            DirectoryCrawler = directoryCrawl;
            DryRun = dryRun;
            SkipReason = skipReason;
            Exception = exception;
        }

        /// <summary>
        /// In the "directory entered" phase client code can choose to stop processing the current directory effectively skipping to the next.
        /// </summary>
        /// <param name="reason"></param>
        public void SkipThisFile(SkipReason reason = SkipReason.ClientSkipped)
        {
            throw new FileSkippedException(reason);
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
