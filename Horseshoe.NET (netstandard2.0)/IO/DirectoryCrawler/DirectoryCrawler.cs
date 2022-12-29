using System;

namespace Horseshoe.NET.IO.DirectoryCrawler
{
    /// <summary>
    /// An adaptable, robust directory traversal engine that can accommodate many use cases
    /// and powers features such as <see cref="RecursiveCopy"/>, <see cref="RecursiveDelete"/> 
    /// and <see cref="RecursiveMove"/>
    /// </summary>
    public class DirectoryCrawler : DirectoryCrawler<DirectoryPath>
    {
        /// <summary>
        /// Creates a new <c>DirectoryCrawler</c> with pluggable actions that let client code perform a wide variety of filesystem oriented tasks
        /// </summary>
        /// <param name="root">A directory to traverse</param>
        /// <param name="directoryCrawled">
        /// A an optional, pluggable action that leverages the directory traversal engine to easily and declaratively execute directory oriented tasks
        /// <example>
        /// Here's an example that prints out all the subdirectories of a directory:
        /// <code>
        /// using Horseshoe.NET.ConsoleX;
        /// using Horseshoe.NET.IO.DirectoryCrawler;
        /// 
        /// var subdirectories = new List&lt;string&gt;();
        /// new DirectoryCrawler
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
        /// </example>
        /// </param>
        /// <param name="fileCrawled">
        /// A an optional, pluggable action that leverages the directory traversal engine to easily and declaratively execute file oriented tasks
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
        ///     }
        /// ).Go();
        /// largeFiles.Sort();
        /// RenderX.List(largeFiles);
        /// </code>
        /// </example>
        /// </param>
        /// <param name="options">crawler options</param>
        public DirectoryCrawler
        (
            DirectoryPath root,
            Action<DirectoryCrawlEvent, DirectoryPath, DirectoryMetadata<DirectoryPath>> directoryCrawled = null,
            Action<FileCrawlEvent, FilePath, FileMetadata<DirectoryPath>> fileCrawled = null,
            CrawlOptions options = null
        ) : base
        (
            root,
            directoryCrawled: directoryCrawled,
            fileCrawled: fileCrawled,
            options: options
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
    }

    /// <summary>
    /// A generic, adaptable, robust directory traversal engine that can accommodate many use cases
    /// and powers features such as <see cref="RecursiveSize"/> and <see cref="Horseshoe.NET.Crypto.RecursiveHash"/>
    /// </summary>
    public class DirectoryCrawler<T>
    {
        private bool _stopped;

        /// <summary>
        /// A directory to traverse
        /// </summary>
        public DirectoryPath Root { get; }

        /// <summary>
        /// The cached length of <c>Root</c>, useful in situations that perform a large amount of 
        /// string manipulation such as <c>RecursiveCopy</c> and <c>RecursiveMove</c>
        /// </summary>
        public int RootLength { get; }

        /// <summary>
        /// A pluggable action that leverages the directory traversal engine to easily and declaratively execute directory oriented tasks
        /// <example>
        /// Here's an example that prints out all the subdirectories of a directory:
        /// <code>
        /// using Horseshoe.NET.ConsoleX;
        /// using Horseshoe.NET.IO.DirectoryCrawler;
        /// 
        /// var subdirectories = new List&lt;string&gt;();
        /// new DirectoryCrawler
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
        /// </example>
        /// </summary>
        public Action<DirectoryCrawlEvent, DirectoryPath, DirectoryMetadata<T>> DirectoryCrawled { get; }

        /// <summary>
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
        ///     }
        /// ).Go();
        /// largeFiles.Sort();
        /// RenderX.List(largeFiles);
        /// </code>
        /// </example>
        /// </summary>
        public Action<FileCrawlEvent, FilePath, FileMetadata<T>> FileCrawled { get; }

        /// <summary>
        /// Crawl options
        /// </summary>
        public CrawlOptions Options { get; }

        /// <summary>
        /// A small group of basic statistics gathered over the course of the directory crawl operation, 
        /// e.g. number of files and directory scanned, number of files and directories skipped and 
        /// total combined size of all the scanned files
        /// </summary>
        public DirectoryCrawlStatistics Statistics { get; private set; }

