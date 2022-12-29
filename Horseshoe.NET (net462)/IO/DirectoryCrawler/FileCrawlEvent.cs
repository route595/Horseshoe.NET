namespace Horseshoe.NET.IO.DirectoryCrawler
{
    /// <summary>
    /// The file related event types triggered by the directory crawler 
    /// </summary>
    public enum FileCrawlEvent
    {
        /// <summary>
        /// 
        /// </summary>
        FileFound,

        /// <summary>
        /// 
        /// </summary>
        FileSkipped,

        /// <summary>
        /// 
        /// </summary>
        FileErrored,

        /// <summary>
        /// 
        /// </summary>
        OnHalt
    }
}
