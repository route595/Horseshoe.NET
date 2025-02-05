namespace Horseshoe.NET.ConsoleX
{
    /// <summary>
    /// Dictates whether lists render with indexes which are 0 or 1-based.
    /// </summary>
    public enum ListIndexStyle
    {
        /// <summary>
        /// Displays list with no indexes.
        /// </summary>
        None,

        /// <summary>
        /// Displays to user and user inputs 0-based indexes.
        /// </summary>
        ZeroBased,

        /// <summary>
        /// Displays to user and user inputs 1-based indexes.
        /// </summary>
        OneBased
    }
}
