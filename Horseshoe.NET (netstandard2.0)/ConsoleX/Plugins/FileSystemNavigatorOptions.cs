namespace Horseshoe.NET.ConsoleX.Plugins
{
    /// <summary>
    /// <c>FileSystemNavigator</c> options
    /// </summary>
    public class FileSystemNavigatorOptions
    {
        /// <summary>
        /// Specify the start directory
        /// </summary>
        public string StartDirectory { get; set; }

        /// <summary>
        /// Whether the crawler only scans directories, default is <c>false</c>
        /// </summary>
        public bool DirectoryModeOn { get; set; }

        /// <summary>
        /// Whether "(Go up one directory)" goes outside the start directory, default is <c>false</c>
        /// </summary>
        public bool AllowTraversalOutsideStartDirectory { get; set; }
    }
}
