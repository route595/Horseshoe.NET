using System;
using System.IO;

namespace Horseshoe.NET.IO.DirectoryCrawler
{
    /// <summary>
    /// <para>
    /// This <c>DirectoryCrawler</c> traverses and deletes a directory and all the files and subfolders inside.
    /// </para>
    /// <para>
    /// It is possible to delete only certain files and folders.  See <see cref="CrawlOptions"/>.
    /// </para>
    /// </summary>
    public class RecursiveDelete : DirectoryCrawler
    {
        /// <summary>
        /// Creates a new <c>RecursiveDelete</c>
        /// </summary>
        /// <param name="root">The directory whose contents and self are to be deleted</param>
        /// <param name="directoryCrawled">
        /// A an optional, pluggable action that leverages the directory traversal engine to easily and declaratively execute directory oriented tasks.
        /// <example>
        /// <para>
        /// Here's an example that prints out all the subdirectories that were deleted by the directory traversal engine.
        /// Note: In practice is better to use <c>deletingDirectory</c> for this task.  See documentation for <c>deletingDirectory</c>.
        /// <code>
        /// using Horseshoe.NET.ConsoleX;
        /// using Horseshoe.NET.IO.DirectoryCrawler;
        /// 
        /// var subdirectories = new List&lt;string&gt;();
        /// new RecursiveDelete
        /// (
        ///     @"C:\myFilesAndFolders",
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
        /// A an optional, pluggable action that leverages the directory traversal engine to easily and declaratively execute file oriented tasks.
        /// <example>
        /// <para>
        /// Here's an example that lists all files greater than 1 MB that were deleted by the directory traversal engine.
        /// Note: In practice is better to use <c>deletingFile</c> for this task.  See documentation for <c>deletingFile</c>.
        /// <code>
        /// using Horseshoe.NET.ConsoleX;
        /// using Horseshoe.NET.IO;
        /// using Horseshoe.NET.IO.DirectoryCrawler;
        /// 
        /// var largeFiles = new List&lt;string&gt;();
        /// new RecursiveDelete
        /// (
        ///     @"C:\myFilesAndFolders",
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
        /// <param name="deletingFile">
        /// An optional, pluggable action to perform when a file is about to be deleted.
        /// <example>
        /// <para>
        /// Here's an example that lists all files greater than 1 MB that were deleted by the directory traversal engine.
        /// <code>
        /// using Horseshoe.NET.ConsoleX;
        /// using Horseshoe.NET.IO;
        /// using Horseshoe.NET.IO.DirectoryCrawler;
        /// 
        /// var largeFiles = new List&lt;string&gt;();
        /// new RecursiveDelete
        /// (
        ///     @"C:\myFilesAndFolders",
        ///     deletingFile: (file, metadata) =&gt;
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
        /// <param name="fileDeleted">Optional action to perform after a file is deleted</param>
        /// <param name="deletingDirectory">
        /// An optional, pluggable action to perform when a directory is about to be deleted
        /// <example>
        /// <para>
        /// Here's an example that prints out all the subdirectories that were deleted by directory traversal engine.
        /// <code>
        /// using Horseshoe.NET.ConsoleX;
        /// using Horseshoe.NET.IO.DirectoryCrawler;
        /// 
        /// var subdirectories = new List&lt;string&gt;();
        /// new RecursiveDelete
        /// (
        ///     @"C:\myFilesAndFolders",
        ///     deletingDirectory: (dir, metadata) =&gt;
        ///     {
        ///         subdirectories.Add(dir);
        ///     }
        /// ).Go();
        /// subdirectories.Sort();
        /// RenderX.List(subdirectories);
        /// </code>
        /// </para>
        /// </example>
        /// </param>
        /// <param name="directoryDeleted">Optional action to perform after a directory is deleted</param>
        /// <param name="precludeRootDirectory">Causes the root directory to not be deleted</param>
        /// <param name="options">Crawler options</param>
        public RecursiveDelete
        (
            DirectoryPath root,
            Action<DirectoryCrawlEvent, DirectoryPath, DirectoryMetadata<DirectoryPath>> directoryCrawled = null,
            Action<FileCrawlEvent, FilePath, FileMetadata<DirectoryPath>> fileCrawled = null,
            Action<FilePath, FileMetadata<DirectoryPath>> deletingFile = null,
            Action<FilePath, FileMetadata<DirectoryPath>> fileDeleted = null,
            Action<DirectoryPath, DirectoryMetadata<DirectoryPath>> deletingDirectory = null,
            Action<DirectoryPath, DirectoryMetadata<DirectoryPath>> directoryDeleted = null,
            bool precludeRootDirectory = false,
            CrawlOptions options = null
        ) : base
        (
            root,
            directoryCrawled: (@event, dir, metadata) =>
            {
                directoryCrawled?.Invoke(@event, dir, metadata);    /* let client handle events first, if applicable */
                switch (@event)
                {
                    case DirectoryCrawlEvent.DirectoryExited:
                        deletingDirectory?.Invoke(dir, metadata);
                        if (!metadata.DryRun)
                        {
                            dir.Delete();
                        }
                        directoryDeleted?.Invoke(dir, metadata);
                        break;
                }
            },
            fileCrawled: (@event, file, metadata) =>
            {
                fileCrawled?.Invoke(@event, file, metadata);    /* let client handle events first, if applicable */
                switch (@event)
                {
                    case FileCrawlEvent.FileFound:
                        deletingFile?.Invoke(file, metadata);
                        if (!metadata.DryRun)
                        {
                            file.Delete();
                        }
                        fileDeleted?.Invoke(file, metadata);
                        break;
                }
            },
            options: precludeRootDirectory
                ? new CrawlOptions(options) { AutoSkipRootDirectoryExited = true }
                : options
        )
        { 
        }
    }
}
