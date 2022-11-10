namespace Horseshoe.NET.IO.DirectoryCrawler
{
    public class RecursiveSize : DirectoryCrawler<long>
    {
        public RecursiveSize
        (
            DirectoryPath root,
            DirectoryCrawledEvent<long> directoryCrawled = null,
            CrawlOptions options = null
        ) : base
        (
            root,
            directoryCrawled: directoryCrawled,
            options: options
        )
        {
        }

        public override long CrawlComplete()
        {
            return Statistics.SizeOfFilesCrawled;
        }
    }
}

