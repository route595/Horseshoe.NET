using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.AccessControl;

using Horseshoe.NET.Text;

namespace Horseshoe.NET.IO
{
    /// <summary>
    /// A versatile, code saving, <c>DirectoryInfo</c>-backed directory class with implicit conversion to and from both <c>string</c> and <c>DirectoryInfo</c>. 
    /// </summary>
    public readonly struct DirectoryPath : IEquatable<DirectoryPath>
    {
        /// <summary>
        /// The backing <c>DirectoryInfo</c> instance
        /// </summary>
        public DirectoryInfo Directory { get; }

        /// <summary>
        /// Creates a new <c>DirectoryPath</c>
        /// </summary>
        /// <param name="directoryPath">A <c>string</c> path</param>
        /// <exception cref="ArgumentNullException"></exception>
        public DirectoryPath(string directoryPath)
        {
            Directory = new DirectoryInfo(directoryPath ?? throw new ArgumentNullException(nameof(directoryPath)));
        }

        /// <summary>
        /// Creates a new <c>DirectoryPath</c>
        /// </summary>
        /// <param name="directory">A <c>DirectoryInfo</c> instance</param>
        /// <exception cref="ArgumentNullException"></exception>
        public DirectoryPath(DirectoryInfo directory)
        {
            Directory = directory ?? throw new ArgumentNullException(nameof(directory));
        }

        /// <inheritdoc cref="DirectoryInfo.Name"/>
        public string Name =>
            Directory.Name;

        /// <inheritdoc cref="FileSystemInfo.FullName"/>
        public string FullName =>
            Directory.FullName;

        /// <summary>
        /// Gets the number of characters in the current directory's <see cref="FileSystemInfo.FullName">full path</see>.
        /// </summary>
        public int Length =>
            FullName.Length;

        /// <inheritdoc cref="DirectoryInfo.Parent"/>
        public DirectoryPath? Parent =>
            Directory.Parent != null ? Directory.Parent : null;

        /// <inheritdoc cref="DirectoryInfo.Create()"/>
        public void Create() =>
            Directory.Create();

        /// <inheritdoc cref="DirectoryInfo.Create(DirectorySecurity)"/>  // DirectorySecurity not in .NET Standard 2.0
        public void Create(DirectorySecurity directorySecurity) =>
            Directory.Create(directorySecurity);

        /// <inheritdoc cref="DirectoryInfo.Delete()"/>
        public void Delete() =>
            Directory.Delete();

        /// <inheritdoc cref="DirectoryInfo.Delete(bool)"/>
        public void Delete(bool recursive) =>
            Directory.Delete(recursive);

        /// <inheritdoc cref="DirectoryInfo.Exists"/>
        public bool Exists =>
            Directory.Exists;

        /// <summary>
        /// Tests whether this directory is devoid of any subdirectories and files.
        /// </summary>
        /// <returns><c>true</c> or <c>false</c></returns>
        public bool IsEmpty() =>
            Directory.GetFileSystemInfos().Length == 0;

        /// <inheritdoc cref="FileSystemInfo.LastWriteTime"/>
        public DateTime DateModified
        {
            get => Directory.LastWriteTime;
            set => Directory.LastWriteTime = value;
        }

        /// <inheritdoc cref="FileSystemInfo.LastWriteTimeUtc"/>
        public DateTime DateModifiedUtc
        {
            get => Directory.LastWriteTimeUtc;
            set => Directory.LastWriteTimeUtc = value;
        }

        /// <inheritdoc cref="DirectoryInfo.GetFiles()"/>
        public FilePath[] GetFiles() =>
            Directory.GetFiles().Select(f => new FilePath(f)).ToArray();

        /// <inheritdoc cref="DirectoryInfo.GetFiles(string)"/>
        public FilePath[] GetFiles(string searchPattern) =>
            Directory.GetFiles(searchPattern).Select(f => new FilePath(f)).ToArray();

        /// <inheritdoc cref="DirectoryInfo.GetFiles(string, SearchOption)"/>
        public FilePath[] GetFiles(string searchPattern, SearchOption searchOption) =>
            Directory.GetFiles(searchPattern, searchOption).Select(f => new FilePath(f)).ToArray();

        /// <inheritdoc cref="DirectoryInfo.GetDirectories()"/>
        public DirectoryPath[] GetDirectories() =>
            Directory.GetDirectories().Select(d => new DirectoryPath(d)).ToArray();

        /// <inheritdoc cref="DirectoryInfo.GetDirectories(string)"/>
        public DirectoryPath[] GetDirectories(string searchPattern) =>
            Directory.GetDirectories(searchPattern).Select(d => new DirectoryPath(d)).ToArray();

        /// <inheritdoc cref="DirectoryInfo.GetDirectories(string, SearchOption)"/>
        public DirectoryPath[] GetDirectories(string searchPattern, SearchOption searchOption) =>
            Directory.GetDirectories(searchPattern, searchOption).Select(d => new DirectoryPath(d)).ToArray();

        /// <summary>
        /// Returns the full directory path
        /// </summary>
        /// <returns></returns>
        public override string ToString() =>
            Directory?.FullName ?? TextConstants.Null;

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (obj is DirectoryPath other)
                return Equals(other);
            return false;
        }

