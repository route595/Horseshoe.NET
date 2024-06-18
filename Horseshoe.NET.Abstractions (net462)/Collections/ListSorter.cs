using System;
using System.Collections.Generic;
using System.Linq;

namespace Horseshoe.NET.Collections
{
    /// <summary>
    /// Provides several options for auto-sorting parsed data from a query 
    /// </summary>
    /// <typeparam name="T">item type</typeparam>
    public class ListSorter<T> 
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
        /// Creates a new <c>ListSorter</c>
        /// </summary>
        public ListSorter()
        {
        }

        /// <summary>
        /// Creates a new <c>ListSorter</c>
        /// </summary>
        /// <param name="sorter">a sorter function</param>
        public ListSorter(Func<T, IComparable> sorter)
        {
            Sorter = sorter;
        }

        /// <summary>
        /// Creates a new <c>ListSorter</c>
        /// </summary>
        /// <param name="comparer">An <c>IComparer&lt;T&gt;</c></param>
        public ListSorter(IComparer<T> comparer)
        {
            Comparer = comparer;
        }

        /// <summary>
        /// Creates a new <c>ListSorter</c>
        /// </summary>
        /// <param name="comparison">A <c>Comparison&lt;T&gt;</c></param>
        public ListSorter(Comparison<T> comparison)
        {
            Comparison = comparison;
        }

        /// <summary>
        /// Sorts a <c>List</c> based on the current sorter
        /// </summary>
        /// <param name="list">A list</param>
        /// <param name="journal">An optional trace journal</param>
        /// <returns>The sorted list or <c>null</c> if the supplied list is <c>null</c></returns>
        public List<T> Sort
        (
            List<T> list,
            TraceJournal journal = null
        )
        {
            // journaling
            journal = journal ?? new TraceJournal();
            journal.WriteEntry("ListSorter.Sort()");
            journal.Level++;

            // do stuff
            if (list == null)
            {
                journal.WriteEntry("list is null");
                journal.Level--;
                return null;
            }

            if (Sorter != null)
            {
                journal.WriteEntry("using List.OrderBy(Func<T, IComparable>) \"Sorter\"");
                list = list
                    .OrderBy(Sorter)
                    .ToList();
            }
            else if (Comparer != null)
            {
                journal.WriteEntry("using List.Sort(IComparer<T>) \"Comparer\"");
                list.Sort(Comparer);
            }
            else if (Comparison != null)
            {
                journal.WriteEntry("using List.Sort(Comparison<T>) \"Comparison\"");
                list.Sort(Comparison);
            }
            else
            {
                journal.WriteEntry("using List.Sort()");
                list.Sort();
            }

            // finalize
            journal.Level--;
            return list;
        }
    }
}
