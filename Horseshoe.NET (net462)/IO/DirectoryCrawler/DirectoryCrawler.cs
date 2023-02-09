using System;
using System.IO;
using Horseshoe.NET.Collections;

namespace Horseshoe.NET.IO.DirectoryCrawler
{
    /// <summary>
    /// An adaptable, robust directory traversal engine that can accommodate many use cases
    /// and powers features such as a recursive delete using the metadata's built-in delete request, 
    /// <see cref="RecursiveCopy"/> and <see cref="RecursiveMove"/>.
    /// </summary>
    public class DirectoryCrawler : DirectoryCrawler<DirectoryPath>
    {
        /// <summary>
        /// Creates a new <c>DirectoryCrawler</c> with pluggable actions that let client code perform a wide variety of filesystem oriented tasks
        /// </summary>
        /// <param name="root">A directory to traverse</param>
        /// <param name="options">Optional traversal engine options.</param>
        /// <param name="directoryCrawled">
        /// A directory traversal event dispatcher to hook into.
        /// <example>
        /// Here's an example that prints out all the subdirectories of a directory:
        /// <code>
        /// using Horseshoe.NET.ConsoleX;
        /// using Horseshoe.NET.IO.DirectoryCrawler;
        /// 
        /// var subdirectories = new List&lt;string&gt;();
        /// new DirectoryCrawler
        /// (
        ///     @"C:\FolderToProcess",
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
        /// </example>
        /// </param>
        /// <param name="fileCrawled">
        /// A file traversal event dispatcher to hook into.
        /// <example>
        /// Here's an example that lists all files greater than 1 MB:
        /// <code>
        /// using Horseshoe.NET.ConsoleX;
        /// using Horseshoe.NET.IO;
        /// using Horseshoe.NET.IO.DirectoryCrawler;
        /// 
        /// var largeFiles = new List&lt;string&gt;();
        /// new DirectoryCrawler
        /// (
        ///     @"C:\FolderToProcess",
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
        ///     }
        /// ).Go();
        /// largeFiles.Sort();
        /// RenderX.List(largeFiles);
        /// </code>
        /// </example>
        /// </param>
        /// <param name="statistics">Optional traversal engine statistics.</param>
        public DirectoryCrawler
        (
            DirectoryPath root,
            CrawlOptions options = null,
            Action<DirectoryCrawlEvent, DirectoryPath, DirectoryMetadata<DirectoryPath>> directoryCrawled = null,
            Action<FileCrawlEvent, FilePath, FileMetadata<DirectoryPath>> fileCrawled = null,
            TraversalStatistics statistics = null
        ) : base
        (
            root,
            directoryCrawled: directoryCrawled,
            fileCrawled: fileCrawled,
            options: options,
            statistics: statistics
        )
        {
        }

        /// <summary>
        /// An action to perform at the end of the directory crawl process, returns a value that could represent the 
        /// cumulative or final result of the crawl operation. 
        /// </summary>
        /// <returns></returns>
        public override DirectoryPath CrawlComplete()
        {
            return Root;
        }

