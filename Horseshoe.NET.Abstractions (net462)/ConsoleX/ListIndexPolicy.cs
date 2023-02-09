namespace Horseshoe.NET.ConsoleX
{
    /// <summary>
    /// Determines if lists are rendered with selection indexes and whether such indexes are 0-based
    /// </summary>
    public enum ListIndexPolicy
    {
        /// <summary>
        /// Display lists with no indexes
        /// </summary>
        None,

        /// <summary>
        /// Display lists with zero based indexes
        /// </summary>
        DisplayZeroBased,

        /// <summary>
        /// Display lists with one-based indexes
        /// </summary>
        DisplayOneBased
    }
}
