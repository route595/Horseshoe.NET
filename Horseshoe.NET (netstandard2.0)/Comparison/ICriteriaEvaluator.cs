using System;

namespace Horseshoe.NET.Comparison
{
    /// <summary>
    /// Defines the method that evaulates an item, particularly against a supplied criterium or set of criteria
    /// </summary>
    /// <typeparam name="T">A comparable type.</typeparam>
    public interface ICriteriaEvaluator<T> where T : IComparable<T>
    {
        /// <summary>
        /// Represents a concrete method that evaluates an item, particularly against a supplied criterium or set of criteria
        /// </summary>
        Func<T, bool> Eval { get; }
    }
}