        /// <summary>
        /// <para>
        /// Executes a specialized <c>DirectoryCrawler</c> with the singular aim of recursively deleting a directory 
        /// / directories and all its / their contents.  Similar to...
        /// </para>
        /// <para>
        /// Usage:
        /// <code>
        /// using Horseshoe.NET.IO.DirectoryCrawler;
        /// 
        /// DirectoryCrawler.RecursiveDeleteDirectory
        /// (
        ///     @"C:\FolderToDelete",
        ///     precludeRootDirectory: true,  /* &lt;- can be accomplished with events, but this is simpler */
        ///     directoryCrawled: (@event, dir, metadata) =>
        ///     {
        ///         switch(@event)
        ///         {
        ///             case DirectoryCrawlEvent.DirectoryDeleting:
        ///                 Console.WriteLine("DIR DELETED: " + dir);
        ///                 break;
        ///         }
        ///     }
        ///     directoryCrawled: (@event, file, metadata) =>
        ///     {
        ///         switch(@event)
        ///         {
        ///             case DirectoryCrawlEvent.FileDeleting:
        ///                 Console.WriteLine("DELETING FILE: " + file);
        ///                 break;
        ///             case DirectoryCrawlEvent.FileErrored:
        ///                 Console.WriteLine("**FILE ERRORED: " + file + "**");
        ///                 break;
        ///         }
        ///     }
        /// );
        /// </code>
        /// </para>
        /// </summary>
        /// <param name="directoryPath">A directory path.</param>
        /// <param name="options">Optional traversal engine options.</param>
        /// <param name="precludeRootDirectory">If <c>true</c> leaves the root directory not deleted, default is <c>false</c>.</param>
        /// <param name="directoryCrawled">A directory traversal event dispatcher to hook into.</param>
        /// <param name="fileCrawled">A file traversal event dispatcher to hook into.</param>
        /// <param name="statistics">Optional traversal engine statistics.</param>
        public static DirectoryCrawler RecursiveDeleteDirectory
        (
            DirectoryPath directoryPath,
            CrawlOptions options = null,
            bool precludeRootDirectory = false,
            Action<DirectoryCrawlEvent, DirectoryPath, DirectoryMetadata<DirectoryPath>> directoryCrawled = null,
            Action<FileCrawlEvent, FilePath, FileMetadata<DirectoryPath>> fileCrawled = null,
            TraversalStatistics statistics = null
        )
        {
            var dirCrawl = new DirectoryCrawler
            (
                root: directoryPath,
                options: new CrawlOptions(options) { CrawlMode = CrawlMode.DeleteDirectories },
                directoryCrawled: (@event, dir, metadata) =>
                {
                    directoryCrawled?.Invoke(@event, dir, metadata);  /* let client handle events first, if applicable */
                    
                    switch (@event)
                    {
                        case DirectoryCrawlEvent.DirectoryDeleting:
                            if (dir == directoryPath /* or metadata.Level == 0 */ && precludeRootDirectory)
                            {
                                metadata.SkipThisDirectory(SkipReason.RootDirectoryNotDeleted);
                            }
                            break;
                    }
                },
                fileCrawled: fileCrawled,
                statistics: statistics
            );
            dirCrawl.Go();
            return dirCrawl;
        }

        /// <summary>
        /// Executes a specialized <c>DirectoryCrawler</c> with the singular aim of reporting the total size of the processed contents.
        /// </summary>
        /// <param name="directoryPath">A directory path.</param>
        /// <param name="directoryCrawled">A directory traversal event dispatcher to hook into.</param>
        /// <param name="fileCrawled">A file traversal event dispatcher to hook into.</param>
        /// <param name="options">Optional traversal engine options.</param>
        /// <param name="statistics">Optional traversal engine statistics.</param>
        public static long GetTotalSize
        (
            DirectoryPath directoryPath,
            CrawlOptions options = null,
            Action<DirectoryCrawlEvent, DirectoryPath, DirectoryMetadata<long>> directoryCrawled = null,
            Action<FileCrawlEvent, FilePath, FileMetadata<long>> fileCrawled = null,
            TraversalStatistics statistics = null
        )
        {
            var dirCrawl = new DirectoryCrawler<long>
            (
                root: directoryPath,
                options: new CrawlOptions(options) { StatisticsOn = true },
                directoryCrawled: directoryCrawled,
                fileCrawled: fileCrawled,
                statistics: statistics
            );
            dirCrawl.Go();
            return dirCrawl.Statistics.SizeOfFilesProcessed;
        }
    }

