namespace Horseshoe.NET.IO.DirectoryCrawler
{
    /// <summary>
    /// An exception, raised when consumers halt the directory traversal engine, that is handled by the system (the definition of a benign exception)
    /// </summary>
    public class DirectoryCrawlHaltedException : BenignException
    {
        /// <summary>
        /// Creates a new <c>DirectoryCrawlHaltedException</c>
        /// </summary>
        public DirectoryCrawlHaltedException() : base("Directory crawl halted...") { }
    }
}
