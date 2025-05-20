using System;

namespace Horseshoe.NET.Comparison
{
    /// <summary>
    /// Defines a single-criterium comparator.
    /// </summary>
    /// <typeparam name="T">Runtime type of items being compared</typeparam>
    public class SingleCriterinator<T> : ICriterinator<T>, ISingleCriteriumEvaluator<T> where T : IComparable<T>
    {
        /// <summary>
        /// A value used as comparison criteria
        /// </summary>
        public T Criterium { get; }

        /// <inheritdoc cref="ISingleCriteriumEvaluator{T}.Eval"/>
        public Func<T, T, bool> Eval { get; }

        /// <summary>
        /// Creates a new instance of the <see cref="SingleCriterinator{T}"/> class.
        /// </summary>
        /// <param name="criterium">A criterim</param>
        /// <param name="eval">An eval function</param>
        public SingleCriterinator(T criterium, Func<T, T, bool> eval)
        {
            Criterium = criterium;
            Eval = eval;
        }

        /// <inheritdoc cref="ICriterinator{T}.IsMatch(T)"/>
        public bool IsMatch(T item)
        {
            if (Eval == null)
                return false;
            return Eval.Invoke(item, Criterium);
        }
    }
}
