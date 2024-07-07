using System;
using System.IO;
using System.Text;
using Horseshoe.NET.IO.FileTraversal;

namespace Horseshoe.NET.IO
{
    /// <summary>
    /// Utility methods for files and directories.
    /// </summary>
    public static class FileUtil
    {
        /// <summary>
        /// Appends an extension to a file name
        /// </summary>
        /// <param name="fileName">A file name</param>
        /// <param name="extension">A common file type extension</param>
        /// <param name="preferUpperCase">Optional, set to <c>true</c> to append the extension in upper case, default is <c>false</c></param>
        /// <returns></returns>
        public static string AppendExtension(string fileName, FileType extension, bool preferUpperCase = false)
        {
            return AppendExtension(fileName, preferUpperCase ? extension.ToString() : extension.ToString().ToLower());
        }

        /// <summary>
        /// Appends an extension to a file name
        /// </summary>
        /// <param name="fileName">A file name</param>
        /// <param name="extension">An extension</param>
        /// <returns></returns>
        public static string AppendExtension(string fileName, string extension)
        {
            if (!extension.StartsWith("."))
                extension = "." + extension;
            if (fileName.ToLower().EndsWith(extension.ToLower()))
                return fileName;
            return fileName + extension;
        }

        /// <inheritdoc cref="FileUtilAbstractions.GetDisplayFileSize(FilePath, int?, int?, bool, FileSizeUnit?, bool)"/>
        public static string GetDisplayFileSize(FilePath file, int? minDecimalPlaces = null, int? maxDecimalPlaces = null, bool addSeparators = false, FileSizeUnit? unit = null, bool bi = false)
        {
            return FileUtilAbstractions.GetDisplayFileSize(file, minDecimalPlaces: minDecimalPlaces, maxDecimalPlaces: maxDecimalPlaces, addSeparators: addSeparators, unit: unit, bi: bi);
        }

        /// <inheritdoc cref="FileUtilAbstractions.GetDisplayFileSize(long?, int?, int?, bool, FileSizeUnit?, bool)"/>
        public static string GetDisplayFileSize(long? size, int? minDecimalPlaces = null, int? maxDecimalPlaces = null, bool addSeparators = true, FileSizeUnit? unit = null, bool bi = false)
        {
            return FileUtilAbstractions.GetDisplayFileSize(size, minDecimalPlaces: minDecimalPlaces, maxDecimalPlaces: maxDecimalPlaces, addSeparators: addSeparators, unit: unit, bi: bi);
        }

        /// <summary>
        /// Ensures paths end with path separator - internal method
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string NormalizePath(string path)
        {
            path = path.Trim();
            if (path[path.Length - 1] != Path.DirectorySeparatorChar)
            {
                path += Path.DirectorySeparatorChar;
            }
            return path;
        }

        /// <inheritdoc cref="FileUtilAbstractions.IsChildOf(DirectoryPath, DirectoryPath, bool)"/>
        public static bool IsChildOf(DirectoryPath directory, DirectoryPath ancestorDirectory, bool ignoreCase = false)
        {
            return FileUtilAbstractions.IsChildOf(directory, ancestorDirectory, ignoreCase: ignoreCase);
        }

        /// <inheritdoc cref="FileUtilAbstractions.IsChildOf(FilePath, DirectoryPath, bool)"/>
        public static bool IsChildOf(FilePath file, DirectoryPath ancestorDirectory, bool ignoreCase = false)
        {
            return FileUtilAbstractions.IsChildOf(file, ancestorDirectory, ignoreCase: ignoreCase);
        }

        /// <inheritdoc cref="FileUtilAbstractions.DisplayAsVirtualPathFromRoot(DirectoryPath, DirectoryPath, bool, bool, StringBuilder)">
        public static string DisplayAsVirtualPathFromRoot(DirectoryPath directory, DirectoryPath root, bool ignoreCase = false, bool strict = false, StringBuilder strb = null)
        {
            return FileUtilAbstractions.DisplayAsVirtualPathFromRoot(directory, root, ignoreCase: ignoreCase, strict: strict, strb: strb);
        }

