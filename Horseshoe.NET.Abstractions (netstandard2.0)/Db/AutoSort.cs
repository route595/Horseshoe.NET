using System;
using System.Collections.Generic;

namespace Horseshoe.NET.Db
{
    /// <summary>
    /// Provides several options for auto-sorting parsed data from a query 
    /// </summary>
    /// <typeparam name="T">item type</typeparam>
    public class AutoSort<T>
    {
        /// <summary>
        /// Client-supplied sorter for <c>IComparable</c>s (see <c>IEnumerable&lt;T&gt;.OrderBy()</c>)
        /// </summary>
        public Func<T, IComparable> Sorter { get; }

        /// <summary>
        /// A <c>Comparer&lt;T&gt;</c> with which to sort (see <see cref="List{T}.Sort(IComparer{T})"/>)
        /// </summary>
        public IComparer<T> Comparer { get; }

        /// <summary>
        /// A <c>Comparison&lt;T&gt;</c> with which to sort (see <see cref="List{T}.Sort(Comparison{T})"/>)
        /// </summary>
        public Comparison<T> Comparison { get; }

        /// <summary>
        /// Creates a new <c>AutoSort</c>
        /// </summary>
        public AutoSort()
        {
        }

        /// <summary>
        /// Creates a new <c>AutoSort</c>
        /// </summary>
        /// <param name="sorter">a sortert function</param>
        public AutoSort(Func<T, IComparable> sorter)
        {
            Sorter = sorter;
        }

        /// <summary>
        /// Creates a new <c>AutoSort</c>
        /// </summary>
        /// <param name="comparer">An <c>IComparer&lt;T&gt;</c></param>
        public AutoSort(IComparer<T> comparer)
        {
            Comparer = comparer;
        }

        /// <summary>
        /// Creates a new <c>AutoSort</c>
        /// </summary>
        /// <param name="comparison">A <c>Comparison&lt;T&gt;</c></param>
        public AutoSort(Comparison<T> comparison)
        {
            Comparison = comparison;
        }
    }
}