        /// <summary>
        /// Creates a new <c>DirectoryCrawler</c> with pluggable actions that let client code perform a wide variety of filesystem oriented tasks
        /// </summary>
        /// <param name="root">A directory to traverse</param>
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
        ///     }
        /// ).Go();
        /// largeFiles.Sort();
        /// RenderX.List(largeFiles);
        /// </code>
        /// </example>
        /// </param>
        /// <param name="options">crawler options</param>
        public DirectoryCrawler
        (
            DirectoryPath root,
            Action<DirectoryCrawlEvent, DirectoryPath, DirectoryMetadata<T>> directoryCrawled = null,
            Action<FileCrawlEvent, FilePath, FileMetadata<T>> fileCrawled = null,
            CrawlOptions options = null
        )
        {
            Root = root;
            RootLength = root.FullName.Length;
            DirectoryCrawled = directoryCrawled;
            FileCrawled = fileCrawled;
            Options = options ?? new CrawlOptions();
            Statistics = new DirectoryCrawlStatistics();
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
        ///     @"C:\myFilesAndFolders",
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
        ///     @"C:\myFilesAndFolders",
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
        /// <returns></returns>
        public T Go()
        {
            // validation
            if (!Root.Exists)
                throw new DirectoryCrawlException("Root directory not found: " + Root);

            // populate variables
            Statistics = new DirectoryCrawlStatistics();
            var rootDirMetadata = new DirectoryMetadata<T>(0, this, dryRun: Options.DryRun, statistics: Statistics);
            _stopped = false;

            // start the directory traversal engine
            InitCrawl();
            DirectoryCrawled?.Invoke(DirectoryCrawlEvent.OnInit, Root, rootDirMetadata);
            if (!_stopped)
            {
                _Go(Root, 0);
            }
            if (!_stopped)
            {
                T t = CrawlComplete();
                DirectoryCrawled?.Invoke(DirectoryCrawlEvent.OnComplete, Root, rootDirMetadata);
                return t;
            }
            return default;
        }

        void _Go(DirectoryPath dir, int level)
        {
            // stop recursion
            if (_stopped)
                return;

            // current directory
            var curDirMetadata = new DirectoryMetadata<T>(level, this, dryRun: Options.DryRun, statistics: Statistics);

            // check auto-skip root dir and dir filter
            if (Options.DirectoryFilter != null && !Options.DirectoryFilter.IsIncluded(dir))
            {
                _bulkSkipDir(dir, curDirMetadata, SkipReason.ClientFiltered);
                return;
            }

            // validation
            if (!dir.Exists)
            {
                Stop();
                var curDirectoryMetadataEx = new DirectoryMetadata<T>(curDirMetadata) { Exception = new DirectoryCrawlException("Directory does not exist: " + dir) };
                DirectoryCrawled?.Invoke(DirectoryCrawlEvent.OnHalt, dir, curDirectoryMetadataEx);
                return;
            }

            Statistics.DirectoriesCrawled++;

            try
            {
                DirectoryCrawled?.Invoke(DirectoryCrawlEvent.DirectoryEntered, dir, curDirMetadata);
            }
            catch (DirectorySkippedException)
            {
                Statistics.DirectoriesCrawled--;
                _bulkSkipDir(dir, curDirMetadata, SkipReason.ClientSkipped);
                return;
            }
            catch (DirectoryCrawlHaltedException)
            {
                Stop();
                DirectoryCrawled?.Invoke(DirectoryCrawlEvent.OnHalt, dir, curDirMetadata);
                return;
            }
            catch (Exception ex)
            {
                Statistics.DirectoriesErrored++;
                if (Options.ReportErrorsAndContinue)
                {
                    var curDirectoryMetadataEx = new DirectoryMetadata<T>(curDirMetadata) { Exception = ex };
                    DirectoryCrawled?.Invoke(DirectoryCrawlEvent.DirectoryErrored, dir, curDirectoryMetadataEx);
                }
                else throw ex;
            }

            // process files first
            if (!Options.DirectoriesOnly)
            {
                FileMetadata<T> fileMetadata;
                var files = Options.FileSearchPattern != null
                    ? dir.GetFiles(Options.FileSearchPattern)
                    : dir.GetFiles();

                foreach (var file in files)
                {
                    fileMetadata = new FileMetadata<T>(level + 1, this, dryRun: Options.DryRun);

                    // check file filter
                    if (Options.FileFilter != null && !Options.FileFilter.IsIncluded(file))
                    {
                        Statistics.FilesSkipped++;
                        var fileMetadataSk = new FileMetadata<T>(fileMetadata) { SkipReason = SkipReason.ClientFiltered };
                        FileCrawled?.Invoke(FileCrawlEvent.FileSkipped, file, fileMetadataSk);
                        continue;
                    }

                    Statistics.FilesCrawled++;

                    try
                    {
                        Statistics.SizeOfFilesCrawled += file.Length;
                        FileCrawled?.Invoke(FileCrawlEvent.FileFound, file, fileMetadata);
                    }
                    catch (FileSkippedException fsex)
                    {
                        Statistics.FilesCrawled--;
                        Statistics.SizeOfFilesCrawled -= file.Length;
                        Statistics.FilesSkipped++;
                        var fileMetadataEx = new FileMetadata<T>(fileMetadata) { SkipReason = fsex.SkipReason, SkipComment = fsex.SkipComment };
                        FileCrawled?.Invoke(FileCrawlEvent.FileSkipped, file, fileMetadataEx);
                        continue;
                    }
                    catch (DirectoryCrawlHaltedException)
                    {
                        Stop();
                        FileCrawled?.Invoke(FileCrawlEvent.OnHalt, file, fileMetadata);
                        return;
                    }
                    catch (Exception ex)
                    {
                        if (Options.ReportErrorsAndContinue)
                        {
                            Statistics.FilesErrored++;
                            var fileMetadataEx = new FileMetadata<T>(fileMetadata) { Exception = ex };
                            FileCrawled?.Invoke(FileCrawlEvent.FileErrored, file, fileMetadataEx);
                        }
                        else throw ex;
                    }
                }
            }

            // process subdirectories last
            var subDirs = Options.DirectorySearchPattern != null
                ? dir.GetDirectories(Options.DirectorySearchPattern)
                : dir.GetDirectories();
            foreach (var subDir in subDirs)
            {
                _Go(subDir, level + 1);
                if (_stopped)
                    return;
            }

            // finally, exiting the method is equivalent to exiting the directory, trigger 'exit' event here
            // except in the following special cases...
            //   1 of 1 - cause 'recursive delete' to preclude the root directory from being deleted (see RecursiveDelete optional constructor parameter 'precludeRootDirectory')
            if (dir == Root && Options.AutoSkipRootDirectoryExited)
            {
                Statistics.DirectoriesCrawled--;
                Statistics.DirectoriesSkipped++;
                var curDirectoryMetadataSk = new DirectoryMetadata<T>(curDirMetadata) { SkipReason = SkipReason.AutoSkipped };
                DirectoryCrawled?.Invoke(DirectoryCrawlEvent.DirectorySkipped, dir, curDirectoryMetadataSk);
            }
            else
            {
                DirectoryCrawled?.Invoke(DirectoryCrawlEvent.DirectoryExited, dir, curDirMetadata);
            }
        }

        // updates 'skipped' statistics for the supplied dir including all its files and subfolders
        private void _bulkSkipDir(DirectoryPath dir, DirectoryMetadata<T> curDirMetadata, SkipReason reason)
        {
            var curDirectoryMetadataSk = new DirectoryMetadata<T>(curDirMetadata) { SkipReason = reason };
            DirectoryCrawled?.Invoke(DirectoryCrawlEvent.DirectorySkipped, dir, curDirectoryMetadataSk);

            // updates 'skipped' statistics for the dir including all its files and subfolders
            new DirectoryCrawler
            (
                dir,
                directoryCrawled: (@event, _dir, metadata) =>
                {
                    switch (@event)
                    {
                        case DirectoryCrawlEvent.DirectoryEntered:
                            Statistics.DirectoriesSkipped++;
                            break;
                    }
                },
                fileCrawled: (@event, _dir, metadata) =>
                {
                    switch (@event)
                    {
                        case FileCrawlEvent.FileFound:
                            Statistics.FilesSkipped++;
                            break;
                    }
                }
            ).Go();
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
