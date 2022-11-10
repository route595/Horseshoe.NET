namespace Horseshoe.NET.IO.FileFilter
{
    public interface IDirectoryFilter : IFilesystemObjectFilter
    {
        bool IsMatch(DirectoryPath directory);
    }
}
