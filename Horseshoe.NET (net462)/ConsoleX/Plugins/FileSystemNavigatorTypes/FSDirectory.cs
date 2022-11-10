namespace Horseshoe.NET.ConsoleX.Plugins.FileSystemNavigatorTypes
{
    public class FSDirectory : MenuObject
    {
        public string Path { get; }

        public FSDirectory(string path, string text) : base(text)
        {
            Path = path;
        }
    }
}
