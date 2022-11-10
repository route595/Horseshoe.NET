namespace Horseshoe.NET.IO.DirectoryCrawler
{
    public class DirectoryCrawlHaltedException : BenignException
    {
        public DirectoryCrawlHaltedException() : base("Directory crawl halted...") { }
    }
}
