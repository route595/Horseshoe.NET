namespace Horseshoe.NET.IO.DirectoryCrawler
{
    /// <summary>
    /// The file related event types triggered by the directory crawler 
    /// </summary>
    public enum FileCrawlEvent
    {
        /// <summary>
        /// Event type referring to encountering a file with intent to process it although this may result in a skip or deletion.
        /// </summary>
        FileProcessing,

        /// <summary>
        /// Event type referring to the processing of a file, denoting it was neither skipped nor deleted.
        /// </summary>
        FileProcessed,

        /// <summary>
        /// Event type referring to a file about to be deleted.
        /// </summary>
        FileDeleting,

        /// <summary>
        /// Event type referring to the deletion of a file.
        /// </summary>
        FileDeleted,

        /// <summary>
        /// Event type referring to skipping a file.  More specifically, it refers to files found by the traversal engine (i.e. not skipped or deleted) but then client code subsequently skips.
        /// </summary>
        FileSkipped,

        /// <summary>
        /// Event type referring to encountering an error while processing file.
        /// </summary>
        FileErrored
    }
}
