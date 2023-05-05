using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Horseshoe.NET.Collections;
using Horseshoe.NET.Text;

namespace Horseshoe.NET.IO.FileTraversal
{
    /// <summary>
    /// A generic file and directory iterator with a simple, event-driven paradigm enabling a wide array 
    /// of custom file and directory oriented tasks out of the box.
    /// </summary>
    public class TraversalEngine
    {
    //    /// <summary>
    //    /// The starting directory.  Traversal will not occur outside of this directory.
    //    /// </summary>
    //    public DirectoryPath Root { get; }

    //    /// <summary>
    //    /// File and directory filters for when not every file and directory must be traversed. 
    //    /// </summary>
    //    public virtual TraversalOptimization Optimization { get; set; }

    //    /// <summary>
    //    /// A storage area for values collected by the engine. Client can also store values here.
    //    /// </summary>
    //    public IDictionary<string, object> Statistics { get; }

    //    /// <summary>
    //    /// A storage area for file and directory errors.
    //    /// </summary>
    //    public IList<TraversalError> TraversalErrors { get; }

    //    /// <summary>
    //    /// If <c>true</c>, certain operations like 'delete' will not execute although all events will still trigger.
    //    /// </summary>
    //    public bool DryRun { get; private set; }

    //    /// <summary>
    //    /// If <c>true</c>, allows traversal related errors to accumulate. Otherwise, the traveral engine should halt at the first exception.  
    //    /// Default is <c>false</c>. 
    //    /// </summary>
    //    /// <seealso cref="TraversalErrors"/>
    //    /// <seealso cref="DirectoryErrored"/>
    //    /// <seealso cref="FileErrored"/>
    //    public bool AccumulateErrors { get; set; }

    //    /// <summary>
    //    /// Settings and data including client-settable <c>Action</c> that instructs
    //    /// the traversal engine to perform specific directory-related tasks in realtime.
    //    /// </summary>
    //    public DirectoryTraversalMetadata DirectoryMetadata { get; }

    //    /// <summary>
    //    /// A temporary snapshot of the last traversed file.  Contains a client-settable <c>Action</c> that instructs
    //    /// the traversal engine to perform specific directory-related tasks in realtime.
    //    /// </summary>
    //    /// <remarks>
    //    /// Retaining the file's size, for example, can be useful given that once deleted the <c>FilePath</c> object may 
    //    /// continue to be viable, however, accessing the file's size is not feasible.
    //    /// </remarks>
    //    public FileTraversalMetadata FileMetadata { get; }

    //    /// <summary>
    //    /// Event to listen for engine ignition.
    //    /// </summary>
    //    public virtual Action<TraversalEngine> Begin { get; set; }

    //    /// <summary>
    //    /// Event to listen for engine termination.
    //    /// </summary>
    //    public virtual Action<TraversalEngine> End { get; set; }

    //    /// <summary>
    //    /// Event to listen for each time a directory is entered.
    //    /// </summary>
    //    public virtual Action<DirectoryPath, TraversalEngine> DirectoryEntered { get; }

    //    /// <summary>
    //    /// Event to listen for each time a directory is filtered out during taversal or manually skipped.
    //    /// </summary>
    //    public virtual Action<DirectoryPath, TraversalEngine> DirectorySkipped { get; }

    //    /// <summary>
    //    /// Event to listen for each time a directory is being recursively deleted.
    //    /// </summary>
    //    public virtual Action<DirectoryPath, TraversalEngine> RecursiveDeletingDirectory { get; }

    //    /// <summary>
    //    /// Event to listen for each time a directory is being deleted.
    //    /// </summary>
    //    public virtual Action<DirectoryPath, TraversalEngine> DeletingDirectory { get; }

    //    /// <summary>
    //    /// Event to listen for each time a directory is deleted.
    //    /// </summary>
    //    public virtual Action<DirectoryPath, TraversalEngine> DirectoryDeleted { get; }

    //    /// <summary>
    //    /// Event to listen for each time a directory errors, e.g. was not able to be deleted.
    //    /// </summary>
    //    public virtual Action<DirectoryPath, TraversalEngine> DirectoryErrored { get; }

