namespace Horseshoe.NET.IO.FileTraversal
{
    /// <summary>
    /// Client requested file traversal commands.
    /// </summary>
    public class TraversalFileIntercom
    {

        /// <summary>
        /// A file action label for the traversal engine to display in the statistics 
        /// </summary>
        public string ActionName { get; set; }

        internal bool Skipped { get; private set; }
        internal bool DeleteRequested { get; private set; }
        internal bool DryRun { get; private set; }

        /// <summary>
        /// Client can call this during certain traversal actions.
        /// </summary>
        public void Skip() => Skipped = true;

        /// <summary>
        /// Client can call this during certain traversal actions.
        /// </summary>
        public void RequestDelete(bool dryRun = false) 
        {
            DeleteRequested = true;
            if (dryRun)
                DryRun = true;
        }

        /// <summary>
        /// Resets the instance's properties to default values.
        /// </summary>
        /// <returns>The instance.</returns>
        public TraversalFileIntercom Reset(bool dryRun = false)
        {
            ActionName = null;
            Skipped = false;
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
