namespace Horseshoe.NET.IO.FileTraversal
{
    /// <summary>
    /// An object included in traversal events that allow client to affect the behavior of the 
    /// traversal engine on specific directories. 
    /// </summary>
    public class DirectoryTraversalMetadata
    {
        /// <summary>
        /// Client-selectable action to perform on the current file or directory.
        /// </summary>
        public ClientAction Action { get; set; }

        /// <summary>
        /// If <c>true</c>, the current directory was skipped by the client (as opposed to filter automation by the traversal engine), usually <c>false</c>.
        /// </summary>
        public bool ClientSkipped { get; set; }

        /// <summary>
        /// If <c>true</c>, the entire current directory and all contents were skipped by the client.
        /// </summary>
        public bool RecursivelySkipped { get; set; }

        /// <summary>
        /// Called by the engine to prepare this metadata instance for the next event.
        /// Better than creating a new instance for each event.
        /// </summary>
        public void Reset()
        {
            Action = ClientAction.Browse;
            ClientSkipped = false;
            RecursivelySkipped = false;
        }
    }
}
