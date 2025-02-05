using System.Collections.Generic;
using System.Linq;

namespace Horseshoe.NET.FrontLoader
{
    /// <summary>
    /// Front loads settings for future <c>object</c> comparison
    /// </summary>
    /// <typeparam name="T">Type of object</typeparam>
    public class ValueFrontLoader<T>
    {
        /// <summary>
        /// An <c>object</c> upon which to perform future comparisons
        /// </summary>
        public T Value { get; set; }

        /// <summary>
        /// An optional <c>IEqualityComparer&lt;T&gt;</c>
        /// </summary>
        public IEqualityComparer<T> Comparer { get; set; }

        ///// <summary>
        ///// Inspired by SQL, checks to see if an item equal to the front loaded value is found in the argument item collection
        ///// </summary>
        ///// <param name="items">A collection of items to search</param>
        ///// <returns><c>true</c> or <c>false</c></returns>
        //public bool In(params T[] items)
        //{
        //    return In(items as IEnumerable<T>);
        //}

        ///// <summary>
        ///// Inspired by SQL, checks to see if an item equal to the front loaded value is found in the argument item collection
        ///// </summary>
        ///// <param name="items">A collection of items to search</param>
        ///// <returns><c>true</c> or <c>false</c></returns>
        //public bool In(IEnumerable<T> items)
        //{
        //    if (items == null || !items.Any())
        //        return false;
        //    return items.Contains(Value, Comparer);
        //}
    }
}
