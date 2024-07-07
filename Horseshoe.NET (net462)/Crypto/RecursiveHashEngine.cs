using System;
using System.Text;

using Horseshoe.NET.IO;
using Horseshoe.NET.IO.FileTraversal;

namespace Horseshoe.NET.Crypto
{
    public class RecursiveHashEngine : TraversalEngine
    {
        private StringBuilder _hashes = new StringBuilder();

        public override AdvancedAction AdvancedAction =>
            new AdvancedAction
            (
                "Hash",
                fileHelloAction: (fp) =>
                {
                    var hash = Hash.String(fp, options: HashOptions);
                    OnFileHashed?.Invoke(fp, hash);
                    _hashes.Append(hash);
                }
            );

        /// <summary>
        /// Hash options
        /// </summary>
        public HashOptions HashOptions { get; }

        /// <summary>
        /// Event to trigger each time a file hello occurs and is not skipped
        /// </summary>
        public Action<FilePath, string> OnFileHashed { get; set; }

        /// <summary>
        /// Create a new <c>RecursiveHashEngine</c>
        /// </summary>
        /// <param name="root">A directory whose files to hash</param>
        /// <param name="optimizations"></param>
        /// <param name="hashOptions"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public RecursiveHashEngine
        (
            DirectoryPath root,
            TraversalOptimizations optimizations = null,
            HashOptions hashOptions = null
            //Action<DirectoryPath, TraversalDirectoryCommands> onDirectoryHello = null,
            //Action<DirectoryPath, bool> onDirectoryGoodbye = null,
            //Action<DirectoryPath> onDirectorySkipping = null,
            //Action<DirectoryPath> onDirectorySkipped = null,
            //Action<FilePath, TraversalFileCommands> onFileHello = null,
            //Action<FilePath> onFileSkipped = null,
            //Action<FilePath, string> onFileHashed = null
        ) : base(root, optimizations: optimizations)
        {
            HashOptions = hashOptions;
            //OnDirectoryHello = onDirectoryHello;
            //OnDirectoryGoodbye = onDirectoryGoodbye;
            //OnDirectorySkipping = onDirectorySkipping;
            //OnDirectorySkipped = onDirectorySkipped;
            //OnFileHello = onFileHello;
            //OnFileSkipped = onFileSkipped;
            //OnFileHashed = onFileHashed;
        }

        /// <summary>
        /// Calculates the cumulutive hash of all the file hashes
        /// </summary>
        /// <returns>The cumulative hash or, if no files were hashed, the empty string</returns>
        public string GetCumulativeHash() => _hashes.Length > 0
            ? Hash.String(_hashes.ToString(), options: HashOptions)
            : string.Empty;
    }
}
