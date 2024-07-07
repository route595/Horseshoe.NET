using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Horseshoe.NET.IO.FileTraversal
{
    /// <summary>
    /// A snapshot of what occurred during the file traversal session  
    /// </summary>
    public class TraversalStatistics : List<TraversalStatistics.Entry>
    {
        public const string Hello = "Hello";
        public const string Skipping = "Skipping";
        public const string Skipped = "Skipped";
        public const string Deleting = "Deleting";
        public const string DeletingContents = "Deleting Contents";
        public const string Deleted = "Deleted";
        public const string ContentsDeleted = "Contents Deleted";
        public const string NoLogging = "NoLogging";
        public const string LogFile = "LogFile";
        public const string LogDirectory = "LogDirectory";
        public const string UpdateLogActionFile = "UpdateLogActionFile";
        public const string UpdateLogActionDirectory = "UpdateLogActionDirectory";

        public int DirectoryCount => this.Count(tse => tse.ObjectType == FileSystemObjectType.Directory);
        public int TotalFileCount => this.Count(tse => tse.ObjectType == FileSystemObjectType.File);
        public long TotalFileSize => this.Sum(tse => tse.FileSize ?? 0L);
        public int FilesDeleted { get; set; }

        public void Log(DirectoryPath directory, DirectoryPath root, string action = Hello)
        {
            Add(new Entry(directory, root, FileSystemObjectType.Directory, action: action));
        }

        public void Log(FilePath file, DirectoryPath root, long? fileSize = null, string action = Hello)
        {
            Add(new Entry(file, root, FileSystemObjectType.File, fileSize: fileSize, action: action));
        }

        //public void UpdateActionLastDirectory(string action)
        //{
        //    this.Last(tse => tse.ObjectType == FileSystemObjectType.Directory).Action = action;
        //}

        //public void UpdateActionLastFile(string action)
        //{
        //    this.Last(tse => tse.ObjectType == FileSystemObjectType.File).Action = action;
        //}

        public void UpdateAction(DirectoryPath directory, string action)
        {
            this.Single(tse => tse.ObjectType == FileSystemObjectType.Directory && tse.Path == directory.FullName).Action = action;
        }

        public void UpdateAction(FilePath file, string action)
        {
            this.Single(tse => tse.ObjectType == FileSystemObjectType.File && tse.Path == file.FullName).Action = action;
        }

        public string Dump()
        {
            if (!this.Any())
                return "No Entries" + Environment.NewLine;
            var strb = new StringBuilder();
            var groups = this
                .GroupBy(e => e.ObjectType)
                .OrderBy(grp => grp.Key);
            foreach (var group in groups)
            {
                switch (group.Key)
                {
                    case FileSystemObjectType.Directory:
                        strb.AppendLine("Directories");
                        break;
                    case FileSystemObjectType.File:
                        strb.AppendLine("Files");
                        break;
                }
                var groups2 = group
                    .GroupBy(e => e.Action)
                    .OrderBy(grp => grp.Key);
                foreach(var group2 in groups2)
                {
                    string sz = "";
                    if (group.Key == FileSystemObjectType.File)
                    {
                        var tot = group2.Sum(e => e.FileSize ?? 0L);
                        if (tot > 0)
                            sz = " (" + FileUtilAbstractions.GetDisplayFileSize(tot) + ")";
                    }
                    strb.AppendLine("  " + group2.Count() + " " + group2.Key + sz);
                }
            }
            return strb.ToString();
        }

        public class Entry
        {
            public string Path { get; }
            public string VirtualPath { get; }
            public FileSystemObjectType ObjectType { get; }
            public long? FileSize { get; }
            public string Action { get; set; }

            private readonly static StringBuilder strb = new StringBuilder();  // using this optimizes memory

            public Entry(FilePath file, DirectoryPath root, FileSystemObjectType objectType, long? fileSize = null, string action = null)
                : this(FileUtilAbstractions.DisplayAsVirtualPathFromRoot(file, root, strb: strb), objectType, fileSize: fileSize ?? (file.Exists ? file.Size as long? : null), action: action)
            {
                Path = file;
            }

            public Entry(DirectoryPath directory, DirectoryPath root, FileSystemObjectType objectType, string action = null)
                : this(FileUtilAbstractions.DisplayAsVirtualPathFromRoot(directory, root, strb: strb), objectType, action: action)
            {
                Path = directory;
            }

            private Entry(string virtualPath, FileSystemObjectType objectType, long? fileSize = null, string action = null)
            {
                VirtualPath = virtualPath;
                ObjectType = objectType;
                FileSize = fileSize;
                Action = action;
            }

            public override string ToString()
            {
                return Action + ": " + VirtualPath + (FileSize.HasValue ? " (" + FileUtilAbstractions.GetDisplayFileSize(FileSize.Value) + ")" : "")
                    + (ObjectType == FileSystemObjectType.Directory ? " (dir)" : "");
            }
        }
    }
}
