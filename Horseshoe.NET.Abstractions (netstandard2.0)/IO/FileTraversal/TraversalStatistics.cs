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
        private string _tempVirtualPath;

        public int DirectoryCount => this.Count(tse => tse.ObjectType == FileSystemObjectType.Directory);
        public int TotalFileCount => this.Count(tse => tse.ObjectType == FileSystemObjectType.File);
        public long TotalFileSize => this.Sum(tse => tse.FileSize ?? 0L);
        public int FilesDeleted { get; set; }

        private readonly static StringBuilder strb = new StringBuilder();  // using this optimizes memory

        public void Log(DirectoryPath directory, DirectoryPath root, string action)
        {
            Add(new Entry { VirtualPath = FileUtilAbstractions.DisplayAsVirtualPathFromRoot(directory, root, strb: strb), ObjectType = FileSystemObjectType.Directory, Action = action });
        }

        public void LogHello(DirectoryPath directory, DirectoryPath root)
        {
            Log(directory, root, TraversalConstants.Hello);
        }

        public void LogHello(FilePath file, DirectoryPath root)
        {
            Add(new Entry { VirtualPath = FileUtilAbstractions.DisplayAsVirtualPathFromRoot(file, root, strb: strb), ObjectType = FileSystemObjectType.File, FileSize = file.Size, Action = TraversalConstants.Hello });
        }

        //public void UpdateActionLastDirectory(string action)
        //{
        //    this.Last(tse => tse.ObjectType == FileSystemObjectType.Directory).Action = action;
        //}

        //public void UpdateActionLastFile(string action)
        //{
        //    this.Last(tse => tse.ObjectType == FileSystemObjectType.File).Action = action;
        //}

        public void UpdateAction(DirectoryPath directory, DirectoryPath root, string action)
        {
            _tempVirtualPath = FileUtilAbstractions.DisplayAsVirtualPathFromRoot(directory, root, strb: strb);
            this.Single(tse => tse.ObjectType == FileSystemObjectType.Directory && tse.VirtualPath == _tempVirtualPath).Action = action;
        }

        public void UpdateAction(FilePath file, DirectoryPath root, string action)
        {
            _tempVirtualPath = FileUtilAbstractions.DisplayAsVirtualPathFromRoot(file, root, strb: strb);
            this.Single(tse => tse.ObjectType == FileSystemObjectType.File && tse.VirtualPath == _tempVirtualPath).Action = action;
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
            //public string Path { get; }
            public string VirtualPath { get; set; }
            public FileSystemObjectType ObjectType { get; set; }
            public long? FileSize { get; set; }
            public string Action { get; set; }

            public override string ToString()
            {
                return Action + ": " + VirtualPath + (FileSize.HasValue ? " (" + FileUtilAbstractions.GetDisplayFileSize(FileSize.Value) + ")" : "")
                    + (ObjectType == FileSystemObjectType.Directory ? " (dir)" : "");
            }
        }
    }
}
