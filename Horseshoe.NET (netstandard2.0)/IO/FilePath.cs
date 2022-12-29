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

        /// <inheritdoc cref="System.IO.FileInfo.Name"/>
        public string Name =>
            File.Name;

        /// <inheritdoc cref="System.IO.FileSystemInfo.FullName"/>
        public string FullName =>
            File.FullName;

        /// <inheritdoc cref="System.IO.Path.GetDirectoryName(string)"/>
        public DirectoryPath Parent =>
            Path.GetDirectoryName(FullName);

        /// <inheritdoc cref="System.IO.FileSystemInfo.Extension"/>
        public string Extension =>
            File.Extension;

        /// <inheritdoc cref="System.IO.FileInfo.Exists"/>
        public bool Exists => 
            File.Exists;

        /// <inheritdoc cref="System.IO.FileInfo.Length"/>
        public long Length =>
            File.Length;

        /// <inheritdoc cref="System.IO.FileInfo.Length"/>
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
            FileUtil.GetDisplayFileSize(File.Length, minDecimalPlaces: minDecimalPlaces, maxDecimalPlaces: maxDecimalPlaces, addSeparators: addSeparators, unit: unit, bi: bi);

        /// <inheritdoc cref="System.IO.FileSystemInfo.LastWriteTime"/>
        public DateTime DateModified
        {
            get => File.LastWriteTime;
            set => File.LastWriteTime = value;
        }

        /// <inheritdoc cref="System.IO.FileSystemInfo.LastWriteTimeUtc"/>
        public DateTime DateModifiedUtc
        {
            get => File.LastWriteTimeUtc;
            set => File.LastWriteTimeUtc = value;
        }

        /// <inheritdoc cref="System.IO.FileInfo.MoveTo(string)"/>
        public void MoveTo(string destFileName) =>
            File.MoveTo(destFileName);

        /// <inheritdoc cref="System.IO.FileInfo.CopyTo(string)"/>
        public void CopyTo(string destFileName) =>
            File.CopyTo(destFileName);

        /// <inheritdoc cref="System.IO.FileInfo.CopyTo(string, bool)"/>
        public void CopyTo(string destFileName, bool overwrite) =>
            File.CopyTo(destFileName, overwrite);

        /// <inheritdoc cref="System.IO.FileInfo.Open(FileMode)"/>
        public FileStream Open(FileMode mode) =>
            File.Open(mode);

        /// <inheritdoc cref="System.IO.FileInfo.Open(FileMode, FileAccess)"/>
        public FileStream Open(FileMode mode, FileAccess fileAccess) =>
            File.Open(mode, fileAccess);

        /// <inheritdoc cref="System.IO.FileInfo.Open(FileMode, FileAccess, FileShare)"/>
        public FileStream Open(FileMode mode, FileAccess fileAccess, FileShare fileShare) =>
            File.Open(mode, fileAccess, fileShare);

        /// <inheritdoc cref="System.IO.FileInfo.OpenRead"/>
        public FileStream OpenRead() =>
            File.OpenRead();

        /// <inheritdoc cref="System.IO.FileInfo.OpenText"/>
        public StreamReader OpenText() =>
            File.OpenText();

        /// <inheritdoc cref="System.IO.File.ReadAllBytes(string)"/>
        public byte[] ReadAllBytes() =>
            System.IO.File.ReadAllBytes(FullName);

        /// <inheritdoc cref="System.IO.File.ReadAllText(string)"/>
        public string ReadAllText() =>
            System.IO.File.ReadAllText(FullName);

        /// <inheritdoc cref="System.IO.File.ReadAllText(string, Encoding)"/>
        public string ReadAllText(Encoding encoding) =>
            System.IO.File.ReadAllText(FullName, encoding);

        /// <inheritdoc cref="System.IO.File.ReadAllLines(string)"/>
        public string[] ReadAllLines() =>
            System.IO.File.ReadAllLines(FullName);

        /// <inheritdoc cref="System.IO.File.ReadAllLines(string, Encoding)"/>
        public string[] ReadAllLines(Encoding encoding) =>
            System.IO.File.ReadAllLines(FullName, encoding);

        /// <inheritdoc cref="System.IO.FileInfo.OpenWrite"/>
        public FileStream OpenWrite() =>
            File.OpenWrite();

        /// <inheritdoc cref="System.IO.File.WriteAllText(string, string)"/>
        public void WriteAllText(string contents) =>
            System.IO.File.WriteAllText(FullName, contents);

        /// <inheritdoc cref="System.IO.File.WriteAllText(string, string, Encoding)"/>
        public void WriteAllText(string contents, Encoding encoding) =>
            System.IO.File.WriteAllText(FullName, contents, encoding);

        /// <inheritdoc cref="System.IO.File.WriteAllLines(string, string[])"/>
        public void WriteAllLines(string[] contents) =>
            System.IO.File.WriteAllLines(FullName, contents);

        /// <inheritdoc cref="System.IO.File.WriteAllLines(string, string[], Encoding)"/>
        public void WriteAllLines(string[] contents, Encoding encoding) =>
            System.IO.File.WriteAllLines(FullName, contents, encoding);

        /// <inheritdoc cref="System.IO.File.WriteAllLines(string, IEnumerable{string})"/>
        public void WriteAllLines(IEnumerable<string> contents) =>
            System.IO.File.WriteAllLines(FullName, contents);

        /// <inheritdoc cref="System.IO.File.WriteAllLines(string, IEnumerable{string}, Encoding)"/>
        public void WriteAllLines(IEnumerable<string> contents, Encoding encoding) =>
            System.IO.File.WriteAllLines(FullName, contents, encoding);

        /// <inheritdoc cref="System.IO.File.WriteAllBytes(string, byte[])"/>
        public void WriteAllBytes(byte[] contents) =>
            System.IO.File.WriteAllBytes(FullName, contents);

        /// <inheritdoc cref="System.IO.FileInfo.Delete"/>
        public void Delete() =>
            File.Delete();

        /// <summary>
        /// Returns the full file path
        /// </summary>
        /// <returns></returns>
        public override string ToString() =>
            TextUtil.Reveal(File?.FullName);

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
