using System;

namespace Horseshoe.NET.IO.DirectoryCrawler
{
    /// <summary>
    /// Granular settings specifying file copy operation behaviors
    /// </summary>
    [Flags]
    public enum CopyMode
    {
        /// <summary>
        /// Asserts whether the destination directory is empty or non-existent
        /// </summary>
        ErrorIfDestinationNotEmpty = 0,

        /// <summary>
        /// Enables blanket file overwriting capabilities.  This option alone does not remove orphaned destination files and folders, for this please see <c>RemoveDestinationFilesAndDirectoriesNotInSource</c>.
        /// </summary>
        Overwrite = 1,

        /// <summary>
        /// Adds an overwrite exception for already existing destination files if the source file is newer, rarely used.
        /// </summary>
        DoNotOverwriteFileIfNewer = 2,

        /// <summary>
        /// Adds an overwrite exception for already existing destination files if the source file has the same modified date, commonly used.
        /// </summary>
        DoNotOverwriteFileIfSameModifiedDate = 4,

        /// <summary>
        /// Adds an overwrite exception for already existing destination files if the source file produces the same hash, use with caution in scenarios involving network drives.
        /// </summary>
        DoNotOverwriteFileIfMatchingHash = 8,

        /// <summary>
        /// Removes orphaned destination files and folders.
        /// </summary>
        RemoveDestinationFilesAndDirectoriesNotInSource = 16,

        /// <summary>
        /// Attempts to prevent syncing and copying issues by emptying the destination root directory prior to copying the source to the destination, possibly less performant.
        /// </summary>
        DeleteDestinationDirectoryBeforeCopy = 32,
    }
}
