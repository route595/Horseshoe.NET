using System;
using System.IO;

namespace Horseshoe.NET.IO.FileTraversal
{
    /// <summary>
    /// File and directory filters for when not every file and directory must be traversed.
    /// </summary>
    public class TraversalOptimization
    {
        /// <summary>
        /// The optional, native search pattern to specify which files are browsed by the engine, e.g. "*.txt".
        /// </summary>
        public string FileSearchPattern { get; set; }

        /// <summary>
        /// A programmatic filter to specify which files are browsed by the engine, e.g. (file) => file.Length > 1000000L.
        /// </summary>
        public Func<FileInfo, bool> FileFilter { get; set; }

        /// <summary>
        /// A programmatic filter to specify which directories are browsed by the engine, e.g. (dir) => dir.Name.In("bin", "obj").
        /// </summary>
        public Func<DirectoryInfo, bool> DirectoryFilter { get; set; }

        /// <summary>
        /// If <c>true</c> only directories are scanned, default is <c>false</c>.
        /// </summary>
        public bool DirectoryOnlyMode { get; set; }
    }
}
