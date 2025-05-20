using System;

namespace Horseshoe.NET.Comparison
{
    /// <summary>
    /// Defines the method that evaulates an item against a criteria range, e.g. (item, lo, hi) => { ... }
    /// </summary>
    /// <typeparam name="T">Runtime type of items being compared</typeparam>
    public interface IRangeCriteriaEvaluator<T> where T : IComparable<T>
    {
        /// <summary>
        /// Represents a concrete method that evaluates an item against a criteria range, e.g. (item, lo, hi) => { ... }
        /// </summary>
        Func<T, T, T, bool> Eval { get; }
    }
}
