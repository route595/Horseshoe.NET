using System.Collections.Generic;

using Horseshoe.NET.Collections;

namespace Horseshoe.NET.IO.DirectoryCrawler
{
    /// <summary>
    /// A dictionary of file and directory statistics, in 64-bit integer format, optionally 
    /// gathered by the traversal engine.  Client code may also add customized statistics.
    /// </summary>
    public class TraversalStatistics : Dictionary<string, long>
    {
        /// <summary>
        /// Reports how many directories were encountered, minus any deleted or skipped.
        /// </summary>
        public int DirectoriesProcessed
        {
            get => TryGetValue("TraversalStatistics.DirectoriesProcessed", out long longValue) ? (int)longValue : 0;
            set => DictionaryUtilAbstractions.AddOrReplace(this, "TraversalStatistics.DirectoriesProcessed", value);
        }

        /// <summary>
        /// Reports how many directories were encountered, minus any deleted or skipped.
        /// </summary>
        public int DirectoriesSkipped
        {
            get => TryGetValue("TraversalStatistics.DirectoriesSkipped", out long longValue) ? (int)longValue : 0;
            set => DictionaryUtilAbstractions.AddOrReplace(this, "TraversalStatistics.DirectoriesSkipped", value);
        }

        /// <summary>
        /// Reports how many directories were deleted.
        /// </summary>
        public int DirectoriesDeleted
        {
            get => TryGetValue("TraversalStatistics.DirectoriesDeleted", out long longValue) ? (int)longValue : 0;
            set => DictionaryUtilAbstractions.AddOrReplace(this, "TraversalStatistics.DirectoriesDeleted", value);
        }

        /// <summary>
        /// Reports how many directories resulted in exceptions (e.g. could not be deleted, etc.).
        /// </summary>
        public int DirectoriesErrored
        {
            get => TryGetValue("TraversalStatistics.DirectoriesErrored", out long longValue) ? (int)longValue : 0;
            set => DictionaryUtilAbstractions.AddOrReplace(this, "TraversalStatistics.DirectoriesErrored", value);
        }

        /// <summary>
        /// Total number of directories in this traversal event.
        /// </summary>
        public int TotalDirectories => DirectoriesProcessed + DirectoriesSkipped + DirectoriesDeleted + DirectoriesErrored;

        /// <summary>
        /// Reports how many files were encountered, not including any that were subsequently deleted or skipped.
        /// </summary>
        public int FilesProcessed
        {
            get => TryGetValue("TraversalStatistics.FilesProcessed", out long longValue) ? (int)longValue : 0;
            set => DictionaryUtilAbstractions.AddOrReplace(this, "TraversalStatistics.FilesProcessed", value);
        }

        /// <summary>
        /// Reports the total size of the files that were processed, not including any that were subsequently deleted or skipped.
        /// </summary>
        public long SizeOfFilesProcessed
        {
            get => TryGetValue("TraversalStatistics.SizeOfFilesProcessed", out long longValue) ? longValue : 0L;
            set => DictionaryUtilAbstractions.AddOrReplace(this, "TraversalStatistics.SizeOfFilesProcessed", value);
        }

        /// <summary>
        /// Reports how many files were skipped.
        /// </summary>
        public int FilesSkipped
        {
            get => TryGetValue("TraversalStatistics.FilesSkipped", out long longValue) ? (int)longValue : 0;
            set => DictionaryUtilAbstractions.AddOrReplace(this, "TraversalStatistics.FilesSkipped", value);
        }

        /// <summary>
        /// Reports the total size of the files that were skipped.
        /// </summary>
        public long SizeOfFilesSkipped
        {
            get => TryGetValue("TraversalStatistics.SizeOfFilesSkipped", out long longValue) ? longValue : 0L;
            set => DictionaryUtilAbstractions.AddOrReplace(this, "TraversalStatistics.SizeOfFilesSkipped", value);
        }

        /// <summary>
        /// Reports how many files were deleted.
        /// </summary>
        public int FilesDeleted
        {
            get => TryGetValue("TraversalStatistics.FilesDeleted", out long longValue) ? (int)longValue : 0;
            set => DictionaryUtilAbstractions.AddOrReplace(this, "TraversalStatistics.FilesDeleted", value);
        }

        /// <summary>
        /// Reports the total size of the files that were deleted.
        /// </summary>
        public long SizeOfFilesDeleted
        {
            get => TryGetValue("TraversalStatistics.SizeOfFilesDeleted", out long longValue) ? longValue : 0L;
            set => DictionaryUtilAbstractions.AddOrReplace(this, "TraversalStatistics.SizeOfFilesDeleted", value);
        }

        /// <summary>
        /// Reports how many files resulted in exceptions.
        /// </summary>
        public int FilesErrored
        {
            get => TryGetValue("TraversalStatistics.FilesErrored", out long longValue) ? (int)longValue : 0;
            set => DictionaryUtilAbstractions.AddOrReplace(this, "TraversalStatistics.FilesErrored", value);
        }

        /// <summary>
        /// Total number of files in this traversal event.
        /// </summary>
        public int TotalFiles => FilesProcessed + FilesDeleted + FilesErrored;

        /// <summary>
        /// Reports the total size of all the files that were encounterd, skipped or deleted.
        /// </summary>
        public long TotalSizeOfFiles => SizeOfFilesProcessed + SizeOfFilesDeleted;
    }
}
