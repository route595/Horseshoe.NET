namespace Horseshoe.NET.IO.DirectoryCrawler
{
    public class DirectorySkippedException : BenignException
    {
        public SkipReason SkipReason { get; }

        public DirectorySkippedException(SkipReason skipReason) : base("Directory skipped...")
        {
            SkipReason = skipReason;
        }
    }
}
