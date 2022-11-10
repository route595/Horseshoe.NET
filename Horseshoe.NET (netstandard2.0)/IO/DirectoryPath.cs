using System;
using System.IO;

namespace Horseshoe.NET.IO
{
    public struct DirectoryPath
    {
        private DirectoryInfo Directory { get; }

        public DirectoryPath(string directoryPath)
        {
            Directory = new DirectoryInfo(directoryPath ?? throw new ArgumentNullException(nameof(directoryPath)));
        }

        public DirectoryPath(DirectoryInfo directory)
        {
            Directory = directory ?? throw new ArgumentNullException(nameof(directory));
        }

        public string Name =>
            Directory.Name;

        public string FullName =>
            Directory.FullName;

        public bool Exists =>
            Directory.Exists;

        public DateTime DateModified =>
            Directory.LastWriteTime;

        public FileInfo[] GetFiles() =>
            Directory.GetFiles();

        public void Create() =>
            Directory.Create();

        public void Delete() =>
            Directory.Delete();

        public FileInfo[] GetFiles(string searchPattern) =>
            Directory.GetFiles(searchPattern);

        public FileInfo[] GetFiles(string searchPattern, SearchOption searchOption) =>
            Directory.GetFiles(searchPattern, searchOption);

        public DirectoryInfo[] GetDirectories() =>
            Directory.GetDirectories();

        public DirectoryInfo[] GetDirectories(string searchPattern) =>
            Directory.GetDirectories(searchPattern);

        public DirectoryInfo[] GetDirectories(string searchPattern, SearchOption searchOption) =>
            Directory.GetDirectories(searchPattern, searchOption);

        public override string ToString() =>
            Directory?.ToString();

        public static implicit operator DirectoryPath(string directoryPath) => new DirectoryPath(directoryPath);
        public static implicit operator string(DirectoryPath directoryPath) => directoryPath.ToString();
        public static implicit operator DirectoryPath(DirectoryInfo directory) => new DirectoryPath(directory);
        public static implicit operator DirectoryInfo(DirectoryPath directoryPath) => directoryPath.Directory;
    }
}
