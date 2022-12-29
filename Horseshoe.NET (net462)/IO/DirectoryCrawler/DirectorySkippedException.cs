namespace Horseshoe.NET.IO.DirectoryCrawler
{
    /// <summary>
    /// An exception, raised when consumers skip a directory, that is handled by the system (the definition of a benign exception)
    /// </summary>
    public class DirectorySkippedException : BenignException
    {
        /// <summary>
        /// Creates a new <c>DirectorySkippedException</c>
        /// </summary>
        public DirectorySkippedException() : base("Directory skipped...")
        {
        }
    }
}