    /// <summary>
    /// A generic, adaptable, robust directory traversal engine that can accommodate many use cases
    /// and powers some neat capabilities.  See
    /// <see cref="DirectoryCrawler.RecursiveDeleteDirectory(DirectoryPath, CrawlOptions, bool, Action{DirectoryCrawlEvent, DirectoryPath, DirectoryMetadata{DirectoryPath}}, Action{FileCrawlEvent, FilePath, FileMetadata{DirectoryPath}}, TraversalStatistics)"/>,
    /// <see cref="DirectoryCrawler.GetTotalSize(DirectoryPath, CrawlOptions, Action{DirectoryCrawlEvent, DirectoryPath, DirectoryMetadata{long}}, Action{FileCrawlEvent, FilePath, FileMetadata{long}}, TraversalStatistics)"/> and
    /// <see cref="Crypto.RecursiveHash"/>
    /// </summary>
    public class DirectoryCrawler<T>
    {
        private bool _stopped;

        /// <summary>
        /// A directory to traverse.
        /// </summary>
        public DirectoryPath Root { get; }

        /// <summary>
        /// A directory traversal event dispatcher to hook into.
        /// <example>
        /// Here's an example that prints out all the subdirectories of a directory:
        /// <code>
        /// using Horseshoe.NET.ConsoleX;
        /// using Horseshoe.NET.IO.DirectoryCrawler;
        /// 
        /// var subdirectories = new List&lt;string&gt;();
        /// new DirectoryCrawler
        /// (
        ///     @"C:\FolderToProcess",
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
        /// </example>
        /// </summary>
        public Action<DirectoryCrawlEvent, DirectoryPath, DirectoryMetadata<T>> DirectoryCrawled { get; protected set; }

        /// <summary>
        /// A file traversal event dispatcher to hook into.
        /// <example>
        /// Here's an example that lists all files greater than 1 MB:
        /// <code>
        /// using Horseshoe.NET.ConsoleX;
        /// using Horseshoe.NET.IO;
        /// using Horseshoe.NET.IO.DirectoryCrawler;
        /// 
        /// var largeFiles = new List&lt;string&gt;();
        /// new DirectoryCrawler
        /// (
        ///     @"C:\FolderToProcess",
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
        ///     }
        /// ).Go();
        /// largeFiles.Sort();
        /// RenderX.List(largeFiles);
        /// </code>
        /// </example>
        /// </summary>
        public Action<FileCrawlEvent, FilePath, FileMetadata<T>> FileCrawled { get; protected set; }

        /// <summary>
        /// An optional, client-supplied action for processing a file during each "processing" phase of the directory traversal.
        /// Note, "dry run" mode is honored. Meaning if <c>Options.DryRun == true</c> this action will not be invoked.
        /// </summary>
        /// <remarks>
        /// <para>
        /// It makes sense in many implementations to set this property with a new action for each file
        /// by hooking into <see cref="FileCrawled"/> and listening for the "processing" phase. 
        /// </para>
        /// <example>
        /// Here's an example, this is how <see cref="RecursiveMove"/> achieves a file-specific action in each 'processing' phase.
        /// <code>
        /// FileCrawled = (@event, file, metadata) =>
        /// {
        ///     fileCrawled?.Invoke(@event, file, metadata);  /* let client handle events first */
        ///
        ///     FilePath destinationFile = Path.Combine(@"C:\", file.FullName.Substring(Root.Length));
        ///
        ///     switch (@event)
        ///     {
        ///         case FileCrawlEvent.FileProcessing:
        ///             FileProcess =  /* Note: honors "dry run" mode */
        ///                 (file0, metadata0) =>
        ///                 {
        ///                     file0.MoveTo(destinationFile);  // uses built-in tallying
        ///                 };  
        ///             break;
        ///     }
        /// };
        /// </code>
        /// </example>
        /// </remarks>
        public Action<FilePath, FileMetadata<T>> FileProcess { get; set; }

        /// <summary>
        /// Crawl options
        /// </summary>
        public CrawlOptions Options { get; }

        /// <summary>
        /// A small group of basic statistics gathered over the course of the directory crawl operation, 
        /// e.g. number of files and directory scanned, number of files and directories skipped and 
        /// total combined size of all the scanned files
        /// </summary>
        public TraversalStatistics Statistics { get; }

