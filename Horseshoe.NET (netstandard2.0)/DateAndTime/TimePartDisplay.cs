namespace Horseshoe.NET.DateAndTime
{
    /// <summary>
    /// Switches specific date time parts on or off in the displayed output using <c>SetOptions</c>.  Options include years, months, days, etc.
    /// </summary>
    public enum TimePartDisplay
    {
        /// <summary>
        /// Precludes the specific date time part from the output
        /// </summary>
        Off,

        /// <summary>
        /// Includes the specific date time part in the output
        /// </summary>
        On,

        /// <summary>
        /// Allows the specific date time part to be displayed in the output, or not, depending on the algorithm
        /// </summary>
        Auto
    }
}
