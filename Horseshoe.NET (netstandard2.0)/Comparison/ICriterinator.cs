using System;

namespace Horseshoe.NET.Comparison
{
    /// <summary>
    /// Defines all properties and methods common to Horseshoe.NET criterinators.
    /// </summary>
    /// <typeparam name="T">A comparable type.</typeparam>
    public interface ICriterinator<T> where T : IComparable<T>
    {
        /// <summary>
        /// Evaluates whether an item is a criteria match.  Implementations should handle any additional considerations
        /// such as case-sensitivity.
        /// </summary>
        /// <param name="item">An item which to compare agains the criteria.</param>
        bool IsMatch(T item);
    }
}
