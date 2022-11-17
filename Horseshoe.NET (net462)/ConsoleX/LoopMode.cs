namespace Horseshoe.NET.ConsoleX
{
    /// <summary>
    /// Determines the behavior of the app, whether the screen clears for each new routine
    /// or if the display is continuous.
    /// </summary>
    public enum LoopMode
    {
        /// <summary>
        /// Continuous display (default)
        /// </summary>
        Continuous,

        /// <summary>
        /// Screen clears for each new routine
        /// </summary>
        ClearScreen
    }
}
