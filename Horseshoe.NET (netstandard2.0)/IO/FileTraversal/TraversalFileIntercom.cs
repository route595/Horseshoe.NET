namespace Horseshoe.NET.IO.FileTraversal
{
    /// <summary>
    /// Client requested file traversal commands.
    /// </summary>
    public class TraversalFileIntercom
    {
        internal bool SkipRequested { get; private set; }
        internal bool DeleteRequested { get; private set; }
        internal bool DryRun { get; private set; }

        /// <summary>
        /// Client can call this during certain traversal actions.
        /// </summary>
        public void Skip() => SkipRequested = true;

        /// <summary>
        /// Client can call this during certain traversal actions.
        /// </summary>
        public void Delete() => DeleteRequested = true;

        /// <summary>
        /// Resets the instance's properties to default values.
        /// </summary>
        /// <returns>The instance.</returns>
        public TraversalFileIntercom Reset(bool dryRun = false)
        {
            SkipRequested = false;
            DeleteRequested = false;
            DryRun = dryRun;
            return this;
        }

        /// <summary>
        /// Use this instance for memory optimization
        /// </summary>
        public static TraversalFileIntercom Instance { get; } = new TraversalFileIntercom();
    }
}
