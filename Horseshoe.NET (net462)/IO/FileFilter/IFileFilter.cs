namespace Horseshoe.NET.IO.FileFilter
{
    public interface IFileFilter : IFilesystemObjectFilter
    {
        bool IsMatch(FilePath file);
    }
}
