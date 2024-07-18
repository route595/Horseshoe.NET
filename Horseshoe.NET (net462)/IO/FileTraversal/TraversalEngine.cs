using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Horseshoe.NET.IO.FileTraversal
{
    /// <summary>
    /// A generic file / directory crawler with a simple, event-driven paradigm enabling a wide array 
    /// of custom file and directory oriented tasks out of the box.
    /// </summary>
    public class TraversalEngine
    {
        /// <summary>
        /// The starting directory.  Traversal will not occur outside of this directory.
        /// </summary>
        public DirectoryPath Root { get; }

        /// <summary>
        /// A hint suggesting whether or not to perform a file write or delete, useful during development and prototyping
        /// </summary>
        public bool DryRun { get; set; }

        /// <summary>
        /// Attempts to reflect at the instance level the real-time state of the internal recursive delete flag 
        /// </summary>
        public bool RecursiveDeleteMode { get; private set; }

        /// <summary>
        /// A snapshot of what occurs during a file traversal session
        /// </summary>
        public TraversalStatistics Statistics { get; }

        /// <summary>
        /// File and directory preferences and filters
        /// </summary>
        public virtual TraversalOptimizations Optimizations { get; }

        /// <inheritdoc cref="TraversalOptimizations.HasDirectoryFilter"/>
        public bool HasDirectoryFilter => Optimizations.HasDirectoryFilter;

        /// <inheritdoc cref="TraversalOptimizations.HasFileFilter"/>
        public bool HasFileFilter => Optimizations.HasFileFilter;

        /// <summary>
        /// Event to trigger on directory discovery
        /// </summary>
        public virtual Action<DirectoryPath, TraversalEngine, TraversalDirectoryIntercom> OnDirectoryHello { get; set; }

        /// <summary>
        /// Event to trigger on directory filter match
        /// </summary>
        public virtual Action<DirectoryPath, TraversalEngine, TraversalDirectoryIntercom> OnDirectoryFilterMatch { get; set; }

        /// <summary>
        /// Event to trigger on directory exit
        /// </summary>
        public virtual Action<DirectoryPath, TraversalEngine, TraversalDirectoryIntercom> OnDirectoryGoodbye { get; set; }

        /// <summary>
        /// Event to trigger on directory being skipped
        /// </summary>
        public virtual Action<DirectoryPath, TraversalEngine> OnDirectorySkipped { get; set; }

        /// <summary>
        /// Event to trigger when a directory is being deleted
        /// </summary>
        public virtual Action<DirectoryPath, TraversalEngine> OnDirectoryDeleting { get; set; }

        /// <summary>
        /// Event to trigger on directory deletion
        /// </summary>
        public virtual Action<DirectoryPath, TraversalEngine> OnDirectoryDeleted { get; set; }

        /// <summary>
        /// Event to trigger on file discovery
        /// </summary>
        public virtual Action<FilePath, TraversalEngine, TraversalFileIntercom> OnFileHello { get; set; }

        /// <summary>
        /// Event to trigger when a file is being skipped
        /// </summary>
        public virtual Action<FilePath, TraversalEngine> OnFileSkip { get; set; }

        /// <summary>
        /// Event to trigger after a file has been deleted
        /// </summary>
        public virtual Action<FilePath, TraversalEngine, long> OnFileDelete { get; set; }

        /// <summary>
        /// Event to trigger when a warning condition occurs
        /// </summary>
        public virtual Action<string> OnWarning { get; set; }

        private DirectoryPath? doNotDelete;

        public TraversalEngine(DirectoryPath root, TraversalStatistics statistics = null, TraversalOptimizations optimizations = null)
        {
            Root = root.FullName.Last() == Path.DirectorySeparatorChar
                ? root.FullName.Substring(0, root.FullName.Length - 1)
                : root.FullName;
            Statistics = statistics ?? new TraversalStatistics();
            Optimizations = optimizations ?? new TraversalOptimizations();
        }

        /// <summary>
        /// Starts the file traversal engine
        /// </summary>
        public void Start()
        {
            Start(false);
        }


        /// <summary>
        /// Starts the file traversal engine in recursive delete mode
        /// </summary>
        public void StartRecursiveDelete()
        {
            Start(true);
        }

        /// <summary>
        /// Starts the file traversal engine
        /// </summary>
        private void Start(bool recursiveDelete)
        {
            bool filterMatch = false;
            if (Optimizations.DirectoryFilter != null && Optimizations.DirectoryFilter.Invoke(Root))
            {
                filterMatch = true;
            }
            if (Optimizations.DirectorySearchPattern != null && Regex.IsMatch(Root, Optimizations.DirectorySearchPattern.Replace(".", "\\.").Replace("*", ".*")))
            {
                filterMatch = false;
            }
            Iterate(Root, filterMatch, recursiveDelete);
        }

        private void Iterate(DirectoryPath dirPath, bool filterMatch, bool recursiveDelete)
        {
            OnDirectoryHello?.Invoke(dirPath, this, TraversalDirectoryIntercom.Instance.Reset());
            Statistics.LogHello(dirPath, Root);
            bool skipDir = false;
            if (recursiveDelete)
            {
                if (TraversalDirectoryIntercom.Instance.Skipped)
                    OnWarning?.Invoke("Skip denied in recursive delete mode");
                if (TraversalDirectoryIntercom.Instance.DeleteRequested)
                    OnWarning?.Invoke("Delete request is redundant in recursive delete mode");
                OnDirectoryDeleting?.Invoke(dirPath, this);
                Statistics.UpdateAction(dirPath, Root, action: TraversalConstants.Deleting);
            }
            else
            {
                if (filterMatch)
                {
                    OnDirectoryFilterMatch?.Invoke(dirPath, this, TraversalDirectoryIntercom.Instance);
                    if (TraversalDirectoryIntercom.Instance.DeleteRequested)
                    {
                        RecursiveDeleteMode = recursiveDelete = true;
                        if (TraversalDirectoryIntercom.Instance.DeleteContents)
                            doNotDelete = dirPath;
                        OnDirectoryDeleting?.Invoke(dirPath, this);
                        Statistics.UpdateAction(dirPath, Root, action: TraversalConstants.Deleting);
                    }
                    else if (TraversalDirectoryIntercom.Instance.Skipped)
                    {
                        OnDirectorySkipped?.Invoke(dirPath, this);
                        Statistics.UpdateAction(dirPath, Root, TraversalConstants.Skipped);
                        skipDir = true;
                    }
                }
                else if (HasDirectoryFilter)
                {
                    OnDirectorySkipped?.Invoke(dirPath, this);
                    Statistics.UpdateAction(dirPath, Root, TraversalConstants.Skipped);
                    skipDir = true;
                }
            }
            TraversalDirectoryIntercom.Instance.Reset();

            // Files
            if ((!Optimizations.DirectoriesOnlyMode && !skipDir) || recursiveDelete)
            {
                foreach (var file in GetFiles(dirPath, Optimizations, recursiveDelete))
                {
                    Statistics.LogHello(file, Root);
                    var fileSize = file.Size;
                    OnFileHello?.Invoke(file, this, TraversalFileIntercom.Instance.Reset(dryRun: DryRun));
                    if (recursiveDelete)
                    {
                        if (TraversalFileIntercom.Instance.Skipped)
                            OnWarning?.Invoke("Skip denied in recursive delete mode");
                        if (TraversalFileIntercom.Instance.DeleteRequested)
                            OnWarning?.Invoke("Delete request is redundant in recursive delete mode");
                        if (!DryRun)
                            file.Delete();
                        OnFileDelete?.Invoke(file, this, fileSize);
                        Statistics.UpdateAction(file, Root, TraversalConstants.Deleted);
                    }
                    else if (TraversalFileIntercom.Instance.Skipped)
                    {
                        OnFileSkip?.Invoke(file, this);
                        Statistics.UpdateAction(file, Root, TraversalConstants.Skipped);
                    }
                    else if (TraversalFileIntercom.Instance.DeleteRequested)
                    {
                        if (!DryRun && !TraversalFileIntercom.Instance.DryRun)
                            file.Delete();
                        OnFileDelete?.Invoke(file, this, fileSize);
                        Statistics.UpdateAction(file, Root, TraversalConstants.Deleted);
                    }
                    else if (TraversalFileIntercom.Instance.ActionName != null)
                    {
                        Statistics.UpdateAction(file, Root, TraversalFileIntercom.Instance.ActionName);
                    }
                }
            }

            // Subdirectories
            var dirNames = GetSubdirectories(dirPath, Optimizations, out bool filtered)
                .Select(d => d.Name);
            foreach (var dir in dirPath.GetDirectories())
            {
                Iterate(dir, !filtered || dir.Name.In(dirNames), recursiveDelete);
            }

            // goodbye
            OnDirectoryGoodbye?.Invoke(dirPath, this, TraversalDirectoryIntercom.Instance.Reset());

            if (skipDir)
                return;

            if (recursiveDelete)
            {
                if (doNotDelete.HasValue && doNotDelete.Value == dirPath)
                {
                    doNotDelete = null;
                    Statistics.UpdateAction(dirPath, Root, TraversalConstants.ContentsDeleted);
                }
                else
                {
                    if (!DryRun)
                        dirPath.Delete();
                    OnDirectoryDeleted?.Invoke(dirPath, this);
                    Statistics.UpdateAction(dirPath, Root, TraversalConstants.Deleted);
                }
            }
            else if (TraversalDirectoryIntercom.Instance.Skipped)
            {
                OnWarning?.Invoke("Skip request is denied in goodbye stage");
            }
            else if (TraversalDirectoryIntercom.Instance.DeleteRequested)
            {
                if (TraversalDirectoryIntercom.Instance.Skipped)
                    OnWarning?.Invoke("Skip request is denied in delete mode");
                if (TraversalDirectoryIntercom.Instance.DeleteContents)
                    OnWarning?.Invoke("Delete contents is not honored in goodbye stage");

                if (dirPath.Exists)
                {
                    if (!DryRun)
                        dirPath.Delete();
                    Statistics.UpdateAction(dirPath, Root, TraversalConstants.Deleted);
                    OnDirectoryDeleted?.Invoke(dirPath, this);
                }
                else
                {
                    OnWarning?.Invoke("This directory has already been deleted: " + FileUtilAbstractions.DisplayAsVirtualPathFromRoot( dirPath, Root));
                }
            }
            RecursiveDeleteMode = recursiveDelete;  // for when recursiveDelete naturally ends after the last iteration, if applicable
        }

        private static IEnumerable<FilePath> GetFiles(DirectoryPath dirPath, TraversalOptimizations optimizations, bool recursiveDelete)
        {
            IEnumerable<FilePath> files = optimizations.FileSearchPattern != null && !recursiveDelete
                ? dirPath.GetFiles(optimizations.FileSearchPattern)
                : dirPath.GetFiles();

            if (optimizations.FileFilter != null && !recursiveDelete)
            {
                files = files.Where(optimizations.FileFilter);
            }

            return files;
        }

        private static IEnumerable<DirectoryPath> GetSubdirectories(DirectoryPath dirPath, TraversalOptimizations optimizations, out bool filtered)
        {
            if (optimizations.DirectorySearchPattern == null && optimizations.DirectoryFilter == null)
            {
                filtered = false;
                return Enumerable.Empty<DirectoryPath>();
            }

            filtered = true;
            IEnumerable<DirectoryPath> dirs;

            if (optimizations.DirectorySearchPattern != null)
            {
                dirs = dirPath.GetDirectories(optimizations.DirectorySearchPattern);
            }
            else
            {
                dirs = dirPath.GetDirectories();
            }

            if (optimizations.DirectoryFilter != null)
            {
                dirs = dirs.Where(optimizations.DirectoryFilter);
            }

            return dirs;
        }

        //public void Delete(FilePath file, bool dryRun = false, string logAction = TraversalStatistics.NoLogging)
        //{
        //    var fileSize = file.Size;
        //    if (!(DryRun || dryRun))
        //        file.Delete();
        //    OnFileDelete?.Invoke(file, fileSize);
        //    switch (logAction)
        //    {
        //        case TraversalStatistics.LogFile:
        //            Log(file, fileSize: fileSize, action: TraversalStatistics.Deleted);
        //            break;
        //        case TraversalStatistics.UpdateLogActionFile:
        //            Statistics.UpdateAction(file, TraversalStatistics.Deleted);
        //            break;
        //    }
        //}

        //public void Deleting(DirectoryPath directory, string logAction = TraversalStatistics.NoLogging)
        //{
        //    OnDirectoryDeleting?.Invoke(directory);
        //    switch (logAction)
        //    {
        //        case TraversalStatistics.LogDirectory:
        //            Log(directory, action: TraversalStatistics.Deleting);
        //            break;
        //        case TraversalStatistics.UpdateLogActionDirectory:
        //            Statistics.UpdateAction(directory, TraversalStatistics.Deleting);
        //            break;
        //    }
        //}

        //public void Delete(DirectoryPath directory, bool dryRun = false, string logAction = TraversalStatistics.NoLogging)
        //{
        //    if (!(DryRun || dryRun))
        //        directory.Delete();
        //    OnDirectoryDeleted?.Invoke(directory);
        //    switch (logAction)
        //    {
        //        case TraversalStatistics.LogDirectory:
        //            Log(directory, action: TraversalStatistics.Deleted);
        //            break;
        //        case TraversalStatistics.UpdateLogActionDirectory:
        //            Statistics.UpdateAction(directory, Root, TraversalStatistics.Deleted);
        //            break;
        //    }
        //}

        //private void Log(FilePath file, long? fileSize = null, string action = TraversalStatistics.Hello)
        //{
        //    Statistics.Log(file, Root, fileSize: file.Size, action: TraversalStatistics.Hello);
        //}

        //public void Log(FilePath file, long? fileSize = null, string action = TraversalStatistics.Hello)
        //{
        //    Statistics.Log(file, Root, fileSize: fileSize, action: action);
        //}

        //public void Log(DirectoryPath directory, string action = TraversalStatistics.Hello)
        //{
        //    Statistics.Log(directory, Root, action: action);
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="root">The starting directory.  Traversal will not occur outside of this directory.</param>
        /// <param name="directoryNames">The names of directories you wish to delete</param>
        /// <param name="deleteContents">If <c>true</c>, deletes the contents of the directory while leaving the directory intact.  Default is <c>false</c>.</param>
        /// <param name="dryRun">A hint suggesting whether or not to perform a file write or delete, useful during development and prototyping</param>
        /// <param name="statistics">A snapshot of what occurs during a file traversal session</param>
        /// <param name="onDirectoryHello">Event to trigger on directory discovery</param>
        /// <param name="onDirectoryFilterMatch">Event to trigger on directory filter match</param>
        /// <param name="onDirectoryGoodbye">Event to trigger on directory exit</param>
        /// <param name="onDirectorySkipped">Event to trigger on directory being skipped</param>
        /// <param name="onDirectoryDeleting">Event to trigger when a directory is being deleted</param>
        /// <param name="onDirectoryDeleted">Event to trigger on directory deletion</param>
        /// <param name="onFileHello">Event to trigger on file discovery</param>
        /// <param name="onFileDelete">Event to trigger after a file has been deleted</param>
        /// <param name="onWarning">Event to trigger when a warning condition occurs</param>
        /// <param name="ignoreCase">If <c>true</c>, matches directories whose names would match if not for the letter case, default is <c>false</c>.</param>
        /// <returns>A traversal engine configured for hunt and delete directories.</returns>
        public static TraversalEngine BuildDirectoryHunterAndDeleter
        (
            DirectoryPath root,
            string[] directoryNames,
            bool deleteContents = false,
            bool dryRun = false,
            TraversalStatistics statistics = null,
            Action<DirectoryPath, TraversalEngine, TraversalDirectoryIntercom> onDirectoryHello = null,
            Action<DirectoryPath, TraversalEngine, TraversalDirectoryIntercom> onDirectoryFilterMatch = null,
            Action<DirectoryPath, TraversalEngine, TraversalDirectoryIntercom> onDirectoryGoodbye = null,
            Action<DirectoryPath, TraversalEngine> onDirectorySkipped = null,
            Action<DirectoryPath, TraversalEngine> onDirectoryDeleting = null,
            Action<DirectoryPath, TraversalEngine> onDirectoryDeleted = null,
            Action<FilePath, TraversalEngine, TraversalFileIntercom> onFileHello = null,
            Action<FilePath, TraversalEngine, long> onFileDelete = null,
            Action<string> onWarning = null,
            bool ignoreCase = false
        )
        {
            var optimizations = new TraversalOptimizations
            {
                DirectoriesOnlyMode = true,
                DirectoryFilter = dp => ignoreCase ? dp.Name.InIgnoreCase(directoryNames) : dp.Name.In(directoryNames)
            };
            return new TraversalEngine(root, statistics: statistics, optimizations: optimizations)
            {
                OnDirectoryHello = onDirectoryHello,
                OnDirectoryFilterMatch = (dp, eng, icom) =>
                {
                    icom.RequestDelete(deleteContents: deleteContents);
                    onDirectoryFilterMatch?.Invoke(dp, eng, icom);
                },
                OnDirectoryGoodbye = onDirectoryGoodbye,
                OnDirectorySkipped = onDirectorySkipped,
                OnDirectoryDeleting = onDirectoryDeleting,
                OnDirectoryDeleted = onDirectoryDeleted,
                OnFileHello = onFileHello,
                OnFileDelete = onFileDelete,
                OnWarning = onWarning,
                DryRun = dryRun
            };
        }
    }
}
