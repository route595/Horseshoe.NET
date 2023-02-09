using Horseshoe.NET.ObjectsAndTypes;

namespace Horseshoe.NET.IO.DirectoryCrawler
{
    /// <summary>
    /// A specialized subclass of <c>CrawlOptions</c> which features <c>CopyMode</c> for handling file copy issues.
    /// </summary>
    public class CopyOptions : CrawlOptions
    {
        /// <summary>
        /// Granular settings specifying file copy operation behavior
        /// </summary>
        public CopyMode? CopyMode { get; set; }

        /// <summary>
        /// Creates a new <c>CopyOptions</c>
        /// </summary>
        public CopyOptions() { }

        /// <summary>
        /// Creates a new <c>CopyOptions</c> from a <c>CrawlOptions</c> instance
        /// </summary>
        /// <param name="options">a <c>CrawlOptions</c> instance</param>
        public CopyOptions(CrawlOptions options)
        {
            ObjectUtilAbstractions.MapProperties(options, this);
        }
    }
}

