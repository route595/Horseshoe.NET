namespace Horseshoe.NET.IO.FileTraversal
{
    /// <summary>
    /// A simple structure for storing error data
    /// </summary>
    public class TraversalError
    {
        /// <summary>
        /// For example: "deletion failed", exception type or exception message.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// The file or directory path where the error occurred.
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Presents error data in a message + path format.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Message + ": " + Path;
        }
    }
}
