namespace Horseshoe.NET.ConsoleX.Plugins.FileSystemNavigatorTypes
{
    /// <summary>
    /// A class that represents a file in <c>FileSystemNavigator</c> and is a <c>MenuObject</c> 
    /// </summary>
    public class FSFile : MenuObject
    {
        /// <summary>
        /// The file path
        /// </summary>
        public string Path { get; }

        /// <summary>
        /// Create a new FSFile
        /// </summary>
        /// <param name="path">a file path</param>
        /// <param name="text">a file name</param>
        public FSFile(string path, string text) : base(text)
        {
            Path = path;
        }
    }
}
