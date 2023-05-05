namespace Horseshoe.NET.IO.FileTraversal
{
    /// <summary>
    /// A set of basic actions that can be assigned to directory iterations.
    /// </summary>
    public enum RecursiveAction
    {
        /// <summary>
        /// The default action
        /// </summary>
        Browse,

        /// <summary>
        /// Deletes files and directories
        /// </summary>
        Delete
    }
}
