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
            list = CollectionUtilAbstractions.AsList(_Sort(list, journal: journal));

            // finalize
            journal.Level--;
            return list;
        }

        /// <summary>
        /// Sorts an <c>IEnumerable</c> based on the current sorter
        /// </summary>
        /// <param name="collection">A collection</param>
        /// <param name="journal">An optional trace journal</param>
        /// <returns>The sorted collection or <c>null</c> if the supplied collection is <c>null</c></returns>
        public IEnumerable<T> _Sort
        (
            IEnumerable<T> collection,
            TraceJournal journal = null
        )
        {
            // journaling
            journal = journal ?? new TraceJournal();
            journal.WriteEntry("ListSorter.Sort()");
            journal.Level++;

            // do stuff
            if (collection == null)
            {
                journal.WriteEntry("collection is null");
                journal.Level--;
                return null;
            }

            if (Sorter != null)
            {
                journal.WriteEntry("using List.OrderBy(Func<T, IComparable>) \"Sorter\"");
                collection = collection
                    .OrderBy(Sorter);
            }
            else if (Comparer != null)
            {
                journal.WriteEntry("using List.Sort(IComparer<T>) \"Comparer\"");
                if (!(collection is List<T> list))
                    list = new List<T>(collection);
                list.Sort(Comparer);
                collection = list;
            }
            else if (Comparison != null)
            {
                journal.WriteEntry("using List.Sort(Comparison<T>) \"Comparison\"");
                if (!(collection is List<T> list))
                    list = new List<T>(collection);
                list.Sort(Comparison);
                collection = list;
            }
            else
            {
                journal.WriteEntry("using List.Sort()");
                if (!(collection is List<T> list))
                    list = new List<T>(collection);
                list.Sort();
                collection = list;
            }

            // finalize
            journal.Level--;
            return collection;
        }
    }
}