        /// <summary>
        /// Creates a new <c>DirectoryCrawler</c> with pluggable actions that let client code perform a wide variety of filesystem oriented tasks
        /// </summary>
        /// <param name="root">A directory to traverse</param>
        /// <param name="options">Optional traversal engine options.</param>
        /// <param name="directoryCrawled">
        /// A pluggable action that leverages the directory traversal engine to easily and declaratively execute directory oriented tasks
        /// <para>
        /// Here's an example that prints out all the subdirectories of a directory:
        /// <code>
        /// using Horseshoe.NET.ConsoleX;
        /// using Horseshoe.NET.IO.DirectoryCrawler;
        /// 
        /// var subdirectories = new List&lt;string&gt;();
        /// new DirectoryCrawler
        /// (
        ///     @"C:\FolderToProcess",
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
        /// </param>
        /// <param name="fileCrawled">
        /// A pluggable action that leverages the directory traversal engine to easily and declaratively execute file oriented tasks
        /// <example>
        /// Here's an example that lists all files greater than 1 MB:
        /// <code>
        /// using Horseshoe.NET.ConsoleX;
        /// using Horseshoe.NET.IO;
        /// using Horseshoe.NET.IO.DirectoryCrawler;
        /// 
        /// var largeFiles = new List&lt;string&gt;();
        /// new DirectoryCrawler
        /// (
        ///     @"C:\FolderToProcess",
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
        ///     }
        /// ).Go();
        /// largeFiles.Sort();
        /// RenderX.List(largeFiles);
        /// </code>
        /// </example>
        /// </param>
        /// <param name="statistics">Optional traversal engine statistics.</param>
        public DirectoryCrawler
        (
            DirectoryPath root,
            CrawlOptions options = null,
            Action<DirectoryCrawlEvent, DirectoryPath, DirectoryMetadata<T>> directoryCrawled = null,
            Action<FileCrawlEvent, FilePath, FileMetadata<T>> fileCrawled = null,
            TraversalStatistics statistics = null
        )
        {
            Root = root;
            DirectoryCrawled = directoryCrawled;
            FileCrawled = fileCrawled;
            Options = options ?? new CrawlOptions();
            Statistics = statistics ?? new TraversalStatistics();
        }

        /// <summary>
        /// Subclasses can execute initialization code by overriding this method.
        /// <example>
        /// Alternatively, initialization logic can also go here...
        /// <code>
        /// using Horseshoe.NET.IO.DirectoryCrawler;
        /// 
        /// new DirectoryCrawler
        /// (
        ///     @"C:\FolderToProcess",
        ///     directoryCrawled: (@event, dir, metadata) =&gt;
        ///     {
        ///         switch(@event) 
        ///         {
        ///             case DirectoryCrawlEvent.OnInit:
        ///                 // your code goes here
        ///                 break;
        ///         }
        ///     }
        /// ).Go();
        /// </code>
        /// </example>
        /// </summary>
        public virtual void InitCrawl() { }

        /// <summary>
        /// Subclasses can execute finalization code by overriding this method.
        /// <example>
        /// Alternatively, finalization logic can also go here...
        /// <code>
        /// using Horseshoe.NET.IO.DirectoryCrawler;
        /// 
        /// new DirectoryCrawler
        /// (
        ///     @"C:\FolderToProcess",
        ///     directoryCrawled: (@event, dir, metadata) =&gt;
        ///     {
        ///         switch(@event) 
        ///         {
        ///             case DirectoryCrawlEvent.OnComplete:
        ///                 // your code goes here
        ///                 break;
        ///         }
        ///     }
        /// ).Go();
        /// </code>
        /// </example>
        /// </summary>
        public virtual T CrawlComplete() { return default; }

