namespace Horseshoe.NET.IO.FileFilter
{
    public interface IFilesystemObjectFilter
    {
        bool CaseSensitive { get; }

        FilterMode FilterMode { get; }
    }
}
