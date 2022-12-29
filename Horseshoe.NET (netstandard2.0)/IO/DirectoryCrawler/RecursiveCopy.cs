using System;
using System.IO;
using System.Linq;

using Horseshoe.NET.Collections;
using Horseshoe.NET.Crypto;

namespace Horseshoe.NET.IO.DirectoryCrawler
{
    /// <summary>
    /// This <c>DirectoryCrawler</c> traverses a directory and copies it and all the files and subfolders inside to the destination path.
    /// </summary>
    public class RecursiveCopy : DirectoryCrawler
    {
        /// <summary>
        /// The destination counterpart of <c>Root</c>, the directory whose contents and self are to be copied
        /// </summary>
        public DirectoryPath DestinationRoot { get; }

        /// <summary>
        /// Create a new <c>RecursiveCopy</c>
        /// </summary>
        /// <param name="sourceRoot">The directory to copy, a.k.a. <c>Root</c></param>
        /// <param name="destinationRoot">The path to which the the directory will be copied</param>
        /// <param name="directoryCrawled">
        /// A an optional, pluggable action that leverages the directory traversal engine to easily and declaratively execute directory oriented tasks
        /// <example>
        /// <para>
        /// Here's an example that prints out all the subdirectories that were copied by the directory traversal engine.
        /// <code>
        /// using Horseshoe.NET.ConsoleX;
        /// using Horseshoe.NET.IO.DirectoryCrawler;
        /// 
        /// var subdirectories = new List&lt;string&gt;();
        /// new RecursiveCopy
        /// (
        ///     @"C:\myFilesAndFolders",
        ///     @"D:\myFilesAndFolders",
        ///     directoryCrawled: (@event, dir, metadata, statistics) =&gt;
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
        /// Note: In practice is better to use <c>copyingFile</c> for this task.  See documentation for <c>copyingFile</c>.
        /// <code>
        /// using Horseshoe.NET.ConsoleX;
        /// using Horseshoe.NET.IO;
        /// using Horseshoe.NET.IO.DirectoryCrawler;
        /// 
        /// var largeFiles = new List&lt;string&gt;();
        /// new RecursiveCopy
        /// (
        ///     @"C:\myFilesAndFolders",
        ///     @"D:\myFilesAndFolders",
        ///     fileCrawled: (@event, file, metadata, statistics) =&gt;
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
        /// <param name="copyingFile">
        /// A an optional, pluggable action to perform when a file is about to be copied.
        /// <example>
        /// <para>
        /// Here's an example that lists all files greater than 1 MB that were copied by the directory traversal engine.
        /// <code>
        /// using Horseshoe.NET.ConsoleX;
        /// using Horseshoe.NET.IO;
        /// using Horseshoe.NET.IO.DirectoryCrawler;
        /// 
        /// var largeFiles = new List&lt;string&gt;();
        /// new RecursiveCopy
        /// (
        ///     @"C:\myFilesAndFolders",
        ///     @"D:\myFilesAndFolders",
        ///     copyingFile: (srcFile, destFile, metadata) =&gt;
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
        /// <param name="fileCopied"></param>
        /// <param name="deletingFile"></param>
        /// <param name="fileDeleted"></param>
        /// <param name="creatingDestinationDirectory"></param>
        /// <param name="destinationDirectoryCreated"></param>
        /// <param name="creatingDestinationRootDirectory"></param>
        /// <param name="destinationRootDirectoryCreated"></param>
        /// <param name="deletingDirectory"></param>
        /// <param name="directoryDeleted"></param>
        /// <param name="options"></param>
        /// <exception cref="DirectoryCrawlException"></exception>
        public RecursiveCopy
        (
            DirectoryPath sourceRoot,
            DirectoryPath destinationRoot,
            Action<DirectoryCrawlEvent, DirectoryPath, DirectoryMetadata<DirectoryPath>> directoryCrawled = null,
            Action<FileCrawlEvent, FilePath, FileMetadata<DirectoryPath>> fileCrawled = null,
            Action<FilePath, FilePath, FileMetadata<DirectoryPath>> copyingFile = null,
            Action<FilePath, FilePath, FileMetadata<DirectoryPath>> fileCopied = null,
            Action<FilePath, FileMetadata<DirectoryPath>> deletingFile = null,
            Action<FilePath, FileMetadata<DirectoryPath>> fileDeleted = null,
            Action<DirectoryPath, DirectoryMetadata<DirectoryPath>> creatingDestinationDirectory = null,
            Action<DirectoryPath, DirectoryMetadata<DirectoryPath>> destinationDirectoryCreated = null,
            Action<DirectoryPath, DirectoryMetadata<DirectoryPath>> creatingDestinationRootDirectory = null,
            Action<DirectoryPath, DirectoryMetadata<DirectoryPath>> destinationRootDirectoryCreated = null,
            Action<DirectoryPath, DirectoryMetadata<DirectoryPath>> deletingDirectory = null,
            Action<DirectoryPath, DirectoryMetadata<DirectoryPath>> directoryDeleted = null,
            CopyOptions options = null
        ) : base
        (
            sourceRoot,
            directoryCrawled: (@event, dir, metadata) =>
            {
                var fileCopy = (RecursiveCopy)metadata.DirectoryCrawler;
                options = options ?? new CopyOptions();
                DirectoryPath destinationDirectory = Path.Combine(fileCopy.DestinationRoot.FullName, dir.FullName.Substring(fileCopy.RootLength));
                
                switch (@event)
                {
                    case DirectoryCrawlEvent.OnInit:
                        if (destinationDirectory.Exists)  // <- refers to the destination root
                        {
                            if (((CopyOptions)fileCopy.Options).CopyMode == CopyMode.ErrorIfDestinationNotEmpty && !destinationDirectory.IsEmpty)
                                throw new DirectoryCrawlException("Directory already exists and is not empty: " + destinationDirectory);
                            
                            if ((((CopyOptions)fileCopy.Options).CopyMode & CopyMode.DeleteDestinationDirectoryBeforeCopy) == CopyMode.DeleteDestinationDirectoryBeforeCopy)
                            { 
                                new RecursiveDelete
                                (
                                    destinationDirectory,
                                    deletingFile: deletingFile,
                                    fileDeleted: fileDeleted,
                                    deletingDirectory: deletingDirectory,
                                    directoryDeleted: directoryDeleted,
                                    precludeRootDirectory: true
                                ).Go();
                            }
                        }
                        else
                        {
                            creatingDestinationRootDirectory?.Invoke(dir, metadata);
                            if (!options.DryRun)
                            {
                                destinationDirectory.Create();   // <- refers to the destination root
                            }
                            destinationRootDirectoryCreated?.Invoke(dir, metadata);
                        }
                        break;

                    case DirectoryCrawlEvent.DirectoryEntered:
                        if (destinationDirectory.Exists)
                        {
                            if ((((CopyOptions)fileCopy.Options).CopyMode & CopyMode.RemoveDestinationFilesAndDirectoriesNotInSource) == CopyMode.RemoveDestinationFilesAndDirectoriesNotInSource)
                            {
                                var sourceFileNames = dir.GetFiles()
                                    .Select(f => f.Name)
                                    .ToArray();
                                var sourceSubdirNames = dir.GetDirectories()
                                    .Select(d => d.Name)
                                    .ToArray();
                                var destFiles = destinationDirectory.GetFiles();
                                var destSubdirs = destinationDirectory.GetDirectories();

                                // removing destination files that are not in the source
                                foreach(var destFile in destFiles)
                                {
                                    if (!destFile.Name.In(sourceFileNames))
                                    {
                                        var fileArgsDel = new FileMetadata<DirectoryPath>(metadata.Level + 1, metadata.DirectoryCrawler, dryRun: metadata.DryRun);
                                        deletingFile?.Invoke(destFile, fileArgsDel);
                                        if (!metadata.DryRun)
                                        {
                                            destFile.Delete();
                                        }
                                        fileDeleted?.Invoke(destFile, fileArgsDel);
                                    }
                                }

                                // removing destination directories that are not in the source
                                foreach (var destSubdir in destSubdirs)
                                {
                                    if (!destSubdir.Name.In(sourceSubdirNames))
                                    {
                                        new RecursiveDelete
                                        (
                                            destSubdir,
                                            deletingFile: deletingFile,
                                            fileDeleted: fileDeleted,
                                            deletingDirectory: deletingDirectory,
                                            directoryDeleted: directoryDeleted,
                                            options: new CrawlOptions { DryRun = metadata.DryRun }
                                        ).Go();
                                    }
                                }
                            }
                        }
                        else
                        {
                            creatingDestinationDirectory?.Invoke(destinationDirectory, metadata);
                            if (!metadata.DryRun)
                            {
                                destinationDirectory.Create();
                            }
                            destinationDirectoryCreated?.Invoke(destinationDirectory, metadata);
                        }
                        break;
                }
                directoryCrawled?.Invoke(@event, dir, metadata);  /* let client handle events too, if applicable */
            },
            fileCrawled: (@event, file, metadata) =>
            {
                var fileCopy = (RecursiveCopy)metadata.DirectoryCrawler;
                options = options ?? new CopyOptions();
                FilePath destinationFile = Path.Combine(fileCopy.DestinationRoot.FullName, file.FullName.Substring(fileCopy.RootLength));
                //DirectoryPath destinationDirectory = Path.Combine(fileCopy.DestinationRoot.FullName, dirArgs.Directory.FullName.Substring(fileCopy.RootLength));

                switch (@event)
                {
                    case FileCrawlEvent.FileFound:
                        if (destinationFile.Exists)
                        {
                            // check if overwriting is okay
                            if ((((CopyOptions)fileCopy.Options).CopyMode & CopyMode.Overwrite) == CopyMode.Overwrite)
                            {
                                if (file.DateModified > destinationFile.DateModified)
                                {
                                    // exception 1 of 3 to the overwrite rule
                                    if ((((CopyOptions)fileCopy.Options).CopyMode & CopyMode.DontOverwriteFileIfNewer) == CopyMode.DontOverwriteFileIfNewer)
                                        metadata.SkipThisFile(SkipReason.AlreadyExists, "source file is newer");
                                }
                                else if (destinationFile.DateModified == file.DateModified)
                                {
                                    // exception 2 of 3 to the overwrite rule
                                    if ((((CopyOptions)fileCopy.Options).CopyMode & CopyMode.DontOverwriteFileIfSameModifiedDate) == CopyMode.DontOverwriteFileIfSameModifiedDate)
                                        metadata.SkipThisFile(SkipReason.AlreadyExists, "source file has same modified date");
                                }

                                if ((((CopyOptions)fileCopy.Options).CopyMode & CopyMode.DontOverwriteFileIfMatchingHash) == CopyMode.DontOverwriteFileIfMatchingHash)
                                {
                                    // exception 3 of 3 to the overwrite rule
                                    if (Equals(Hash.String(file), Hash.String(destinationFile)))
                                        metadata.SkipThisFile(SkipReason.AlreadyExists, "source file has same hash");
                                }
                            }
                            else throw new DirectoryCrawlException("File already exists: " + destinationFile);
                        }
                        copyingFile?.Invoke(file, destinationFile, metadata);
                        if (!metadata.DryRun)
                        {
                            file.CopyTo(destinationFile.FullName);
                        }
                        fileCopied?.Invoke(file, destinationFile, metadata);
                        break;
                }
                fileCrawled?.Invoke(@event, file, metadata);  /* let client handle events too, if applicable */
            },
            options: options ?? new CopyOptions()
        )
        {
            DestinationRoot = destinationRoot;
        }
    }
}

