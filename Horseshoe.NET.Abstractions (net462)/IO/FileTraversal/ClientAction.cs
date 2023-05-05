namespace Horseshoe.NET.IO.FileTraversal
{
    /// <summary>
    /// A set of basic actions that client can assign to directory iterations.
    /// </summary>
    public enum ClientAction
    {
        /// <summary>
        /// The default action.
        /// </summary>
        Browse,

        /// <summary>
        /// Skips the files in the current directory.
        /// </summary>
        Skip,

        /// <summary>
        /// Skips to the next file or directory.
        /// </summary>
        RecursiveSkip,

        /// <summary>
        /// Recursively deletes the current directory except the recursive root directory.
        /// </summary>
        Empty,

        /// <summary>
        /// Deletes the current file or recursively deletes the current directory.
        /// </summary>
        Delete
    }
}