        /// <summary>
        /// Starts the directory traversal engine and returns the result of <c>CrawlComplete()</c>
        /// </summary>
        /// <returns>The result of the traversal operation (see <see cref="CrawlComplete"/>.</returns>
        public T Go()
        {
            // validation
            if (!Root.Exists)
                throw new DirectoryCrawlException("Root directory not found: " + Root);

            // populate variables
            var rootDirMetadata = new DirectoryMetadata<T>(0, this, dryRun: Options.DryRun, statistics: Statistics);
            _stopped = false;

            // start the directory traversal engine
            InitCrawl();

            // override the crawl mode, if applicable
            DirectoryCrawled?.Invoke(DirectoryCrawlEvent.OnInit, Root, rootDirMetadata);

            // begin traversing by entering the first (root) directory
            if (!_stopped)
            {
                _Go(Root, false, Options.CrawlMode == CrawlMode.DeleteDirectories, Options.CrawlMode.In(CrawlMode.DeleteDirectories, CrawlMode.DeleteFiles), 0);
            }

            // traversal has stopped (ended naturally or stopped e.g. due to an uncaught exception)
            if (_stopped)
            {
                // if stopped, better to return a null or zero rather than a partial answer which can still be found in the statistics.
                return default;
            }
            T t = CrawlComplete();
            DirectoryCrawled?.Invoke(DirectoryCrawlEvent.OnComplete, Root, new DirectoryMetadata<T>(rootDirMetadata) { CompletedResult = t });
            return t;
        }

