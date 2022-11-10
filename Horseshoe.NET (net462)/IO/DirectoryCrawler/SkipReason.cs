namespace Horseshoe.NET.IO.DirectoryCrawler
{
    public enum SkipReason
    {
        NA,
        ClientSkipped,
        AlreadyExists,
        AlreadyExists_RestartedAfterIncompleteCopy,
        AlreadyExists_DateModified,
        UserFiltered
    }
}
