using System;

namespace Horseshoe.NET.IO.FileTraversal
{
    /// <summary>
    /// File and directory preferences and filters
    /// </summary>
    public class TraversalOptimizations
    {
        /// <summary>
        /// An optional native search pattern to specify which files are browsed by the traversal engine, e.g. "*.txt".
        /// </summary>
        public string FileSearchPattern { get; set; }

        /// <summary>
        /// An optional programmatic filter to specify which files are browsed by the traversal engine, e.g. (file) => file.Length > 1000000L.
        /// </summary>
        public Func<FilePath, bool> FileFilter { get; set; }

        /// <summary>
        /// An optional native search pattern to specify which directories are browsed by the traversal engine, e.g. "Program Files*".
        /// </summary>
        public string DirectorySearchPattern { get; set; }

        /// <summary>
        /// An optional programmatic filter to specify which directories are browsed by the traversal engine, e.g. (dir) => dir.Name.In("bin", "obj").
        /// </summary>
        public Func<DirectoryPath, bool> DirectoryFilter { get; set; }

        /// <summary>
        /// If <c>true</c> only directories are scanned and not files, default is <c>false</c>.
        /// </summary>
        public bool DirectoriesOnlyMode { get; set; }
    }
}
