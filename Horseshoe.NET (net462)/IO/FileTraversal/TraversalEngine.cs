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
        /// A snapshot of what occurred during the file traversal session
        /// </summary>
        public TraversalStatistics Statistics { get; }

        /// <summary>
        /// File and directory preferences and filters
        /// </summary>
        public virtual TraversalOptimizations Optimizations { get; }

        /// <summary>
        /// Event to trigger on directory discovery
        /// </summary>
        public virtual Action<DirectoryPath, TraversalEngine, TraversalDirectoryIntercom> OnDirectoryHello { get; set; }

        /// <summary>
        /// Event to trigger on directory exit
        /// </summary>
        public virtual Action<DirectoryPath, TraversalEngine, TraversalDirectoryIntercom> OnDirectoryGoodbye { get; set; }

        /// <summary>
        /// Event to trigger on skipped directory exit
        /// </summary>
        public virtual Action<DirectoryPath, TraversalEngine> OnDirectorySkipped { get; set; }

        /// <summary>
        /// Event to trigger when a directory is being deleted
        /// </summary>
        public virtual Action<DirectoryPath, TraversalEngine> OnDirectoryDeleting { get; set; }

        /// <summary>
        /// Event to trigger on deleted directory exit
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

        ///// <summary>
        ///// Event to trigger for files that should be processed, if applicable
        ///// </summary>
        //public virtual Action<FilePath, TraversalEngine, TraversalFileIntercom> OnFileProcess { get; set; }

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

        public void Start()
        {
            //AdvancedAction.CurrentTraversalEngine = this;
            bool skipRoot = (Optimizations.DirectoryFilter != null && !Optimizations.DirectoryFilter.Invoke(Root)) || (Optimizations.DirectorySearchPattern != null && !Regex.IsMatch(Root, Optimizations.DirectorySearchPattern.Replace(".", "\\.").Replace("*", ".*")));
            Iterate(Root, skipRoot, false);
        }

        private void Iterate(DirectoryPath dirPath, bool skipDir, bool recursiveDelete)
        {
            OnDirectoryHello?.Invoke(dirPath, this, TraversalDirectoryIntercom.Instance.Reset());
            Statistics.LogHello(dirPath, Root);
            if (skipDir && !recursiveDelete)
            {
                OnDirectorySkipped?.Invoke(dirPath, this);
                Statistics.UpdateAction(dirPath, Root, TraversalConstants.Skipped);
            }
            else
            {
                if (recursiveDelete)
                {
                    if (TraversalDirectoryIntercom.Instance.Skipped)
                        OnWarning?.Invoke("Skip denied in recursive delete mode");
                    if (TraversalDirectoryIntercom.Instance.DeleteRequested)
                        OnWarning?.Invoke("Delete request is redundant in recursive delete mode");
                }
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

                // Files
                if ((!Optimizations.DirectoriesOnlyMode && !skipDir) || recursiveDelete)
                {
                    foreach (var file in GetFiles(dirPath, Optimizations))
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
            }

            // Subdirectories
            var dirNames = GetSubdirectories(dirPath, Optimizations, out bool filtered)
                .Select(d => d.Name);
            foreach (var dir in dirPath.GetDirectories())
            {
                Iterate(dir, filtered && !dir.Name.In(dirNames), recursiveDelete);
            }

            // goodbye
            OnDirectoryGoodbye?.Invoke(dirPath, this, TraversalDirectoryIntercom.Instance.Reset());

            if (skipDir && !recursiveDelete)
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
            RecursiveDeleteMode = recursiveDelete;
        }

        private static IEnumerable<FilePath> GetFiles(DirectoryPath dirPath, TraversalOptimizations optimizations)
        {
            IEnumerable<FilePath> files = optimizations.FileSearchPattern != null
                ? dirPath.GetFiles(optimizations.FileSearchPattern)
                : dirPath.GetFiles();

            if (optimizations.FileFilter != null)
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
    }
}
