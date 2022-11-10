namespace Horseshoe.NET.IO.DirectoryCrawler
{
    public delegate void DirectoryCrawledEvent<T>(DirectoryCrawlEventType eventType, DirectoryArgs<T> dirArgs, FileArgs<T> fileArgs, DirectoryCrawlStatistics stats);
}
