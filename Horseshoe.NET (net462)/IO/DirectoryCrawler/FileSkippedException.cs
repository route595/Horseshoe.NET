namespace Horseshoe.NET.IO.DirectoryCrawler
{
    public class FileSkippedException : BenignException
    {
        public SkipReason SkipReason { get; }

        public FileSkippedException(SkipReason skipReason) : base("File skipped...")
        {
            SkipReason = skipReason;
        }
    }
}
