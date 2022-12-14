namespace Horseshoe.NET.IO.DirectoryCrawler
{
    /// <summary>
    /// The reason a file or directory was skipped.  For reporting purposes and troubleshooting.
    /// </summary>
    public enum SkipReason
    {
        /// <summary>
        /// The default value
        /// </summary>
        NA,

        /// <summary>
        /// Calling code supplied a filter that resulted in the file or directory in question being skipped
        /// </summary>
        ClientFiltered,

        /// <summary>
        /// Calling code specifically requested to skip the file or directory in question (see <see cref="DirectoryMetadata{T}.SkipThisDirectory"/> and <see cref="FileMetadata{T}.SkipThisFile(SkipReason, string)" />)
        /// </summary>
        ClientSkipped,

        /// <summary>
        /// A special case applied here and the root directory was automatically skipped
        /// </summary>
        AutoSkipped,

        /// <summary>
        /// A file copy was skipped due to the destination file already exists
        /// </summary>
        AlreadyExists
    }
}
