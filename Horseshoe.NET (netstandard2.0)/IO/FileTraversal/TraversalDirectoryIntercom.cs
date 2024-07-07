namespace Horseshoe.NET.IO.FileTraversal
{
    /// <summary>
    /// Client requested directory traversal commands.
    /// </summary>
    public class TraversalDirectoryIntercom
    {
        internal bool SkipRequested { get; private set; }
        internal bool DeleteRequested { get; private set; }
        internal bool DeleteContentsRequested { get; private set; }

        /// <summary>
        /// Client may request this action during certain traversal actions.
        /// </summary>
        public void Skip() => SkipRequested = true;

        /// <summary>
        /// Client may request this action during certain traversal actions.
        /// </summary>
        public void Delete() => DeleteRequested = true;

        /// <summary>
        /// Client may request this action during certain traversal actions.
        /// </summary>
        public void DeleteContents() => DeleteContentsRequested = true;

        /// <summary>
        /// Resets the instance's properties to default values.
        /// </summary>
        /// <returns>The instance.</returns>
        public TraversalDirectoryIntercom Reset()
        {
            SkipRequested = false;
            DeleteRequested = false;
            DeleteContentsRequested = false;
            return this;
        }

        /// <summary>
        /// Use this instance for memory optimization
        /// </summary>
        public static TraversalDirectoryIntercom Instance { get; } = new TraversalDirectoryIntercom();
    }
}
