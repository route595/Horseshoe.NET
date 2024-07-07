using System;

namespace Horseshoe.NET.IO.FileTraversal
{
    /// <summary>
    /// A way to perform simple to complex actions on files and directories during the traversal process
    /// </summary>
    public class AdvancedAction
    {
        /// <summary>
        /// The name of the action (reflected in <c>TraversalStatistics.Entry.Action</c>).
        /// </summary>
        public string Name { get; }
        public virtual Action<FilePath> FileHelloAction { get; }
        public virtual Action<FilePath> FileHelloLogAction { get; }
        public virtual Action<DirectoryPath> DirectoryHelloAction { get; }
        public virtual Action<DirectoryPath> DirectoryHelloLogAction { get; }
        public virtual Action<DirectoryPath> DirectoryGoodbyeAction { get; }
        public virtual Action<DirectoryPath> DirectoryGoodbyeLogAction { get; }
        public virtual bool AutoLoadActionRoot { get; }
        public static TraversalEngine CurrentTraversalEngine { get; internal set; }
        public static DirectoryPath? CurrentRoot { get; private set; }
        public static bool CurrentDryRun { get; private set; }
        private static long CurrentFileSize { get; set; }

        public AdvancedAction
        (
            string name
        )
        {
            Name = name ?? throw new ValidationException($"{nameof(name)} cannot be null");
        }

        public AdvancedAction
        (
            string name,
            Action<FilePath> fileHelloAction = null,
            Action<FilePath> fileHelloLogAction = null,
            Action<DirectoryPath> directoryHelloAction = null,
            Action<DirectoryPath> directoryHelloLogAction = null,
            Action<DirectoryPath> directoryGoodbyeAction = null,
            Action<DirectoryPath> directoryGoodbyeLogAction = null,
            bool autoLoadActionRoot = false
        ) : this(name)
        {
            FileHelloAction = fileHelloAction;
            FileHelloLogAction = fileHelloLogAction;
            DirectoryHelloAction = directoryHelloAction;
            DirectoryHelloLogAction = directoryHelloLogAction;
            DirectoryGoodbyeAction = directoryGoodbyeAction;
            DirectoryGoodbyeLogAction = directoryGoodbyeLogAction;
            AutoLoadActionRoot = autoLoadActionRoot;
            if (fileHelloAction == null && directoryHelloAction == null && directoryGoodbyeAction == null)
                throw new ValidationException("at least one non-logging action must be supplied");
        }

        public AdvancedAction SetCurrentRoot(DirectoryPath? root)
        {
            CurrentRoot = root;
            return this;
        }

        public AdvancedAction SetCurrentDryRun(bool dryRun)
        {
            CurrentDryRun = dryRun;
            return this;
        }

        public static AdvancedAction Delete(bool dryRun = false) => new AdvancedAction
        (
            TraversalStatistics.Deleting,
            fileHelloAction: (fp) =>
            {
                CurrentFileSize = fp.Size;
                CurrentTraversalEngine.Delete(fp, dryRun: dryRun);
            },
            fileHelloLogAction: (fp) =>
                CurrentTraversalEngine.Log(fp, fileSize: CurrentFileSize, action: TraversalStatistics.Deleted),
            directoryHelloAction: (dp) =>
                CurrentTraversalEngine.Deleting(dp),
            directoryHelloLogAction: (dp) =>
                CurrentTraversalEngine.Log(dp, action: TraversalStatistics.Deleting),
            directoryGoodbyeAction: (dp) =>
                CurrentTraversalEngine.Delete(dp, dryRun: dryRun),
            directoryGoodbyeLogAction: (dp) =>
                CurrentTraversalEngine.Statistics.UpdateAction(dp, TraversalStatistics.Deleted)
        )
        .SetCurrentDryRun(dryRun);

        public static AdvancedAction DeleteContents(DirectoryPath? directory = null, bool dryRun = false, bool autoLoadActionRoot = false) => new AdvancedAction
        (
            TraversalStatistics.DeletingContents,
            fileHelloAction: (fp) =>
            {
                CurrentFileSize = fp.Size;
                CurrentTraversalEngine.Delete(fp, dryRun: dryRun);
            },
            fileHelloLogAction: (fp) =>
                CurrentTraversalEngine.Log(fp, fileSize: CurrentFileSize, action: TraversalStatistics.Deleted),
            directoryHelloAction: (dp) =>
                CurrentTraversalEngine.Deleting(dp),
            directoryHelloLogAction: (dp) =>
                CurrentTraversalEngine.Log(dp, action: TraversalStatistics.Deleting),
            directoryGoodbyeAction: (dp) =>
            {
                if (dp != CurrentRoot)
                    CurrentTraversalEngine.Delete(dp, dryRun: dryRun);
            },
            directoryGoodbyeLogAction: (dp) =>
            {
                if (dp == CurrentRoot)
                    CurrentTraversalEngine.Statistics.UpdateAction(dp, TraversalStatistics.ContentsDeleted);
                else
                    CurrentTraversalEngine.Statistics.UpdateAction(dp, TraversalStatistics.Deleted);
            },
            autoLoadActionRoot: true
        )
        .SetCurrentRoot(directory)
        .SetCurrentDryRun(dryRun);
    }
}
