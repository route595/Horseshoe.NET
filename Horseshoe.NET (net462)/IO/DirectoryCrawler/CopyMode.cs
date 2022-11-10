namespace Horseshoe.NET.IO.DirectoryCrawler
{
    public enum CopyMode
    {
        NA,
        Normal,
        NormalWithRestartAfterIncompleteCopy,
        Overwrite,
        OverwriteIfNewer,
    }
}
