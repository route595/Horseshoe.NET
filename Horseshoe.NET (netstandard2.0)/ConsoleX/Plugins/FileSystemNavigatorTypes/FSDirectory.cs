namespace Horseshoe.NET.ConsoleX.Plugins.FileSystemNavigatorTypes
{
    /// <summary>
    /// A class that represents a directory in <c>FileSystemNavigator</c> and is a <c>MenuObject</c> 
    /// </summary>
    public class FSDirectory : MenuObject
    {
        /// <summary>
        /// The directory path
        /// </summary>
        public string Path { get; }

        /// <summary>
        /// Create a new FSDirectory
        /// </summary>
        /// <param name="path">a directory path</param>
        /// <param name="text">a directory name</param>
        public FSDirectory(string path, string text) : base(text)
        {
            Path = path;
        }
    }
}
