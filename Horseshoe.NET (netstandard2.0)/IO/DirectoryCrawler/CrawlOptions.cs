using Horseshoe.NET.IO.FileFilter;

namespace Horseshoe.NET.IO.DirectoryCrawler
{
    public class CrawlOptions
    {
        public string FileSearchPattern { get; set; }
        public IFileFilter FileFilter { get; set; }
        public IDirectoryFilter DirectoryFilter { get; set; }
        public bool DirectoriesOnly { get; set; }
        public bool DryRun { get; set; }
        public bool ReportErrorsAndContinue { get; set; }
    }
}

