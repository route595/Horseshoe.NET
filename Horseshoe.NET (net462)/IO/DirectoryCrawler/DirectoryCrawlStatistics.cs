namespace Horseshoe.NET.IO.DirectoryCrawler
{
    /// <summary>
    /// A set of basic file and directory statistics gathered by the directory traversal engine
    /// </summary>
    public class DirectoryCrawlStatistics
    {
        /// <summary>
        /// Reports how many directories were encountered, minus any skipped
        /// </summary>
        public int DirectoriesCrawled { get; internal set; }

        /// <summary>
        /// Reports how many directories were skipped
        /// </summary>
        public int DirectoriesSkipped { get; internal set; }

        /// <summary>
        /// Reports how many directories resulted in exceptions
        /// </summary>
        public int DirectoriesErrored { get; internal set; }

        /// <summary>
        /// Reports how many files were encountered, minus any skipped
        /// </summary>
        public int FilesCrawled { get; internal set; }

        /// <summary>
        /// Reports how many files were skipped
        /// </summary>
        public int FilesSkipped { get; internal set; }

        /// <summary>
        /// Reports how many files resulted in exceptions
        /// </summary>
        public int FilesErrored { get; internal set; }

        /// <summary>
        /// Reports the total size of all files found during the traversal operation, not including any skipped
        /// </summary>
        public long SizeOfFilesCrawled { get; internal set; }
    }

}
