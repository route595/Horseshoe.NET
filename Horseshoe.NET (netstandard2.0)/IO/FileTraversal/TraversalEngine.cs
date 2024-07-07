using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json.Serialization;

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
        /// A snapshot of what occurred during the file traversal session
        /// </summary>
        public TraversalStatistics Statistics { get; }

        /// <summary>
        /// File and directory preferences and filters
        /// </summary>
        public virtual TraversalOptimizations Optimizations { get; }

        /// <summary>
        /// EvenT to trigger on directory discovery
        /// </summary>
        public virtual Action<DirectoryPath, TraversalDirectoryIntercom> OnDirectoryHello { get; set; }

        /// <summary>
        /// Event to trigger on directory exit
        /// </summary>
        public virtual Action<DirectoryPath> OnDirectoryGoodbye { get; set; }

        /// <summary>
        /// Event to trigger when a directory is being skipped
        /// </summary>
        public virtual Action<DirectoryPath> OnDirectorySkipping { get; set; }


        /// <summary>
        /// Event to trigger on skipped directory exit
        /// </summary>
        public virtual Action<DirectoryPath> OnDirectorySkipped { get; set; }

        /// <summary>
        /// Event to trigger when a directory is being deleted
        /// </summary>
        public virtual Action<DirectoryPath> OnDirectoryDeleting { get; set; }

        /// <summary>
        /// Event to trigger on deleted directory exit
        /// </summary>
        public virtual Action<DirectoryPath> OnDirectoryDeleted { get; set; }

        /// <summary>
        /// Event to trigger on file discovery
        /// </summary>
        public virtual Action<FilePath, TraversalFileIntercom> OnFileHello { get; set; }

        /// <summary>
        /// Event to trigger when a file is being skipped
        /// </summary>
        public virtual Action<FilePath> OnFileSkipped { get; set; }

        /// <summary>
        /// Event to trigger after a file has been deleted
        /// </summary>
        public virtual Action<FilePath, long> OnFileDeleted { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual AdvancedAction AdvancedAction { get; set; }

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
            AdvancedAction.CurrentTraversalEngine = this;

            try
            {
                if (Root.Parent.HasValue)
                {
                    var rootSiblingNames = GetSubdirectories(Root.Parent.Value, Optimizations, out bool filtered)
                        .Select(d => d.Name);
                    if (filtered && !Root.Name.In(rootSiblingNames))
                    {
                        Iterate(Root, true, AdvancedAction);
                        return;
                    }
                }
            }
            catch { }
            Iterate(Root, false, AdvancedAction);
        }

        private void Iterate(DirectoryPath dirPath, bool skipDir, AdvancedAction advancedAction)
        {
            if (skipDir)
            {
                OnDirectorySkipping?.Invoke(dirPath);
                Log(dirPath, action: TraversalStatistics.Skipping);
            }
            else if (advancedAction != null)
            {
                if (advancedAction.AutoLoadActionRoot && AdvancedAction.CurrentRoot == null)
                    AdvancedAction.SetCurrentRoot(dirPath);
                if (advancedAction.DirectoryHelloAction != null)
                    advancedAction.DirectoryHelloAction.Invoke(dirPath);
                else
                    OnDirectoryHello?.Invoke(dirPath, null);
                if (advancedAction.DirectoryHelloLogAction != null)
                    advancedAction.DirectoryHelloLogAction.Invoke(dirPath);
                else
                    Log(dirPath);
            }
            else
            {
                OnDirectoryHello?.Invoke(dirPath, TraversalDirectoryIntercom.Instance.Reset());
                if (TraversalDirectoryIntercom.Instance.SkipRequested)
                {
                    OnDirectorySkipping?.Invoke(dirPath);
                    Log(dirPath, action: TraversalStatistics.Skipping);
                    skipDir = true;
                }
                else if (TraversalDirectoryIntercom.Instance.DeleteRequested)
                {
                    advancedAction = AdvancedAction.Delete(dryRun: DryRun);
                    Log(dirPath, action: TraversalStatistics.Deleting);
                }
                else if (TraversalDirectoryIntercom.Instance.DeleteContentsRequested)
                {
                    advancedAction = AdvancedAction.DeleteContents(dirPath, dryRun: DryRun);
                    Log(dirPath, action: TraversalStatistics.DeletingContents);
                }
                else
                {
                    Log(dirPath);
                }
            }

            // Files
            if (!Optimizations.ScanDirectoriesOnly && !skipDir)
            {
                foreach (var file in GetFiles(dirPath, Optimizations))
                {
                    var fileSize = file.Size;
                    if (advancedAction != null)
                    {
                        if (advancedAction.FileHelloAction != null)
                            advancedAction.FileHelloAction.Invoke(file);
                        else
                            OnFileHello?.Invoke(file, null);
                        if (advancedAction.FileHelloLogAction != null)
                            advancedAction.FileHelloLogAction?.Invoke(file);
                        else
                            Log(file, action: "Hello-" + advancedAction.Name);
                    }
                    else
                    {
                        OnFileHello?.Invoke(file, TraversalFileIntercom.Instance.Reset(dryRun: DryRun));
                        if (TraversalFileIntercom.Instance.SkipRequested)
                        {
                            OnFileSkipped?.Invoke(file);
                            Log(file, action: TraversalStatistics.Skipped);
                        }
                        else if (TraversalFileIntercom.Instance.DeleteRequested)
                        {
                            Delete(file);
                            Log(file, action: TraversalStatistics.Deleted);
                        }
                        else
                        {
                            Log(file);
                        }
                    }
                }
            }

            // Subdirectories
            var dirNames = GetSubdirectories(dirPath, Optimizations, out bool filtered)
                .Select(d => d.Name);
            foreach (var dir in dirPath.GetDirectories())
            {
                if (filtered && !dir.Name.In(dirNames))
                {
                    Iterate(dir, true, advancedAction);
                }
                else
                {
                    Iterate(dir, false, advancedAction);
                }
            }

            // goodbye
            if (skipDir)
            {
                OnDirectorySkipped?.Invoke(dirPath);
                Statistics.UpdateAction(dirPath, TraversalStatistics.Skipped);
            }
            else
            {
                if (advancedAction != null)
                {
                    if (advancedAction.DirectoryGoodbyeAction != null)
                    {
                        advancedAction.DirectoryGoodbyeAction.Invoke(dirPath);
                    }
                    advancedAction.DirectoryGoodbyeLogAction?.Invoke(dirPath);
                }

                OnDirectoryGoodbye?.Invoke(dirPath);

                if (advancedAction != null && advancedAction.AutoLoadActionRoot && AdvancedAction.CurrentRoot.HasValue && AdvancedAction.CurrentRoot == dirPath)
                    AdvancedAction.SetCurrentRoot(null);
            }
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

        public void Delete(FilePath file, bool dryRun = false, string logAction = TraversalStatistics.NoLogging)
        {
            var fileSize = file.Size;
            if (!(DryRun || dryRun))
                file.Delete();
            OnFileDeleted?.Invoke(file, fileSize);
            switch (logAction)
            {
                case TraversalStatistics.LogFile:
                    Log(file, fileSize: fileSize, action: TraversalStatistics.Deleted);
                    break;
                case TraversalStatistics.UpdateLogActionFile:
                    Statistics.UpdateAction(file, TraversalStatistics.Deleted);
                    break;
            }
        }

        public void Deleting(DirectoryPath directory, string logAction = TraversalStatistics.NoLogging)
        {
            OnDirectoryDeleting?.Invoke(directory);
            switch (logAction)
            {
                case TraversalStatistics.LogDirectory:
                    Log(directory, action: TraversalStatistics.Deleting);
                    break;
                case TraversalStatistics.UpdateLogActionDirectory:
                    Statistics.UpdateAction(directory, TraversalStatistics.Deleting);
                    break;
            }
        }

        public void Delete(DirectoryPath directory, bool dryRun = false, string logAction = TraversalStatistics.NoLogging)
        {
            if (!(DryRun || dryRun))
                directory.Delete();
            OnDirectoryDeleted?.Invoke(directory);
            switch (logAction)
            {
                case TraversalStatistics.LogDirectory:
                    Log(directory, action: TraversalStatistics.Deleted);
                    break;
                case TraversalStatistics.UpdateLogActionDirectory:
                    Statistics.UpdateAction(directory, TraversalStatistics.Deleted);
                    break;
            }
        }

        public void Log(FilePath file, long? fileSize = null, string action = TraversalStatistics.Hello)
        {
            Statistics.Log(file, Root, fileSize: fileSize, action: action);
        }

        public void Log(DirectoryPath directory, string action = TraversalStatistics.Hello)
        {
            Statistics.Log(directory, Root, action: action);
        }
    }
}
