namespace Horseshoe.NET.ConsoleX.Plugins.FileSystemNavigatorTypes
{
    public class FSFile : MenuObject
    {
        public string Path { get; }

        public FSFile(string path, string text) : base(text)
        {
            Path = path;
        }
    }
}
