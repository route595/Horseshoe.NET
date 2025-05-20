using System;
using System.Collections.Generic;
using System.Linq;

namespace Horseshoe.NET.Comparison
{
    /// <summary>
    /// Defines a multi-criteria comparator
    /// </summary>
    /// <typeparam name="T">Runtime type of items being compared</typeparam>
    public class ManyCriterinator<T> : ICriterinator<T>, IManyCriteriaEvaluator<T> where T : IComparable<T>
    {
        /// <summary>
        /// An array of values used as comparison criteria
        /// </summary>
        public T[] Criteria { get; }

        /// <inheritdoc cref="ICriteriaEvaluator{T}.Eval"/>
        public Func<T, T[], bool> Eval { get; }

        /// <summary>
        /// Creates a new instance of the <see cref="ManyCriterinator{T}"/> class.
        /// </summary>
        /// <param name="criteria">A set of criterim</param>
        /// <param name="eval">An eval function</param>
        public ManyCriterinator(IEnumerable<T> criteria, Func<T, T[], bool> eval) : this(criteria?.ToArray(), eval)
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="ManyCriterinator{T}"/> class.
        /// </summary>
        /// <param name="criteria">A set of criterim</param>
        /// <param name="eval">An eval function</param>
        public ManyCriterinator(T[] criteria, Func<T, T[], bool> eval)
        {
            Criteria = criteria ?? throw new ArgumentNullException(nameof(criteria), nameof(criteria) + " cannot be null");
            Eval = eval;
        }

        /// <inheritdoc cref="ICriterinator{T}.IsMatch(T)"/>
        public bool IsMatch(T item)
        {
            if (Eval == null)
                return false;
            return Eval.Invoke(item, Criteria);
        }
    }
}
