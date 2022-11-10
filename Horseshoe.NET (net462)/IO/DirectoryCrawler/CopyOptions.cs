using Horseshoe.NET.Objects;

namespace Horseshoe.NET.IO.DirectoryCrawler
{
    public class CopyOptions : CrawlOptions
    {
        public CopyMode? CopyMode { get; set; }

        public bool DeleteDestinationsWithNoMatchingSources { get; set; }

        public CopyOptions() { }

        public CopyOptions(CrawlOptions options)
        {
            ObjectUtil.MapProperties(options, this);
            //DirectoriesOnly = options.DirectoriesOnly;
            //FileSearchPattern = options.FileSearchPattern;
            //DryRun = options.DryRun;
        }
    }
}

