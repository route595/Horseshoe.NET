using System;
using System.IO;

namespace Horseshoe.NET.IO
{
    public struct FilePath
    {
        public FileInfo File { get; }

        public FilePath(string filePath)
        {
            File = new FileInfo(filePath ?? throw new ArgumentNullException(nameof(filePath)));
        }

        public FilePath(FileInfo file)
        {
            File = file ?? throw new ArgumentNullException(nameof(file));
        }

        public string Name =>
            File.Name;

        public string FullName =>
            File.FullName;

        public string Extension =>
            File.Extension;

        public bool Exists => 
            File.Exists;

        public long Length =>
            File.Length;

        public long Size =>
            File.Length;

        public string GetDisplaySize(int? minDecimalPlaces = null, int? maxDecimalPlaces = null, bool addSeparators = false, FileSize.Unit? unit = null, bool? bi = null) =>
            FileUtil.GetDisplayFileSize(File.Length, minDecimalPlaces: minDecimalPlaces, maxDecimalPlaces: maxDecimalPlaces, addSeparators: addSeparators, unit: unit, bi: bi);

        public DateTime DateModified => 
            File.LastWriteTime;

        public void MoveTo(string destFileName) =>
            File.MoveTo(destFileName);

        public void CopyTo(string destFileName) =>
            File.CopyTo(destFileName);

        public void CopyTo(string destFileName, bool overwrite) =>
            File.CopyTo(destFileName, overwrite);

        public FileStream Open(FileMode mode) =>
            File.Open(mode);

        public FileStream Open(FileMode mode, FileAccess fileAccess) =>
            File.Open(mode, fileAccess);

        public FileStream Open(FileMode mode, FileAccess fileAccess, FileShare fileShare) =>
            File.Open(mode, fileAccess, fileShare);

        public FileStream OpenRead() =>
            File.OpenRead();

        public StreamReader OpenText() =>
            File.OpenText();

        public FileStream OpenWrite() =>
            File.OpenWrite();

        public void Delete() =>
            File.Delete();

        public override string ToString() =>
            File.ToString();

        public static implicit operator FilePath(string filePath) => new FilePath(filePath);
        //public static implicit operator string(FilePath filePath) => filePath.ToString();
        public static implicit operator FilePath(FileInfo file) => new FilePath(file);
        public static implicit operator FileInfo(FilePath filePath) => filePath.File;
    }
}
