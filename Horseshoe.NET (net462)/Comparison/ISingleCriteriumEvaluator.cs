using System;

namespace Horseshoe.NET.Comparison
{
    /// <summary>
    /// Defines the method that evaulates an item against a single criterium, e.g. (item, criterium) => { ... }
    /// </summary>
    /// <typeparam name="T">Runtime type of items being compared</typeparam>
    public interface ISingleCriteriumEvaluator<T> where T : IComparable<T>
    {
        /// <summary>
        /// Represents a concrete method that evaluates an item against a single criterium, e.g. (item, criterium) => { ... }
        /// </summary>
        Func<T, T, bool> Eval { get; }
    }
}
