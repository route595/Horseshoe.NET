using System;

namespace Horseshoe.NET.Comparison
{
    /// <summary>
    /// Defines a dual-criteria comparator.
    /// </summary>
    /// <typeparam name="T">Runtime type of items being compared</typeparam>
    public class RangeCriterinator<T> : ICriterinator<T>, IRangeCriteriaEvaluator<T> where T : IComparable<T>
    {
        /// <summary>
        /// The lowest of a pair of values used as comparison criteria
        /// </summary>
        public T LoCriterium { get; }

        /// <summary>
        /// The highest of a pair of values used as comparison criteria
        /// </summary>
        public T HiCriterium { get; }

        /// <inheritdoc cref="IRangeCriteriaEvaluator{T}.Eval"/>
        public Func<T, T, T, bool> Eval { get; }

        /// <summary>
        /// Creates a new instance of the <see cref="SingleCriterinator{T}"/> class.
        /// </summary>
        /// <param name="loCriterium">The lowest of a pair of values used as comparison criteria</param>
        /// <param name="hiCriterium">The highest of a pair of values used as comparison criteria</param>
        /// <param name="eval">An eval function</param>
        public RangeCriterinator(T loCriterium, T hiCriterium, Func<T, T, T, bool> eval)
        {
            LoCriterium = loCriterium;
            HiCriterium = hiCriterium;
            Eval = eval;
        }

        /// <inheritdoc cref="ICriterinator{T}.IsMatch(T)"/>
        public bool IsMatch(T item)
        {
            if (Eval == null)
                return false;
            return Eval.Invoke(item, LoCriterium, HiCriterium);
        }
    }
}
