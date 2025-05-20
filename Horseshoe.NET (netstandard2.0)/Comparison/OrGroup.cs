using System;
using System.Collections.Generic;
using System.Linq;

namespace Horseshoe.NET.Comparison
{
    /// <summary>
    /// A group of <c>ICriterinator</c>s which matches if one or more <c>ICriterinator</c> matches
    /// </summary>
    /// <typeparam name="T">Runtime type of items being compare</typeparam>
    public class OrGroup<T> : List<ICriterinator<T>>, ICriterinator<T> where T : IComparable<T>
    {
        /// <summary>
        /// Creates a new 'Or' grouping of <c>ICriterinator</c>s
        /// </summary>
        /// <param name="criterinators">The constituent <c>ICriterinator</c>s</param>
        public OrGroup(IEnumerable<ICriterinator<T>> criterinators) : base(criterinators)
        {
        }

        /// <summary>
        /// Creates a new 'Or' grouping of <c>ICriterinator</c>s
        /// </summary>
        /// <param name="criterinators">The constituent <c>ICriterinator</c>s</param>
        public OrGroup(params ICriterinator<T>[] criterinators) : base(criterinators)
        {
        }

        /// <summary>
        /// Returns <c>true</c> if any <c>ICriterinator</c> matches
        /// </summary>
        /// <param name="inputItem">An item to compare agains the criteria</param>
        /// <returns></returns>
        public bool IsMatch(T inputItem)
        {
            return this.Any(c =>  c.IsMatch(inputItem));
        }
    }
}
