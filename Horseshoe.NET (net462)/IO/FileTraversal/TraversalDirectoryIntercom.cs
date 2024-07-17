namespace Horseshoe.NET.IO.FileTraversal
{
    /// <summary>
    /// Client requested directory traversal commands.
    /// </summary>
    public class TraversalDirectoryIntercom
    {
        internal bool Skipped { get; private set; }
        internal bool DeleteRequested { get; private set; }
        internal bool DeleteContents { get; private set; }
        internal bool DryRun { get; private set; }

        /// <summary>
        /// Client may request this action during certain traversal actions.
        /// </summary>
        public void Skip() => Skipped = true;

        /// <summary>
        /// Client may request this action during certain traversal actions.
        /// </summary>
        public void RequestDelete(bool deleteContents = false, bool dryRun = false)
        {
            DeleteRequested = true;
            DeleteContents = deleteContents;
            DryRun = dryRun;
        }

        /// <summary>
        /// Resets the instance's properties to default values.
        /// </summary>
        /// <returns>The instance.</returns>
        public TraversalDirectoryIntercom Reset()
        {
            Skipped = false;
            DeleteRequested = false;
            DeleteContents = false;
            DryRun = false;
            return this;
        }

        /// <summary>
        /// Use this instance for memory optimization
        /// </summary>
        public static TraversalDirectoryIntercom Instance { get; } = new TraversalDirectoryIntercom();
    }
}
