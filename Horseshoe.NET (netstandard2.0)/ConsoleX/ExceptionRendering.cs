namespace Horseshoe.NET.ConsoleX
{
    /// <summary>
    /// Global exception rendering preferences
    /// </summary>
    public class ExceptionRendering
    {
        /// <summary>
        /// Preferences for rendering the exception class name
        /// </summary>
        /// <remarks><seealso cref="ExceptionTypeRenderingPolicy"/></remarks>
        public ExceptionTypeRenderingPolicy TypeRendering { get; set; }

        /// <summary>
        /// Set to <c>true</c> to include date/time
        /// </summary>
        public bool IncludeDateTime { get; set; }

        /// <summary>
        /// Set to <c>true</c> to include machine name
        /// </summary>
        public bool IncludeMachineName { get; set; }

        /// <summary>
        /// Set to <c>true</c> to include stack trace
        /// </summary>
        public bool IncludeStackTrace { get; set; }

        /// <summary>
        /// How deep to indent new lines (default = 2)
        /// </summary>
        public int Indent { get; set; } = 2;

        /// <summary>
        /// Set to <c>true</c> to include all the inner exceptions recursively
        /// </summary>
        public bool Recursive { get; set; }
    }
}