        void _Go(DirectoryPath dir, bool processingFiles, bool deletingDirectory, bool deletingFiles, int level)
        {
            // stop recursion
            if (_stopped)
                return;

            // variables
            var curDirMetadata = new DirectoryMetadata<T>(level, this, dryRun: Options.DryRun, statistics: Statistics);
            bool processingThisDirectory;

            try
            {
                // validation
                if (!dir.Exists)
                {
                    Stop();
                    throw new DirectoryCrawlException("directory does not exist: " + dir);
                }

                // skip filtered out dir, if applicable
                if (Options.DirectoryFilter == null || Options.DirectoryFilter.IsIncluded(dir))
                {
                    processingFiles = true;
                }

                // enter directory - publish event may give client a chance to skip or delete
                if (Options.StatisticsOn)
                {
                    Statistics.DirectoriesProcessed++;
                }
                DirectoryCrawled?.Invoke(DirectoryCrawlEvent.DirectoryEntered, dir, curDirMetadata);
                if (curDirMetadata.ProcessFiles)    processingFiles   = true;  // manual overrides
                if (curDirMetadata.DeleteDirectory) deletingDirectory = true;
                if (curDirMetadata.DeleteFiles)     deletingFiles     = true;
                processingThisDirectory = processingFiles && !curDirMetadata.SkipDirectory;

                /* * * * * * * * * * * * * * * * * * * *
                 *           Process Files
                 * * * * * * * * * * * * * * * * * * * */
                FileMetadata<T> fileMetadata = new FileMetadata<T>(level + 1, this, dryRun: Options.DryRun, statistics: Statistics);
                long fileSize = 0L;

                if (processingThisDirectory || Options.StatisticsOn)
                {
                    // load variables
                    var files = Options.FileSearchPattern != null
                        ? dir.GetFiles(Options.FileSearchPattern)
                        : dir.GetFiles();

                    // iterate files
                    foreach (var file in files)
                    {
                        var fileMetadataCopy = fileMetadata.Clone();

                        // skip filtered out files, if applicable...
                        if (Options.FileFilter != null && !Options.FileFilter.IsIncluded(file))
                            continue;

                        if (Options.StatisticsOn)
                        {
                            fileSize = file.Length;
                        }

                        // scenario 1 of 5 - skipping this directory
                        if (!processingThisDirectory)
                        {
                            if (Options.StatisticsOn)
                            {
                                Statistics.FilesSkipped++;
                                Statistics.SizeOfFilesSkipped = file.Length;
                            }
                            FileCrawled?.Invoke(FileCrawlEvent.FileSkipped, file, new FileMetadata<T>(fileMetadataCopy) { SkipReason = curDirMetadata.SkipDirectory ? SkipReason.ClientSkipped : (Options.DirectoryFilter != null ? SkipReason.ClientFiltered : SkipReason.AutoSkipped) });
                            continue;
                        }

                        // scenario 2 of 5 - deleting files
                        if (deletingDirectory || deletingFiles)
                        {
                            DoFileDelete(file, fileSize, fileMetadata);
                            continue;
                        }

                        // publish 'file processing' event - this may give client a chance to individually skip or delete
                        FileCrawled?.Invoke(FileCrawlEvent.FileProcessing, file, fileMetadataCopy);

                        // scenario 3 of 5 - processing the file - client skips
                        if (fileMetadataCopy.SkipFile)
                        {
                            if (Options.StatisticsOn)
                            {
                                Statistics.FilesSkipped++;
                                Statistics.SizeOfFilesSkipped = file.Length;
                            }
                            FileCrawled?.Invoke(FileCrawlEvent.FileSkipped, file, new FileMetadata<T>(fileMetadataCopy) { SkipReason = fileMetadataCopy.SkipReason });
                            continue;
                        }

                        // scenario 4 of 5 - processing the file - deletes
                        if (fileMetadataCopy.DeleteFile)
                        {
                            DoFileDelete(file, fileSize, fileMetadataCopy);
                            continue;
                        }

                        // scenario 5 of 5 - processing the file normally
                        try
                        {
                            // process the file using client action, if supplied
                            if (!Options.DryRun)
                            {
                                FileProcess?.Invoke(file, fileMetadataCopy);
                            }

                            // tally
                            if (Options.StatisticsOn)
                            {
                                Statistics.FilesProcessed++;
                                Statistics.SizeOfFilesProcessed += fileSize;
                            }

                            // publish 'file processed' event
                            FileCrawled?.Invoke(FileCrawlEvent.FileProcessed, file, fileMetadataCopy);
                        }
                        catch (DirectoryCrawlHaltedException)
                        {
                            throw;
                        }
                        catch (Exception ex)
                        {
                            if (Options.ReportErrorsAndContinue)
                            {
                                if (Options.StatisticsOn)
                                {
                                    Statistics.FilesErrored++;
                                }
                                FileCrawled?.Invoke(FileCrawlEvent.FileErrored, file, new FileMetadata<T>(fileMetadata) { Exception = ex });
                            }
                            else throw ex;
                        }
                    }
                }

                /* * * * * * * * * * * * * * * * * * * *
                 *         Process Directories
                 * * * * * * * * * * * * * * * * * * * */

                var subDirs = Options.DirectorySearchPattern != null
                    ? dir.GetDirectories(Options.DirectorySearchPattern)
                    : dir.GetDirectories();

                foreach (var subDir in subDirs)
                {
                    _Go(subDir, processingFiles, deletingDirectory, deletingFiles, level + 1);
                    if (_stopped)
                        return;
                }

                /* * * * * * * * * * * * * * * * * * * *
                 *      Finalize Current Directory
                 * * * * * * * * * * * * * * * * * * * */

                if (!processingThisDirectory)
                {
                    if (Options.StatisticsOn)
                    {
                        Statistics.DirectoriesProcessed--;
                        Statistics.DirectoriesSkipped++;
                    }
                    DirectoryCrawled?.Invoke(DirectoryCrawlEvent.DirectorySkipped, dir, new DirectoryMetadata<T>(curDirMetadata) { SkipReason = curDirMetadata.SkipDirectory ? SkipReason.ClientSkipped : (Options.DirectoryFilter != null ? SkipReason.ClientFiltered : SkipReason.AutoSkipped) });
                    return;
                }
                if (deletingDirectory)
                {
                    if (Options.StatisticsOn)
                    {
                        Statistics.DirectoriesProcessed--;
                    }
                    DoDirectoryDelete(dir, curDirMetadata);
                    return;
                }

                DirectoryCrawled?.Invoke(DirectoryCrawlEvent.DirectoryExited, dir, curDirMetadata);
            }
            catch (DirectoryCrawlHaltedException) 
            {
                DirectoryCrawled?.Invoke(DirectoryCrawlEvent.OnHalt, dir, curDirMetadata);
            }
            catch (Exception ex)
            {
                DirectoryCrawled?.Invoke(DirectoryCrawlEvent.OnError, dir, new DirectoryMetadata<T>(curDirMetadata) { Exception = ex });
                throw;
            }
        }

