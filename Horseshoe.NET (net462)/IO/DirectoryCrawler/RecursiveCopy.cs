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
        //private DirectoryCrawlStatistics statistics = new DirectoryCrawlStatistics();

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
        ///     fileCrawled: (@event, file, metadata) =&gt;
        ///     {
        ///         switch(@event) 
        ///         {
        ///             case FileCrawlEvent.FileProcessing:
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
        /// <param name="destinationDirectoryCreating"></param>
        /// <param name="destinationDirectoryCreated"></param>
        /// <param name="options">Optional traversal engine options.</param>
        /// <param name="statistics">Optional traversal engine statistics.</param>
        /// <exception cref="DirectoryCrawlException"></exception>
        public RecursiveCopy
        (
            DirectoryPath sourceRoot,
            DirectoryPath destinationRoot,
            Action<DirectoryCrawlEvent, DirectoryPath, DirectoryMetadata<DirectoryPath>> directoryCrawled = null,
            Action<FileCrawlEvent, FilePath, FileMetadata<DirectoryPath>> fileCrawled = null,
            Action<DirectoryPath, DirectoryMetadata<DirectoryPath>> destinationDirectoryCreating = null,
            Action<DirectoryPath, DirectoryMetadata<DirectoryPath>> destinationDirectoryCreated = null,
            CopyOptions options = null,
            TraversalStatistics statistics = null
        ) : base
        (
            sourceRoot,
            options: options ?? new CopyOptions(),
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
                            if (((CopyOptions)Options).CopyMode == CopyMode.ErrorIfDestinationNotEmpty && !destinationDirectory.IsEmpty())
                                throw new DirectoryCrawlException("Directory already exists and is not empty: " + destinationDirectory);

                            if ((((CopyOptions)Options).CopyMode & CopyMode.DeleteDestinationDirectoryBeforeCopy) == CopyMode.DeleteDestinationDirectoryBeforeCopy)
                            {
                                RecursiveDeleteDirectory
                                (
                                    destinationDirectory,
                                    options: new CrawlOptions { DryRun = Options.DryRun, ReportErrorsAndContinue = Options.ReportErrorsAndContinue, StatisticsOn = Options.StatisticsOn },
                                    statistics: Statistics
                                );
                            }
                        }
                        else
                        {
                            destinationDirectoryCreating?.Invoke(dir, metadata);
                            if (!Options.DryRun)
                            {
                                destinationDirectory.Create();   // <- refers to the destination root
                            }
                            destinationDirectoryCreated?.Invoke(dir, metadata);
                        }
                        break;

                    case DirectoryCrawlEvent.DirectoryEntered:
                        if (destinationDirectory.Exists)
                        {
                            if ((((CopyOptions)Options).CopyMode & CopyMode.RemoveDestinationFilesAndDirectoriesNotInSource) == CopyMode.RemoveDestinationFilesAndDirectoriesNotInSource)
                            {
                                var sourceFileNames = dir.GetFiles()
                                    .Select(f => f.Name)
                                    .ToArray();
                                var sourceSubdirNames = dir.GetDirectories()
                                    .Select(d => d.Name)
                                    .ToArray();
                                var destFiles = destinationDirectory.GetFiles();
                                var destSubdirs = destinationDirectory.GetDirectories();
                                var fileMedatadaDel = new FileMetadata<DirectoryPath>(metadata.Level + 1, metadata.DirectoryCrawler, dryRun: metadata.DryRun, statistics: Statistics);

                                // removing destination files that are not in the source
                                foreach (var destFile in destFiles)
                                {
                                    if (!destFile.Name.In(sourceFileNames))
                                    {
                                        DoFileDelete(destFile, fileMedatadaDel);
                                    }
                                }

                                // removing destination directories that are not in the source
                                foreach (var destSubdir in destSubdirs)
                                {
                                    RecursiveDeleteDirectory
                                    (
                                        destinationDirectory,
                                        options: new CrawlOptions { DryRun = Options.DryRun, ReportErrorsAndContinue = Options.ReportErrorsAndContinue, StatisticsOn = Options.StatisticsOn },
                                        statistics: Statistics
                                    );
                                }
                            }
                        }
                        else
                        {
                            destinationDirectoryCreating?.Invoke(destinationDirectory, metadata);
                            if (!metadata.DryRun)
                            {
                                destinationDirectory.Create();
                            }
                            destinationDirectoryCreated?.Invoke(destinationDirectory, metadata);
                        }
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
                        if (destinationFile.Exists)
                        {
                            // check if overwriting is okay
                            if ((((CopyOptions)Options).CopyMode & CopyMode.Overwrite) == CopyMode.Overwrite)
                            {
                                // exception 1 of 3 - source file is newer
                                if (file.DateModified > destinationFile.DateModified && (((CopyOptions)Options).CopyMode & CopyMode.DoNotOverwriteFileIfNewer) == CopyMode.DoNotOverwriteFileIfNewer)
                                { 
                                    metadata.SkipThisFile(SkipReason.AlreadyExists);
                                    return;
                                }

                                // exception 2 of 3 - source file has same modified date
                                if (destinationFile.DateModified == file.DateModified && (((CopyOptions)Options).CopyMode & CopyMode.DoNotOverwriteFileIfSameModifiedDate) == CopyMode.DoNotOverwriteFileIfSameModifiedDate)
                                { 
                                    metadata.SkipThisFile(SkipReason.AlreadyExists);
                                    return;
                                }

                                // exception 3 of 3 - source file has same hash
                                if ((((CopyOptions)Options).CopyMode & CopyMode.DoNotOverwriteFileIfMatchingHash) == CopyMode.DoNotOverwriteFileIfMatchingHash && Equals(Hash.String(file), Hash.String(destinationFile)))
                                {
                                    metadata.SkipThisFile(SkipReason.AlreadyExists);
                                    return;
                                }
                            }
                            else throw new DirectoryCrawlException("File already exists: " + destinationFile);
                        }

                        FileProcess =  /* Note: honors "dry run" mode */
                            (file0, metadata0) => 
                            {
                                file0.CopyTo(destinationFile);  // uses built-in tallying
                            };
                        break;
                }
            };
        }
    }
}

