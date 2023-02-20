namespace Horseshoe.NET.ConsoleX
{
    /// <summary>
    /// Dictates whether lists are rendered with indexes and whether indexes are 0 or 1-based.
    /// </summary>
    public enum ListIndexPolicy
    {
        /// <summary>
        /// Displays list with no indexes.
        /// </summary>
        None,

        /// <summary>
        /// Displays list with zero-based indexes.
        /// </summary>
        DisplayZeroBased,

        /// <summary>
        /// Displays list with one-based indexes.
        /// </summary>
        DisplayOneBased
    }
}