        /// <summary>
        /// Deletes a directory and performs the corresponding event signaling and tallying (does not actually delete the directory if in 'dry run' mode).
        /// </summary>
        /// <param name="dir">A directory.</param>
        /// <param name="dirMetadata">Directory metadata.</param>
        public void DoDirectoryDelete(DirectoryInfo dir, DirectoryMetadata<T> dirMetadata)
        {
            var dirMetadataDel = new DirectoryMetadata<T>(dirMetadata);
            DirectoryCrawled?.Invoke(DirectoryCrawlEvent.DirectoryDeleting, dir, dirMetadataDel);

            if (Options.StatisticsOn)
            {
                Statistics.DirectoriesDeleted++;
            }

            if (!Options.DryRun)
            {
                try
                {
                    if (dir.IsEmpty())
                    {
                        dir.Delete();
                    }
                    DirectoryCrawled?.Invoke(DirectoryCrawlEvent.DirectoryDeleted, dir, dirMetadata);
                }
                catch (Exception ex)
                {
                    if (Options.StatisticsOn)
                    {
                        Statistics.DirectoriesDeleted--;
                        Statistics.DirectoriesErrored++;
                    }

                    if (Options.ReportErrorsAndContinue)
                    {
                        DirectoryCrawled?.Invoke(DirectoryCrawlEvent.DirectoryErrored, dir, new DirectoryMetadata<T>(dirMetadata) { Exception = ex });
                        return;
                    }
                    throw;
                }
            }
        }

        /// <summary>
        /// Deletes a file and performs the corresponding event signaling and tallying (does not actually delete the file if in 'dry run' mode).
        /// </summary>
        /// <param name="file">A file.</param>
        /// <param name="fileMetadata">File metadata.</param>
        protected void DoFileDelete(FileInfo file, FileMetadata<T> fileMetadata)
        {
            DoFileDelete(file, file.Length, fileMetadata);
        }

        /// <summary>
        /// Deletes a file and performs the corresponding event signaling and tallying (does not actually delete the file if in 'dry run' mode).
        /// </summary>
        /// <param name="file">A file.</param>
        /// <param name="fileSize">The file size (passed in separately for optimization purposes only).</param>
        /// <param name="fileMetadata">File metadata.</param>
        protected void DoFileDelete(FileInfo file, long fileSize, FileMetadata<T> fileMetadata)
        {
            FileCrawled?.Invoke(FileCrawlEvent.FileDeleting, file, fileMetadata);

            if (Options.StatisticsOn)
            {
                Statistics.FilesDeleted++;
                Statistics.SizeOfFilesDeleted += fileSize;
            }

            if (!Options.DryRun)
            {
                try
                {
                    file.Delete();
                    FileCrawled?.Invoke(FileCrawlEvent.FileDeleted, file, fileMetadata);
                }
                catch (Exception ex)
                {
                    if (Options.StatisticsOn)
                    {
                        Statistics.FilesDeleted--;
                        Statistics.SizeOfFilesDeleted -= fileSize;
                        Statistics.FilesErrored++;
                    }

                    if (Options.ReportErrorsAndContinue)
                    {
                        FileCrawled?.Invoke(FileCrawlEvent.FileErrored, file, new FileMetadata<T>(fileMetadata) { Exception = ex });
                        return;
                    }
                    throw;
                }
            }
        }

        /// <summary>
        /// Sets a flag that will naturally end the directory traversal
        /// </summary>
        public void Stop()
        {
            _stopped = true;
        }
    }
}
