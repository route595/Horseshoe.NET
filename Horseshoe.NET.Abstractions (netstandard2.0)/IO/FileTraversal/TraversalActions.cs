namespace Horseshoe.NET.IO.FileTraversal
{
    /// <summary>
    /// A set of benign excptions and triggers useful in temporarily changing file traversal in useful ways.
    /// </summary>
    public static class TraversalActions
    {
        /// <summary>
        /// Client can call this during <c>DirectoryEntered</c> to skip to the next directory or <c>DirectoryDeleting</c> to leave the directory intact.
        /// </summary>
        /// <exception cref="SkipDirectoryException"></exception>
        public static void SkipDirectory() => throw new SkipDirectoryException();

        /// <summary>
        /// Client can call this during <c>FileFound</c> or <c>FileDeleting</c> to skip to the next file.
        /// </summary>
        /// <exception cref="SkipFileException"></exception>
        public static void SkipFile() => throw new SkipFileException();

        /// <summary>
        /// Client can call this during <c>DirectoryEntered</c> to delete the current directory.
        /// </summary>
        /// <exception cref="DeleteDirectoryException"></exception>
        public static void DeleteDirectory() => throw new DeleteDirectoryException();

        /// <summary>
        /// Client can call this during <c>FileFound</c> to delete the current file.
        /// </summary>
        /// <exception cref="DeleteFileException"></exception>
        public static void DeleteFile() => throw new DeleteFileException();

        /// <summary>
        /// A benign exception to signal the engine to skip the current file.
        /// </summary>
        public class SkipFileException : BenignException
        {
        }

        /// <summary>
        /// A benign exception to signal the engine to skip the current directory.
        /// </summary>
        public class SkipDirectoryException : BenignException
        {
        }

        /// <summary>
        /// A benign exception to signal the engine to delete the current file.
        /// </summary>
        public class DeleteFileException : BenignException
        {
        }

        /// <summary>
        /// A benign exception to signal the engine to delete the current directory.
        /// </summary>
        public class DeleteDirectoryException : BenignException
        {
        }
    }
}
