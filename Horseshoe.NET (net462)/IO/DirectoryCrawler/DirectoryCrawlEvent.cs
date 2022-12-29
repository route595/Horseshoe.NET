namespace Horseshoe.NET.IO.DirectoryCrawler
{
    /// <summary>
    /// The set of event types triggered by the directory traversal engine
    /// </summary>
    public enum DirectoryCrawlEvent
    {
        /// <summary>
        /// Event type referring to startup of the directory traveral engine
        /// </summary>
        OnInit,

        /// <summary>
        /// Event type referring to finding a directory (may be skipped even after this event has fired)
        /// </summary>
        DirectoryEntered,

        /// <summary>
        /// Event type referring to completing traversal of a directory
        /// </summary>
        DirectoryExited,

        /// <summary>
        /// Event type referring to skipping a directory (filtered or programmatically skipped)
        /// </summary>
        DirectorySkipped,

        /// <summary>
        /// Event type referring to encountering an exception
        /// </summary>
        DirectoryErrored,

        /// <summary>
        /// Event type referring to halting the directory traveral engine (either due to certain conditions or programmatically)
        /// </summary>
        OnHalt,

        /// <summary>
        /// Event type referring to completion of the directory traveral engine (will always trigger even if an <c>OnHalt</c> event was also triggered)
        /// </summary>
        OnComplete
    }
}
