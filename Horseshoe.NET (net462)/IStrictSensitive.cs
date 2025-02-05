namespace Horseshoe.NET
{
    /// <summary>
    /// Describes an exception that, when caught, can be ignored if 
    /// <c>IsStrictSensitive == true</c> and the caller chooses to switch strictness off.
    /// </summary>
    public interface IStrictSensitive
    {
        /// <summary>
        /// If <c>true</c> this exception can be either thrown or ignored depending on whether the caller chooses to switch strictness on or off.
        /// </summary>
        bool IsStrictSensitive { get; }
    }
}