    //    /// <summary>
    //    /// Event to listen for each time a directory is exited.
    //    /// </summary>
    //    public virtual Action<DirectoryPath, TraversalEngine> DirectoryExited { get; }

    //    /// <summary>
    //    /// Event to listen for each time a file is browsed.
    //    /// </summary>
    //    public virtual Action<FilePath, TraversalEngine> FileFound { get; }

    //    /// <summary>
    //    /// Event to listen for each time a file is about to be deleted.
    //    /// </summary>
    //    public virtual Action<FilePath, TraversalEngine> DeletingFile { get; }

    //    /// <summary>
    //    /// Event to listen for each time a file is deleted.
    //    /// </summary>
    //    public virtual Action<FilePath, TraversalEngine> FileDeleted { get; }

    //    /// <summary>
    //    /// Event to listen for each time a file errors, e.g. was not able to be deleted.
    //    /// </summary>
    //    public virtual Action<FilePath, TraversalEngine> FileErrored { get; }

    //    /// <summary>
    //    /// Value stored in <c>Statistics</c>.
    //    /// </summary>
    //    public int FilesFound
    //    {
    //        get => Statistics.TryGetValue("traversal-statistics.files-found", out object value) ? (int)value : 0;
    //        set => Statistics.AddOrReplace("traversal-statistics.files-found", value);
    //    }

    //    /// <summary>
    //    /// Value stored in <c>Statistics</c>.
    //    /// </summary>
    //    public long SizeOfFilesFound
    //    {
    //        get => Statistics.TryGetValue("traversal-statistics.size-of-files-found", out object value) ? (long)value : 0;
    //        set => Statistics.AddOrReplace("traversal-statistics.size-of-files-found", value);
    //    }

    //    /// <summary>
    //    /// Value stored in <c>Statistics</c>.
    //    /// </summary>
    //    public int FilesDeleted
    //    {
    //        get => Statistics.TryGetValue("traversal-statistics.files-deleted", out object value) ? (int)value : 0;
    //        set => Statistics.AddOrReplace("traversal-statistics.files-deleted", value);
    //    }

    //    /// <summary>
    //    /// Value stored in <c>Statistics</c>.
    //    /// </summary>
    //    public int FilesUnableToBeDeleted
    //    {
    //        get => Statistics.TryGetValue("traversal-statistics.files-unable-to-be-deleted", out object value) ? (int)value : 0;
    //        set => Statistics.AddOrReplace("traversal-statistics.files-unable-to-be-deleted", value);
    //    }

    //    /// <summary>
    //    /// Value stored in <c>Statistics</c>.
    //    /// </summary>
    //    public long SizeOfFilesDeleted
    //    {
    //        get => Statistics.TryGetValue("traversal-statistics.size-of-files-deleted", out object value) ? (long)value : 0;
    //        set => Statistics.AddOrReplace("traversal-statistics.size-of-files-deleted", value);
    //    }

    //    /// <summary>
    //    /// Value stored in <c>Statistics</c>.
    //    /// </summary>
    //    public int DirectoriesBrowsed
    //    {
    //        get => Statistics.TryGetValue("traversal-statistics.dirs-browsed", out object value) ? (int)value : 0;
    //        set => Statistics.AddOrReplace("traversal-statistics.dirs-browsed", value);
    //    }

    //    /// <summary>
    //    /// Value stored in <c>Statistics</c>.
    //    /// </summary>
    //    public int DirectoriesDeleted
    //    {
    //        get => Statistics.TryGetValue("traversal-statistics.dirs-deleted", out object value) ? (int)value : 0;
    //        set => Statistics.AddOrReplace("traversal-statistics.dirs-deleted", value);
    //    }

    //    /// <summary>
    //    /// Value stored in <c>Statistics</c>.
    //    /// </summary>
    //    public int DirectoriesUnableToBeDeleted
    //    {
    //        get => Statistics.TryGetValue("traversal-statistics.dirs-unable-to-be-deleted", out object value) ? (int)value : 0;
    //        set => Statistics.AddOrReplace("traversal-statistics.dirs-unable-to-be-deleted", value);
    //    }

