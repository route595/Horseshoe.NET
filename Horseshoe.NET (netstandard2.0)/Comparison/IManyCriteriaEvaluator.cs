using System;

namespace Horseshoe.NET.Comparison
{
    /// <summary>
    /// Defines the method that evaulates an item against a collection of criteria, e.g. (item, criteria) => { ... }
    /// </summary>
    /// <typeparam name="T">Runtime type of items being compared</typeparam>
    public interface IManyCriteriaEvaluator<T> where T : IComparable<T>
    {
        /// <summary>
        /// Represents a concrete method that evaluates an item against a collection of criteria, e.g. (item, criteria) => { ... }
        /// </summary>
        Func<T, T[], bool> Eval { get; }
    }
}
