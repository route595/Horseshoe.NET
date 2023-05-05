using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using Horseshoe.NET.Text;

namespace Horseshoe.NET.IO
{
    /// <summary>
    /// A versatile, code saving, <c>FileInfo</c>-backed file class with implicit conversion to and from both <c>string</c> and <c>FileInfo</c>.  
    /// </summary>
    public readonly struct FilePath : IEquatable<FilePath>
    {
        /// <summary>
        /// The backing <c>FileInfo</c> instance
        /// </summary>
        public FileInfo File { get; }

        /// <summary>
        /// Creates a new <c>FilePath</c>
        /// </summary>
        /// <param name="filePath">A <c>string</c> path</param>
        /// <exception cref="ArgumentNullException"></exception>
        public FilePath(string filePath)
        {
            File = new FileInfo(HandleFilePathArg(filePath) ?? throw new ArgumentNullException(nameof(filePath)));
        }

        /// <summary>
        /// Creates a new <c>FilePath</c>
        /// </summary>
        /// <param name="file">A <c>FileInfo</c> instance</param>
        /// <exception cref="ArgumentNullException"></exception>
        public FilePath(FileInfo file)
        {
            File = file ?? throw new ArgumentNullException(nameof(file));
        }

        /// <inheritdoc cref="FileInfo.Name"/>
        public string Name =>
            File.Name;

        /// <inheritdoc cref="FileSystemInfo.FullName"/>
        public string FullName =>
            File.FullName;

        /// <inheritdoc cref="FileInfo.Directory"/>
        public DirectoryPath Directory =>
            File.Directory;

        /// <inheritdoc cref="FileInfo.DirectoryName"/>
        public string DirectoryName =>
            File.DirectoryName;

        /// <inheritdoc cref="FileSystemInfo.Extension"/>
        public string Extension =>
            File.Extension;

        /// <inheritdoc cref="FileInfo.Exists"/>
        public bool Exists => 
            File.Exists;

        /// <summary>
        /// Gets the number of characters in the current file's <see cref="FileSystemInfo.FullName">full path</see>.
        /// </summary>
        public long Length =>
            FullName.Length;

        /// <inheritdoc cref="FileInfo.Length"/>
        public long Size =>
            File.Length;

        /// <summary>
        /// Formats the file size as a number bytes, kilobytes, etc.
        /// </summary>
        /// <param name="minDecimalPlaces">Decimal places for rounding</param>
        /// <param name="maxDecimalPlaces">Decimal places for rounding</param>
        /// <param name="addSeparators">Set to <c>true</c> to add separators, such as commas like in culture <c>"en-US"</c></param>
        /// <param name="unit">The preferred file size unit, e.g. KB, MB, etc.  If not supplied the software will use its best guess.</param>
        /// <param name="bi">Whether to use kibibytes vs kilobytes, for example, default is <c>false</c>.</param>
        /// <returns></returns>
        public string GetDisplaySize(int? minDecimalPlaces = null, int? maxDecimalPlaces = null, bool addSeparators = false, FileSizeUnit? unit = null, bool bi = false) =>
            FileUtilAbstractions.GetDisplayFileSize(File.Length, minDecimalPlaces: minDecimalPlaces, maxDecimalPlaces: maxDecimalPlaces, addSeparators: addSeparators, unit: unit, bi: bi);

        /// <inheritdoc cref="FileSystemInfo.LastWriteTime"/>
        public DateTime DateModified
        {
            get => File.LastWriteTime;
            set => File.LastWriteTime = value;
        }

        /// <inheritdoc cref="FileSystemInfo.LastWriteTimeUtc"/>
        public DateTime DateModifiedUtc
        {
            get => File.LastWriteTimeUtc;
            set => File.LastWriteTimeUtc = value;
        }

        /// <inheritdoc cref="FileInfo.MoveTo(string)"/>
        public void MoveTo(FilePath destFile) =>
            File.MoveTo(destFile.FullName);

        /// <inheritdoc cref="FileInfo.CopyTo(string)"/>
        public void CopyTo(FilePath destFile) =>
            File.CopyTo(destFile.FullName);

        /// <inheritdoc cref="FileInfo.CopyTo(string, bool)"/>
        public void CopyTo(FilePath destFile, bool overwrite) =>
            File.CopyTo(destFile.FullName, overwrite);

        /// <inheritdoc cref="FileInfo.Open(FileMode)"/>
        public FileStream Open(FileMode mode) =>
            File.Open(mode);

        /// <inheritdoc cref="FileInfo.Open(FileMode, FileAccess)"/>
        public FileStream Open(FileMode mode, FileAccess fileAccess) =>
            File.Open(mode, fileAccess);

        /// <inheritdoc cref="FileInfo.Open(FileMode, FileAccess, FileShare)"/>
        public FileStream Open(FileMode mode, FileAccess fileAccess, FileShare fileShare) =>
            File.Open(mode, fileAccess, fileShare);

        /// <inheritdoc cref="FileInfo.OpenRead"/>
        public FileStream OpenRead() =>
            File.OpenRead();

        /// <inheritdoc cref="FileInfo.OpenText"/>
        public StreamReader OpenText() =>
            File.OpenText();

        /// <inheritdoc cref="File.ReadAllBytes(string)"/>
        public byte[] ReadAllBytes() =>
            System.IO.File.ReadAllBytes(FullName);

        /// <inheritdoc cref="File.ReadAllText(string)"/>
        public string ReadAllText() =>
            System.IO.File.ReadAllText(FullName);

        /// <inheritdoc cref="File.ReadAllText(string, Encoding)"/>
        public string ReadAllText(Encoding encoding) =>
            System.IO.File.ReadAllText(FullName, encoding);

        /// <inheritdoc cref="File.ReadAllLines(string)"/>
        public string[] ReadAllLines() =>
            System.IO.File.ReadAllLines(FullName);

        /// <inheritdoc cref="File.ReadAllLines(string, Encoding)"/>
        public string[] ReadAllLines(Encoding encoding) =>
            System.IO.File.ReadAllLines(FullName, encoding);

        /// <inheritdoc cref="FileInfo.OpenWrite"/>
        public FileStream OpenWrite() =>
            File.OpenWrite();

        /// <inheritdoc cref="File.WriteAllText(string, string)"/>
        public void WriteAllText(string contents) =>
            System.IO.File.WriteAllText(FullName, contents);

        /// <inheritdoc cref="File.WriteAllText(string, string, Encoding)"/>
        public void WriteAllText(string contents, Encoding encoding) =>
            System.IO.File.WriteAllText(FullName, contents, encoding);

        /// <inheritdoc cref="File.WriteAllLines(string, string[])"/>
        public void WriteAllLines(string[] contents) =>
            System.IO.File.WriteAllLines(FullName, contents);

        /// <inheritdoc cref="File.WriteAllLines(string, string[], Encoding)"/>
        public void WriteAllLines(string[] contents, Encoding encoding) =>
            System.IO.File.WriteAllLines(FullName, contents, encoding);

        /// <inheritdoc cref="File.WriteAllLines(string, IEnumerable{string})"/>
        public void WriteAllLines(IEnumerable<string> contents) =>
            System.IO.File.WriteAllLines(FullName, contents);

        /// <inheritdoc cref="File.WriteAllLines(string, IEnumerable{string}, Encoding)"/>
        public void WriteAllLines(IEnumerable<string> contents, Encoding encoding) =>
            System.IO.File.WriteAllLines(FullName, contents, encoding);

        /// <inheritdoc cref="File.WriteAllBytes(string, byte[])"/>
        public void WriteAllBytes(byte[] contents) =>
            System.IO.File.WriteAllBytes(FullName, contents);

        /// <inheritdoc cref="FileInfo.Delete"/>
        public void Delete() =>
            File.Delete();

        /// <summary>
        /// Returns the full file path
        /// </summary>
        /// <returns></returns>
        public override string ToString() =>
            File?.FullName ?? TextConstants.Null;

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (obj is FilePath other)
                return Equals(other);
            return false;
        }

