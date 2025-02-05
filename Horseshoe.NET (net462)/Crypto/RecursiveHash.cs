using System;

using Horseshoe.NET.IO;
using Horseshoe.NET.IO.FileTraversal;

namespace Horseshoe.NET.Crypto
{
    /// <summary>
    /// Hashes all the files in a directory and returns a cumulative hash
    /// </summary>
    public static class RecursiveHash
    {
        /// <summary>
        /// Generates hash from the filesystem files in the supplied directory
        /// </summary>
        /// <param name="directory">Directory containing the files to hash</param>
        /// <param name="statistics">A snapshot of what occurred during the file traversal session</param>
        /// <param name="optimizations">File and directory preferences and filters</param>
        /// <param name="options">Hash options</param>
        /// <param name="onDirectoryHello">Hash options</param>
        /// <param name="onDirectoryGoodbye">Hash options</param>
        /// <param name="onDirectorySkipped">Hash options</param>
        /// <param name="onFileHello">Hash options</param>
        /// <param name="onFileSkipped">Hash options</param>
        /// <param name="onFileHashed">Hash options</param>
        /// <returns>hash</returns>
        public static string String
        (
            DirectoryPath directory, 
            out TraversalStatistics statistics, 
            TraversalOptimizations optimizations = null,
            HashOptions options = null,
            Action<DirectoryPath, TraversalEngine, TraversalDirectoryIntercom> onDirectoryHello = null,
            Action<DirectoryPath, TraversalEngine, TraversalDirectoryIntercom> onDirectoryGoodbye = null,
            Action<DirectoryPath, TraversalEngine> onDirectorySkipped = null,
            Action<FilePath, TraversalEngine, TraversalFileIntercom> onFileHello = null,
            Action<FilePath, TraversalEngine> onFileSkipped = null,
            Action<FilePath, string> onFileHashed = null
        )
        {
            var engine = new RecursiveHashEngine(directory, optimizations: optimizations, hashOptions: options)
            {
                OnDirectoryHello = onDirectoryHello,
                OnDirectoryGoodbye = onDirectoryGoodbye,
                OnDirectorySkipped = onDirectorySkipped,
                OnFileHello = onFileHello,
                OnFileSkip = onFileSkipped,
                OnFileHashed = onFileHashed
            };
            statistics = engine.Statistics;
            engine.Start();
            return engine.GetCumulativeHash();
        }
    }
}