    //    /// <summary>
    //    /// Creates a new <c>TraversalEngine</c>.
    //    /// </summary>
    //    /// <param name="root">The directory to traverse.</param>
    //    /// <param name="optimization">File and directory filters for when not every file and directory must be traversed.</param>
    //    /// <param name="directoryEntered">An event to hook into.</param>
    //    /// <param name="directorySkipped">An event to hook into.</param>
    //    /// <param name="recursiveDeletingDirectory">An event to hook into.</param>
    //    /// <param name="deletingDirectory">An event to hook into.</param>
    //    /// <param name="directoryDeleted">An event to hook into.</param>
    //    /// <param name="directoryErrored">An event to hook into.</param>
    //    /// <param name="directoryExited">An event to hook into.</param>
    //    /// <param name="fileFound">An event to hook into.</param>
    //    /// <param name="deletingFile">An event to hook into.</param>
    //    /// <param name="fileDeleted">An event to hook into.</param>
    //    /// <param name="fileErrored">An event to hook into.</param>
    //    /// <param name="cumulativeStatistics">An optional storage area for values collected by this and other engines.</param>
    //    public TraversalEngine
    //    (
    //        DirectoryPath root, 
    //        TraversalOptimization optimization = null,
    //        Action<DirectoryPath, TraversalEngine> directoryEntered = null,
    //        Action<DirectoryPath, TraversalEngine> directorySkipped = null,
    //        Action<DirectoryPath, TraversalEngine> recursiveDeletingDirectory = null,
    //        Action<DirectoryPath, TraversalEngine> deletingDirectory = null,
    //        Action<DirectoryPath, TraversalEngine> directoryDeleted = null,
    //        Action<DirectoryPath, TraversalEngine> directoryErrored = null,
    //        Action<DirectoryPath, TraversalEngine> directoryExited = null,
    //        Action<FilePath, TraversalEngine> fileFound = null,
    //        Action<FilePath, TraversalEngine> deletingFile = null,
    //        Action<FilePath, TraversalEngine> fileDeleted = null,
    //        Action<FilePath, TraversalEngine> fileErrored = null,
    //        IDictionary<string, object> cumulativeStatistics = null
    //    )
    //    {
    //        Root = root;
    //        Optimization = optimization ?? new TraversalOptimization();
    //        DirectoryEntered = directoryEntered;
    //        DirectorySkipped = directorySkipped;
    //        RecursiveDeletingDirectory = recursiveDeletingDirectory;
    //        DeletingDirectory = deletingDirectory;
    //        DirectoryDeleted = directoryDeleted;
    //        DirectoryErrored = directoryErrored;
    //        DirectoryExited = directoryExited;
    //        FileFound = fileFound;
    //        DeletingFile = deletingFile;
    //        FileDeleted = fileDeleted;
    //        FileErrored = fileErrored;
    //        Statistics = cumulativeStatistics ?? new Dictionary<string, object>();
    //        TraversalErrors = new List<TraversalError>();
    //        FileMetadata = new FileTraversalMetadata();
    //        DirectoryMetadata = new DirectoryTraversalMetadata();
    //    }

    //    /// <summary>
    //    /// Begins the file and directory traversal.
    //    /// </summary>
    //    public void Start(bool dryRun = false)
    //    {
    //        if (dryRun) DryRun = true;
    //        Begin?.Invoke(this);
    //        Loop(Root);
    //        End?.Invoke(this);
    //    }


    //    private IEnumerable<FileInfo> files;
    //    private void Loop(DirectoryPath dir)
    //    {
    //        var skippingThisDirectory = false;
    //        DirectoryMetadata.Reset();

    //        if (Optimization.DirectoryFilter != null && !Optimization.DirectoryFilter.Invoke(dir))
    //        {
    //            DirectorySkipped?.Invoke(dir, this);
    //            skippingThisDirectory = true;
    //        }