        /// <summary>
        /// Indicates whether this <c>DirectoryPath</c> is equal to another.
        /// </summary>
        /// <param name="other">Another <c>DirectoryPath</c></param>
        /// <returns><c>true</c> or <c>false</c></returns>
        public bool Equals(DirectoryPath other)
        {
            return string.Equals(FullName, other.FullName, StringComparison.OrdinalIgnoreCase);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return 772425832 + EqualityComparer<DirectoryInfo>.Default.GetHashCode(Directory);
        }

        /// <summary>
        /// Indicates whether this <c>DirectoryPath</c> is equal to another.
        /// </summary>
        /// <param name="a">A <c>DirectoryPath</c></param>
        /// <param name="b">Another <c>DirectoryPath</c></param>
        /// <returns><c>true</c> or <c>false</c></returns>
        public static bool operator ==(DirectoryPath a, DirectoryPath b)
        {
            return a.Equals(b);
        }

        /// <summary>
        /// Indicates whether this <c>DirectoryPath</c> is not equal to another.
        /// </summary>
        /// <param name="a">A <c>DirectoryPath</c></param>
        /// <param name="b">Another <c>DirectoryPath</c></param>
        /// <returns><c>true</c> or <c>false</c></returns>
        public static bool operator !=(DirectoryPath a, DirectoryPath b)
        {
            return !(a == b);
        }

        /// <summary>
        /// Implicitly converts a <c>string</c> to a <c>DirectoryPath</c>
        /// </summary>
        /// <param name="directoryPath">A <c>string</c> path</param>
        public static implicit operator DirectoryPath(string directoryPath) => new DirectoryPath(directoryPath);

        /// <summary>
        /// Implicitly converts a <c>DirectoryPath</c> to a <c>string</c>
        /// </summary>
        /// <param name="directory">A <c>DirectoryPath</c></param>
        public static implicit operator string(DirectoryPath directory) => directory.FullName;

        /// <summary>
        /// Implicitly converts a <c>DirectoryInfo</c> to a <c>DirectoryPath</c>
        /// </summary>
        /// <param name="directory">A <c>DirectoryInfo</c> instance</param>
        public static implicit operator DirectoryPath(DirectoryInfo directory) => new DirectoryPath(directory);

        /// <summary>
        /// Implicitly converts a <c>DirectoryPath</c> back to a <c>DirectoryInfo</c>
        /// </summary>
        /// <param name="directory">A <c>DirectoryPath</c></param>
        public static implicit operator DirectoryInfo(DirectoryPath directory) => directory.Directory;
    }
}
