namespace Horseshoe.NET.IO.FileFilter
{
    public class FileSizeFilter : IFileFilter
    {
        public long? MinSize { get; }

        public long? MaxSize { get; }

        public bool CaseSensitive => false;

        public FilterMode FilterMode { get; }  // implements interface, otherwise serves no purpose

        public FileSizeFilter(long? minSize, long? maxSize)
        {
            if (!minSize.HasValue && !maxSize.HasValue)
                throw new ValidationException("Invalid filter: a min size or max size (or both) must be supplied");
            //if (minSize > maxSize)
            //    throw new ValidationException("Invalid filter: min size cannot exceed max size");
            MinSize = minSize;
            MaxSize = maxSize;
        }

        public bool IsMatch(FilePath file)
        {
            if (MinSize.HasValue)
            {
                if (MaxSize < MinSize)  // account for switched min/max values
                    return file.Length <= MaxSize || file.Length >= MinSize;
                if (file.Length < MinSize)
                    return false;
            }
            if (MaxSize.HasValue && file.Length > MaxSize)
                return false;
            return true;
        }
    }
}
