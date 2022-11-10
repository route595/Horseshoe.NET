namespace Horseshoe.NET.IO.DirectoryCrawler
{
    public class DirectoryCrawlStatistics
    {
        public int DirectoriesCrawled { get; internal set; }
        public int DirectoriesSkipped { get; internal set; }
        public int DirectoriesErrored { get; internal set; }
        public int FilesCrawled { get; internal set; }
        public int FilesSkipped { get; internal set; }
        public int FilesErrored { get; internal set; }
        public long SizeOfFilesCrawled { get; internal set; }
    }

}
