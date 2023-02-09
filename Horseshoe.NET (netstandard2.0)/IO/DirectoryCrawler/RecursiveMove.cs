using System;
using System.IO;

namespace Horseshoe.NET.IO.DirectoryCrawler
{
    /// <summary>
    /// This <c>DirectoryCrawler</c> traverses a directory and moves it and all the files and subfolders inside to 
    /// the destination path.
    /// </summary>
    public class RecursiveMove : DirectoryCrawler
    {
        /// <summary>
        /// The destination counterpart of <c>Root</c>, the directory whose contents and self are to be moved
        /// </summary>
        public DirectoryPath DestinationRoot { get; }

        /// <summary>
        /// Creates a new <c>RecursiveMove</c>
        /// </summary>
        /// <param name="sourceRoot">The directory to move, a.k.a. <c>Root</c></param>
        /// <param name="destinationRoot">The path to which the the directory will be moved</param>
        /// <param name="directoryCrawled">
        /// A an optional, pluggable action that leverages the directory traversal engine to easily and declaratively execute directory oriented tasks
        /// <example>
        /// <para>
        /// Here's an example that prints out all the subdirectories that were moved by the directory traversal engine.
        /// <code>
        /// using Horseshoe.NET.ConsoleX;
        /// using Horseshoe.NET.IO.DirectoryCrawler;
        /// 
        /// var subdirectories = new List&lt;string&gt;();
        /// new RecursiveMove
        /// (
        ///     @"C:\myFilesAndFolders",
        ///     @"D:\myFilesAndFolders",
        ///     directoryCrawled: (@event, dir, metadata) =&gt;
        ///     {
        ///         switch(@event) 
        ///         {
        ///             case DirectoryCrawlEvent.DirectoryEntered:
        ///                 subdirectories.Add(dir);
        ///                 break;
        ///         }
        ///     }
        /// ).Go();
        /// subdirectories.Sort();
        /// RenderX.List(subdirectories);
        /// </code>
        /// </para>
        /// </example>
        /// </param>
        /// <param name="fileCrawled">
        /// A an optional, pluggable action that leverages the directory traversal engine to easily and declaratively execute file oriented tasks
        /// <example>
        /// <para>
        /// Here's an example that lists all files greater than 1 MB that were moved by the directory traversal engine.
        /// Note: In practice is better to use <c>movingFile</c> for this task.  See documentation for <c>movingFile</c>.
        /// <code>
        /// using Horseshoe.NET.ConsoleX;
        /// using Horseshoe.NET.IO;
        /// using Horseshoe.NET.IO.DirectoryCrawler;
        /// 
        /// var largeFiles = new List&lt;string&gt;();
        /// new RecursiveMove
        /// (
        ///     @"C:\myFilesAndFolders",
        ///     @"D:\myFilesAndFolders",
        ///     fileCrawled: (@event, file, metadata) =&gt;
        ///     {
        ///         switch(@event) 
        ///         {
        ///             case FileCrawlEvent.FileEncountered:
        ///                 if (file.Size >= 1024000)
        ///                 {
        ///                     largeFiles.Add(file + " (" + FileUtil.GetDisplayFileSize(file.Size) + ")");
        ///                 }
        ///                 break;
        ///         }
        ///     },
        /// ).Go();
        /// largeFiles.Sort();
        /// RenderX.List(largeFiles);
        /// </code>
        /// </para>
        /// </example>
        /// </param>
        /// <param name="destinationDirectoryCreating">Optional action to perform when a destination directory is about to be created</param>
        /// <param name="destinationDirectoryCreated">Optional action to perform after a destination directory is created </param>
        /// <param name="options">Optional traversal engine options.</param>
        /// <param name="statistics">Optional traversal engine statistics.</param>
        public RecursiveMove
        (
            DirectoryPath sourceRoot,
            DirectoryPath destinationRoot,
            Action<DirectoryCrawlEvent, DirectoryPath, DirectoryMetadata<DirectoryPath>> directoryCrawled = null,
            Action<FileCrawlEvent, FilePath, FileMetadata<DirectoryPath>> fileCrawled = null,
            Action<DirectoryPath, DirectoryMetadata<DirectoryPath>> destinationDirectoryCreating = null,
            Action<DirectoryPath, DirectoryMetadata<DirectoryPath>> destinationDirectoryCreated = null,
            CrawlOptions options = null,
            TraversalStatistics statistics = null
        ) : base
        (
            sourceRoot,
            options: options,
            statistics: statistics
        )
        {
            DestinationRoot = destinationRoot;
            DirectoryCrawled = (@event, dir, metadata) =>
            {
                directoryCrawled?.Invoke(@event, dir, metadata);  /* let client handle events first, if applicable */

                DirectoryPath destinationDirectory = Path.Combine(DestinationRoot.FullName, dir.FullName.Substring(Root.Length));

                switch (@event)
                {
                    case DirectoryCrawlEvent.OnInit:
                        if (destinationDirectory.Exists)  // <- refers to the destination root
                        {
                            if (!destinationDirectory.IsEmpty())
                            {
                                throw new DirectoryCrawlException("Directory already exists and is not empty: " + destinationDirectory);
                            }
                        }
                        else
                        {
                            destinationDirectoryCreating?.Invoke(destinationDirectory, metadata);
                            if (!options.DryRun)
                            {
                                destinationDirectory.Create();  // <- refers to the destination root
                            }
                            destinationDirectoryCreated?.Invoke(destinationDirectory, metadata);
                        }
                        break;

                    case DirectoryCrawlEvent.DirectoryEntered:
                        destinationDirectoryCreating?.Invoke(destinationDirectory, metadata);
                        if (!metadata.DryRun)
                        {
                            destinationDirectory.Create();
                        }
                        destinationDirectoryCreated?.Invoke(destinationDirectory, metadata);
                        break;

                    case DirectoryCrawlEvent.DirectoryExited:
                        DoDirectoryDelete(dir, metadata);
                        break;
                }
            };
            FileCrawled = (@event, file, metadata) =>
            {
                fileCrawled?.Invoke(@event, file, metadata);  /* let client handle events first, if applicable */

                FilePath destinationFile = Path.Combine(DestinationRoot.FullName, file.FullName.Substring(Root.Length));

                switch (@event)
                {
                    case FileCrawlEvent.FileProcessing:
                        FileProcess =  /* Note: honors "dry run" mode */
                            (file0, metadata0) =>
                            {
                                file0.MoveTo(destinationFile);  // uses built-in tallying
                            };
                        break;
                }
            };
        }
    }
}

