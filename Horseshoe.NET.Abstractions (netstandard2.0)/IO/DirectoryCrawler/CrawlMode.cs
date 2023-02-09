using System;

namespace Horseshoe.NET.IO.DirectoryCrawler
{
    /// <summary>
    /// Dictates the base behavior and, optionally, a per-directory exception to the base behavior of the current traversal session. 
    /// </summary>
    public enum CrawlMode
    {
        /// <summary>
        /// Default mode.  Recursively processes files, traverses subdirectories.
        /// Suitable either as the base behavior or as a per-directory exception to the base behavior.
        /// </summary>
        /// <remarks>
        /// The files and directories to be processed can be filtered.
        /// </remarks>
        ProcessFiles,

        ///// <summary>
        ///// Processes directories only, traverses subdirectories.
        ///// Suitable either as the base behavior or as a per-directory exception to the base behavior.
        ///// </summary>
        ///// <remarks>
        ///// The directories to be processed can be filtered.
        ///// </remarks>
        //ProcessDirectories,

        ///// <summary>
        ///// Stops processing the current directory and advances to the next sibling directory, if any.  If none, 
        ///// returns to the parent directory whereupon it will exit and advance to its next sibling directory, if any, and so on.
        ///// Suitable primarily as a per-directory exception to the base behavior.  Not recommended as the base behavior.
        ///// </summary>
        ///// <remarks>
        ///// This value in metadata in the "directory entered" phase changes the behavior of the traversal engine for the current
        ///// directory only. If used in the "directory deleting" phase the deletion of the current directory is preempted.
        ///// </remarks>
        //SkipCurrentDirectory,

        ///// <summary>
        ///// Precluded files in the current directory, traverses subdirectorires.  The system uses this mode to enact client filtering.
        ///// Suitable primarily as a per-directory exception to the base behavior.  Not recommended as the base behavior.
        ///// </summary>
        //SkipFilesInCurrentDirectory,

        /// <summary>
        /// Recursively deletes files, traverses subdirectories.
        /// Suitable either as the base behavior or as a per-directory exception to the base behavior.
        /// </summary>
        /// <remarks>
        /// The files and subdirectories to be deleted can be filtered.  The current directory (e.g. the root directory) can be precluded.
        /// </remarks>
        DeleteFiles,

        /// <summary>
        /// Recursively deletes directories, traverses subdirectories that are not deleted.
        /// Suitable either as the base behavior or as a per-directory exception to the base behavior.
        /// </summary>
        /// <remarks>
        /// The subdirectories to be deleted can be filtered.
        /// </remarks>
        DeleteDirectories,

        ///// <summary>
        ///// Deletes files in the current directory and all subdirectories.
        ///// Suitable either as the base behavior or as a per-directory exception to the base behavior.
        ///// </summary>
        ///// <remarks>
        ///// You can use filters to control which files are deleted.
        ///// </remarks>
        //DeleteFiles,

        ///// <summary>
        ///// Recursively deletes the current directory and all its files and subdirectories.  
        ///// Suitable either as the base behavior or as a per-directory exception to the base behavior.
        ///// </summary>
        //RecursiveDeleteCurrentDirectory,
    }
}
