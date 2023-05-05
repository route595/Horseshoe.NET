namespace Horseshoe.NET.IO.FileTraversal
{
    /// <summary>
    /// An object included in traversal events that allow client to affect the behavior of the 
    /// traversal engine on specific files. 
    /// </summary>
    public class FileTraversalMetadata
    {
        /// <summary>
        /// The size of the file being traversed.
        /// </summary>
        public long FileSize { get; set; }

        /// <summary>
        /// The action the traversal engine is to perform on the current file.  Default is <c>Browse</c>, ignored in 'recursive delete' mode.
        /// </summary>
        public ClientAction Action { get; set; }

        /// <summary>
        /// Called by the engine to prepare this metadata instance for the next event.
        /// Better than creating a new instance for each event.
        /// </summary>
        public void Reset(long newFileSize = 0L)
        {
            FileSize = newFileSize;
            Action = ClientAction.Browse;
        }
    }
}