    //        if (!skippingThisDirectory)
    //        {
    //            DirectoryEntered?.Invoke(dir, this);
    //            switch (DirectoryMetadata.Action)
    //            {
    //                case ClientAction.Skip:
    //                    DirectoryMetadata.ClientSkipped = true;
    //                    DirectorySkipped?.Invoke(dir, this);
    //                    skippingThisDirectory = true;
    //                    break;
    //                case ClientAction.RecursiveSkip:
    //                    DirectoryMetadata.ClientSkipped = true;
    //                    DirectoryMetadata.RecursivelySkipped = true;
    //                    DirectorySkipped?.Invoke(dir, this);
    //                    return;
    //                case ClientAction.Empty:
    //                    RecursiveDelete(dir, empty: true);
    //                    return;
    //                case ClientAction.Delete:
    //                    RecursiveDelete(dir);
    //                    return;
    //                default:
    //                    DirectoriesBrowsed++;
    //                    break;
    //            }
    //        }

    //        /* * * * * *
    //         *   Files
    //         * * * * * */
    //        if (!Optimization.DirectoryOnlyMode && !skippingThisDirectory)
    //        {
    //            files = Optimization.FileSearchPattern != null
    //                ? dir.GetFiles(Optimization.FileSearchPattern)
    //                : dir.GetFiles();

    //            if (Optimization.FileFilter != null)
    //            {
    //                files = files.Where(f => Optimization.FileFilter.Invoke(f));
    //            }

    //            foreach (FilePath file in files)
    //            {
    //                FileMetadata.Reset(newFileSize: file.Size);
    //                FilesFound++;
    //                SizeOfFilesFound += FileMetadata.FileSize;
    //                FileFound?.Invoke(file, this);

    //                switch (FileMetadata.Action)
    //                {
    //                    case ClientAction.Skip:
    //                        continue;
    //                    case ClientAction.Delete:
    //                        Delete(file);
    //                        break;
    //                    case ClientAction.RecursiveSkip:  // not valid here - defaulting to 'Browse' functionality
    //                    case ClientAction.Empty:          // not valid here - defaulting to 'Browse' functionality
    //                    case ClientAction.Browse:
    //                    default:
    //                        break;
    //                }
    //            }
    //        }

    //        /* * * * * * * * *
    //         *   Directories
    //         * * * * * * * * */

    //        IEnumerable<DirectoryInfo> dirs = dir.GetDirectories();

    //        foreach (DirectoryPath subDir in dirs)
    //        {
    //            Loop(subDir);
    //        }

    //        if (!skippingThisDirectory)
    //        {
    //            DirectoryExited?.Invoke(dir, this);
    //        }
    //    }

    //    /// <summary>
    //    /// Creates a list of <c>string</c>s representing the data stored in <c>Statistics</c>.
    //    /// </summary>
    //    /// <returns></returns>
    //    public IEnumerable<string> RenderStatisticsToCollection()
    //    {
    //        var list = new List<string>();
    //        if (DirectoriesBrowsed + FilesFound > 0)
    //        {
    //            list.Append("Directories browsed:    " + DirectoriesBrowsed);
    //            list.Append("Files found:            " + FilesFound + " (" + FileUtil.GetDisplayFileSize(SizeOfFilesFound) + ")");
    //        }
    //        if (DirectoriesDeleted + DirectoriesUnableToBeDeleted + FilesDeleted + FilesUnableToBeDeleted > 0)
    //        {
    //            list.Append("Directories deleted:    " + DirectoriesDeleted);
    //            list.Append("Files deleted:          " + FilesDeleted + " (" + FileUtil.GetDisplayFileSize(SizeOfFilesDeleted) + ")");
    //        }
    //        foreach (var kvp in Statistics.Where(_kvp => !_kvp.Key.StartsWith("traversal-statistics.")))
    //            list.Append(TextUtil.Pad(kvp.Key.Last() == ':' ? kvp.Key : kvp.Key + ':', 24) + kvp.Value);
    //        return list;
    //    }

    //    /// <summary>
    //    /// Creates a newline-separated string representing the data stored in <c>Statistics</c>.
    //    /// </summary>
    //    /// <returns></returns>
    //    public string RenderStatistics()
    //    {
    //        return string.Join(Environment.NewLine, RenderStatisticsToCollection());
    //    }
    }
}
