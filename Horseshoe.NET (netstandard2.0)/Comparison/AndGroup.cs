using System;
using System.Collections.Generic;
using System.Linq;

namespace Horseshoe.NET.Comparison
{
    /// <summary>
    /// A group of <c>ICriterinator</c>s which matches only if all <c>ICriterinator</c>s match
    /// </summary>
    /// <typeparam name="T">Runtime type of items being compare</typeparam>
    public class AndGroup<T> : List<ICriterinator<T>>, ICriterinator<T> where T : IComparable<T>
    {
        /// <summary>
        /// Creates a new 'And' grouping of <c>ICriterinator</c>s
        /// </summary>
        /// <param name="criterinators">The constituent <c>ICriterinator</c>s</param>
        public AndGroup(IEnumerable<ICriterinator<T>> criterinators) : base(criterinators)
        {
        }

        /// <summary>
        /// Creates a new 'And' grouping of <c>ICriterinator</c>s
        /// </summary>
        /// <param name="criterinators">The constituent <c>ICriterinator</c>s</param>
        public AndGroup(params ICriterinator<T>[] criterinators) : base(criterinators)
        {
        }

        /// <summary>
        /// Returns <c>true</c> if all <c>ICriterinator</c>s match
        /// </summary>
        /// <param name="inputItem">An item to compare agains the criteria</param>
        /// <returns></returns>
        public bool IsMatch(T inputItem)
        {
            return this.Any() && this.All(c =>  c.IsMatch(inputItem)); 
        }
    }
}
