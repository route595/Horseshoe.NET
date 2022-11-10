namespace Horseshoe.NET.IO.DirectoryCrawler
{
    public enum DirectoryCrawlEventType
    {
        FileFound,
        FileSkipped,
        FileErrored,
        DirectoryEntered,
        DirectoryExited,
        DirectorySkipped,
        DirectoryErrored,
        DirectoryCrawlHalted
    }
}
