using System;
using System.IO;
using System.Linq;

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
        /// <param name="movingFile">
        /// A an optional, pluggable action to perform when a file is about to be moved.
        /// <example>
        /// <para>
        /// Here's an example that lists all files greater than 1 MB that were moved by the directory traversal engine.
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
        ///     movingFile: (srcFile, destFile, metadata) =&gt;
        ///     {
        ///         if (file.Size >= 1024000)
        ///         {
        ///             largeFiles.Add(file + " (" + FileUtil.GetDisplayFileSize(file.Size) + ")");
        ///         }
        ///     },
        /// ).Go();
        /// largeFiles.Sort();
        /// RenderX.List(largeFiles);
        /// </code>
        /// </para>
        /// </example>
        /// </param>
        /// <param name="fileMoved">Optional action to perform after a file is processed</param>
        /// <param name="creatingDestinationDirectory">Optional action to perform when a destination directory is about to be created</param>
        /// <param name="destinationDirectoryCreated">Optional action to perform after a destination directory is created </param>
        /// <param name="creatingDestinationRootDirectory">Optional action to perform when <c>destinationRoot</c> is about to be created</param>
        /// <param name="destinationRootDirectoryCreated">Optional action to perform after <c>destinationRoot</c> is created</param>
        /// <param name="deletingSourceDirectory">Optional action to perform when a source directory is about to be deleted</param>
        /// <param name="sourceDirectoryDeleted">Optional action to perform after a source directory is deleted</param>
        /// <param name="deletingSourceRootDirectory">Optional action to perform when <c>sourceRoot</c> is about to be deleted</param>
        /// <param name="sourceRootDirectoryDeleted">Optional action to perform after <c>sourceRoot</c> is deleted</param>
        /// <param name="options">Crawl options</param>
        public RecursiveMove
        (
            DirectoryPath sourceRoot,
            DirectoryPath destinationRoot,
            Action<DirectoryCrawlEvent, DirectoryPath, DirectoryMetadata<DirectoryPath>> directoryCrawled = null,
            Action<FileCrawlEvent, FilePath, FileMetadata<DirectoryPath>> fileCrawled = null,
            Action<FilePath, FilePath, FileMetadata<DirectoryPath>> movingFile = null,
            Action<FilePath, FilePath, FileMetadata<DirectoryPath>> fileMoved = null,
            Action<DirectoryPath, DirectoryPath, DirectoryMetadata<DirectoryPath>> creatingDestinationDirectory = null,
            Action<DirectoryPath, DirectoryPath, DirectoryMetadata<DirectoryPath>> destinationDirectoryCreated = null,
            Action<DirectoryPath, DirectoryMetadata<DirectoryPath>> creatingDestinationRootDirectory = null,
            Action<DirectoryPath, DirectoryMetadata<DirectoryPath>> destinationRootDirectoryCreated = null,
            Action<DirectoryPath, DirectoryMetadata<DirectoryPath>> deletingSourceDirectory = null,
            Action<DirectoryPath, DirectoryMetadata<DirectoryPath>> sourceDirectoryDeleted = null,
            Action<DirectoryPath, DirectoryMetadata<DirectoryPath>> deletingSourceRootDirectory = null,
            Action<DirectoryPath, DirectoryMetadata<DirectoryPath>> sourceRootDirectoryDeleted = null,
            CrawlOptions options = null
        ) : base
        (
            sourceRoot,
            directoryCrawled: (@event, dir, metadata) =>
            {
                directoryCrawled?.Invoke(@event, dir, metadata);  /* let client handle events first, if applicable */

                var fileMove = (RecursiveMove)metadata.DirectoryCrawler;
                options = options ?? new CopyOptions();
                DirectoryPath destinationDirectory = Path.Combine(fileMove.DestinationRoot.FullName, dir.FullName.Substring(fileMove.RootLength));

                switch (@event)
                {
                    case DirectoryCrawlEvent.OnInit:
                        if (destinationDirectory.Exists)  // <- refers to the destination root
                        {
                            if (!destinationDirectory.IsEmpty)
                            {
                                throw new DirectoryCrawlException("Directory already exists and is not empty: " + destinationDirectory);
                            }
                        }
                        else
                        {
                            creatingDestinationRootDirectory?.Invoke(destinationDirectory, metadata);
                            if (!options.DryRun)
                            {
                                destinationDirectory.Create();  // <- refers to the destination root
                            }
                            destinationRootDirectoryCreated?.Invoke(destinationDirectory, metadata);
                        }
                        break;

                    case DirectoryCrawlEvent.DirectoryEntered:
                        creatingDestinationDirectory?.Invoke(dir, destinationDirectory, metadata);
                        if (!metadata.DryRun)
                        {
                            destinationDirectory.Create();
                        }
                        destinationDirectoryCreated?.Invoke(dir, destinationDirectory, metadata);
                        break;

                    case DirectoryCrawlEvent.DirectoryExited:
                        deletingSourceDirectory?.Invoke(dir, metadata);
                        if (!metadata.DryRun)
                        {
                            dir.Delete();
                        }
                        sourceDirectoryDeleted?.Invoke(dir, metadata);
                        break;

                    case DirectoryCrawlEvent.OnComplete:
                        deletingSourceRootDirectory?.Invoke(dir, metadata);
                        if (!metadata.DryRun)
                        {
                            dir.Delete();  // <- refers to the source root
                        }
                        sourceRootDirectoryDeleted?.Invoke(dir, metadata);
                        break;
                }
            },
            fileCrawled: (@event, file, metadata) =>
            {
                fileCrawled?.Invoke(@event, file, metadata);  /* let client handle events first, if applicable */

                var fileMove = (RecursiveMove)metadata.DirectoryCrawler;
                options = options ?? new CopyOptions();
                FilePath destinationFile = Path.Combine(fileMove.DestinationRoot.FullName, file.FullName.Substring(fileMove.RootLength));

                switch (@event)
                {
                    case FileCrawlEvent.FileFound:
                        movingFile?.Invoke(file, destinationFile, metadata);
                        if (!metadata.DryRun)
                        {
                            file.MoveTo(destinationFile.FullName);
                        }
                        fileMoved?.Invoke(file, destinationFile, metadata);
                        break;
                }
            },
            options: options
        )
        {
            DestinationRoot = destinationRoot;
        }
    }
}