        /// <inheritdoc cref="FileUtilAbstractions.DisplayAsVirtualPathFromRoot(FilePath, DirectoryPath, bool, bool, StringBuilder)">
        public static string DisplayAsVirtualPathFromRoot(FilePath file, DirectoryPath root, bool ignoreCase = false, bool strict = false, StringBuilder strb = null)
        {
            return FileUtilAbstractions.DisplayAsVirtualPathFromRoot(file, root, ignoreCase: ignoreCase, strict: strict, strb: strb);
        }

        /// <summary>
        /// Public method (also used by <c>TraversalEngine</c>) to recursively delete a directory.
        /// </summary>
        /// <param name="dir">A directory path.</param>
        /// <param name="preserveDir">If <c>true</c>, recursively empties the directory without deleting it.  Default is <c>false</c>.</param>
        public static void RecursiveDelete(DirectoryPath dir, bool preserveDir = false)
        {
            _RecursiveDelete(dir, preserveDir);
        }

        internal static void RecursiveDelete(TraversalEngine traversalEngine, DirectoryPath dir, bool preserveDir = false)
        {
            //traversalEngine.RecursiveDeletingDirectory?.Invoke(dir, traversalEngine);
            //_RecursiveDelete(traversalEngine, dir, preserveDir, true);
        }

        private static void _RecursiveDelete(DirectoryPath dir, bool preserveDir)
        {
            foreach (var file in dir.GetFiles())
            {
                file.Delete();
            }
            foreach (var subDir in dir.GetDirectories())
            {
                _RecursiveDelete(subDir, false);
            }
            if (!preserveDir)
            {
                dir.Delete();
            }
        }

        private static void _RecursiveDelete(TraversalEngine traversalEngine, DirectoryPath dir, bool preserveDir, bool silent)
        {
            //foreach (var file in dir.GetFiles())
            //{
            //    traversalEngine.FileMetadata.Reset(file.Length);
            //    DeleteFile(traversalEngine, file, silent);
            //}
            //foreach (var subDir in dir.GetDirectories())
            //{
            //    _RecursiveDelete(traversalEngine, subDir, false, silent);
            //}
            //if (!preserveDir)
            //{
            //    DeleteDir(traversalEngine, dir, silent);
            //}
            //traversalEngine.DirectoryDeleted?.Invoke(dir, traversalEngine);
        }

        internal static void DeleteDir(TraversalEngine traversalEngine, DirectoryPath dir, bool silent)
        {
            //if (!silent)
            //{
            //    traversalEngine.DeletingDirectory?.Invoke(dir, traversalEngine);
            //}
            //if (!traversalEngine.DryRun)
            //{
            //    try
            //    {
            //        dir.Delete();
            //    }
            //    catch (Exception ex)
            //    {
            //        if (traversalEngine.AccumulateErrors)
            //        {
            //            traversalEngine.DirectoriesUnableToBeDeleted++;
            //            traversalEngine.TraversalErrors.Add(new TraversalError { Path = dir.FullName, Message = ex.Render() });
            //            traversalEngine.DirectoryErrored?.Invoke(dir, traversalEngine);
            //        }
            //        else
            //        {
            //            throw;
            //        }
            //    }
            //}
            //traversalEngine.DirectoriesDeleted++;
            //if (!silent)
            //{
            //    traversalEngine.DirectoryDeleted?.Invoke(dir, traversalEngine);
            //}
        }

        internal static void DeleteFile(TraversalEngine traversalEngine, FilePath file, bool silent)
        {
            //traversalEngine.FileMetadata.FileSize = file.Length;
            //if (!silent)
            //{
            //    traversalEngine.DeletingFile?.Invoke(file, traversalEngine);
            //}
            //if (!traversalEngine.DryRun)
            //{
            //    try
            //    {
            //        file.Delete();
            //    }
            //    catch (Exception ex)
            //    {
            //        if (traversalEngine.AccumulateErrors)
            //        {
            //            traversalEngine.TraversalErrors.Add(new TraversalError { Path = file.FullName, Message = ex.Render() });
            //            traversalEngine.FilesUnableToBeDeleted++;
            //            traversalEngine.FileErrored?.Invoke(file, traversalEngine);
            //        }
            //        else
            //        {
            //            throw;
            //        }
            //    }
            //}
            //traversalEngine.FilesDeleted++;
            //traversalEngine.SizeOfFilesDeleted += traversalEngine.FileMetadata.FileSize;
            //if (!silent)
            //{
            //    traversalEngine.FileDeleted?.Invoke(file, traversalEngine);
            //}
        }
    }
}
