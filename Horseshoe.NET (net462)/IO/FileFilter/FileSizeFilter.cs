namespace Horseshoe.NET.IO.FileFilter
{
    /// <summary>
    /// A <c>FileFilter</c> implementation for filtering based on file size
    /// </summary>
    public class FileSizeFilter : FileFilter
    {
        /// <summary>
        /// Minimum size.  If omitted, all file sizes less than or equal the maximum file size are a match.
        /// </summary>
        public long? MinSize { get; }

        /// <summary>
        /// Maximum size.  If omitted, all file sizes greater than or equal the minimum file size are a match.
        /// </summary>
        public long? MaxSize { get; }

        /// <summary>
        /// Creates a new <c>FileSizeFilter</c>
        /// </summary>
        /// <param name="minSize">Minimum size.  If omitted, all file sizes less than or equal the maximum file size are a match.</param>
        /// <param name="maxSize">Maximum size.  If omitted, all file sizes greater than or equal the minimum file size are a match.</param>
        /// <param name="filterMode">Optional, dictates which items to include based on criteria matching.</param>
        /// <exception cref="ValidationException"></exception>
        public FileSizeFilter(long? minSize, long? maxSize, FilterMode filterMode = default)
        {
            // validation
            if (minSize.HasValue)
            {
                if (maxSize.HasValue)
                {
                    if (minSize > maxSize)
                        throw new ValidationException("Invalid filter: min size cannot exceed max size");
                }
            }
            else if (!maxSize.HasValue)
                throw new ValidationException("Invalid filter: a min size or max size (or both) must be supplied");

            MinSize = minSize;
            MaxSize = maxSize;
            FilterMode = filterMode;
        }

        /// <summary>
        /// Indicates whether the supplied file constitutes a critea match.
        /// </summary>
        /// <param name="file">a file path</param>
        /// <returns><c>true</c> or <c>false</c></returns>
        public override bool IsMatch(FilePath file)
        {
            return (!MinSize.HasValue || file.Size >= MinSize) && (!MaxSize.HasValue || file.Size <= MaxSize);
        }
    }
}
