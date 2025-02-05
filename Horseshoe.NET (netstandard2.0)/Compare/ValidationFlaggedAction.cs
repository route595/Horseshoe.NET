using System;

namespace Horseshoe.NET.Compare
{
    /// <summary>
    /// Any special actions to perform during a comparison validation.
    /// </summary>
    [Flags]
    public enum ValidationFlaggedAction
    {
        /// <summary>
        /// Default, no action.
        /// </summary>
        None = 0,

        /// <summary>
        /// Switch the low and high criteria for a 'between' comparison if supplied in high-to-low order.
        /// </summary>
        SwitchHiAndLoValues = 1
    }
}
