namespace Horseshoe.NET.IO.DirectoryCrawler
{
    /// <summary>
    /// An exception, raised when consumers skip a file, that is handled by the system (the definition of a benign exception)
    /// </summary>
    public class FileSkippedException : BenignException
    {
        /// <summary>
        /// The reason the current file was skipped, if applicable
        /// </summary>
        public SkipReason SkipReason { get; }

        /// <summary>
        /// Additional context about the current file being skipped, if applicable
        /// </summary>
        public string SkipComment { get; }

        /// <summary>
        /// Creates a new <c>FileSkippedException</c>
        /// </summary>
        /// <param name="skipReason">the reason the current file was skipped, if applicable</param>
        /// <param name="skipComment">additional context about the current file being skipped, if applicable</param>
        public FileSkippedException(SkipReason skipReason, string skipComment = null) : base("File skipped...")
        {
            SkipReason = skipReason;
            SkipComment = skipComment;
        }
    }
}