        /// <summary>
        /// Indicates whether this <c>FilePath</c> is equal to another.
        /// </summary>
        /// <param name="other">another <c>FilePath</c></param>
        /// <returns><c>true</c> or <c>false</c></returns>
        public bool Equals(FilePath other)
        {
            return string.Equals(FullName, other.FullName, StringComparison.OrdinalIgnoreCase);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return -825322277 + EqualityComparer<FileInfo>.Default.GetHashCode(File);
        }

        /// <summary>
        /// Indicates whether this <c>FilePath</c> is equal to another.
        /// </summary>
        /// <param name="a">A <c>FilePath</c></param>
        /// <param name="b">Another <c>FilePath</c></param>
        /// <returns><c>true</c> or <c>false</c></returns>
        public static bool operator ==(FilePath a, FilePath b)
        {
            return a.Equals(b);
        }

        /// <summary>
        /// Indicates whether this <c>FilePath</c> is not equal to another.
        /// </summary>
        /// <param name="a">A <c>FilePath</c></param>
        /// <param name="b">Another <c>FilePath</c></param>
        /// <returns><c>true</c> or <c>false</c></returns>
        public static bool operator !=(FilePath a, FilePath b)
        {
            return !(a == b);
        }

        private static string HandleFilePathArg(string filePath)
        {
            if (filePath == null)
                return null;
            var separatorChars = new[] { '/', '\\' };
            foreach (var @char in separatorChars)
            {
                if (filePath.IndexOf(@char) > -1)
                    return filePath;
            }
            return Path.Combine(Environment.CurrentDirectory, filePath);
        }

        /// <summary>
        /// Implicitly converts a <c>string</c> to a <c>FilePath</c>
        /// </summary>
        /// <param name="filePath">A <c>string</c> path</param>
        public static implicit operator FilePath(string filePath) => new FilePath(filePath);

        /// <summary>
        /// Implicitly converts a <c>FilePath</c> to a <c>string</c>
        /// </summary>
        /// <param name="filePath">A <c>FilePath</c></param>
        public static implicit operator string(FilePath filePath) => filePath.FullName;

        /// <summary>
        /// Implicitly converts a <c>FileInfo</c> to a <c>FilePath</c>
        /// </summary>
        /// <param name="file">A <c>FileInfo</c> instance</param>
        public static implicit operator FilePath(FileInfo file) => new FilePath(file);

        /// <summary>
        /// Implicitly converts a <c>FilePath</c> back to a <c>FileInfo</c>
        /// </summary>
        /// <param name="filePath">A <c>FilePath</c></param>
        public static implicit operator FileInfo(FilePath filePath) => filePath.File;
    }
}
