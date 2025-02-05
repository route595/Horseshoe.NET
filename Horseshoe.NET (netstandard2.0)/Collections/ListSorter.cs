using System;
using System.Collections.Generic;
using System.Linq;

using Horseshoe.NET.RelayMessages;

namespace Horseshoe.NET.Collections
{
    /// <summary>
    /// Provides several options for auto-sorting items in a collection 
    /// </summary>
    /// <typeparam name="T">Collection type</typeparam>
    public class ListSorter<T> 
    {
        private static readonly string MessageRelayGroup = typeof(ListSorter<T>).Namespace;

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
        /// <returns>The sorted list or <c>null</c> if the supplied list is <c>null</c></returns>
        public List<T> Sort (List<T> list)
        {
            return CollectionUtil.ToList(Sort(list as IEnumerable<T>));
        }

        /// <summary>
        /// Sorts an <c>IEnumerable</c> based on the current sorter
        /// </summary>
        /// <param name="collection">A collection</param>
        /// <returns>The sorted collection or <c>null</c> if the supplied collection is <c>null</c></returns>
        public IEnumerable<T> Sort (IEnumerable<T> collection)
        {
            SystemMessageRelay.RelayMethodInfo(group: MessageRelayGroup);

            if (collection == null)
                return null;

            if (Sorter != null)
            {
                SystemMessageRelay.RelayMessage("using List.OrderBy(Func<T, IComparable>) \"Sorter\"", group: MessageRelayGroup);
                collection = collection
                    .OrderBy(Sorter);
            }
            else if (Comparer != null)
            {
                SystemMessageRelay.RelayMessage("using List.Sort(IComparer<T>) \"Comparer\"", group: MessageRelayGroup);
                if (!(collection is List<T> list))
                    list = new List<T>(collection);
                list.Sort(Comparer);
                collection = list;
            }
            else if (Comparison != null)
            {
                SystemMessageRelay.RelayMessage("using List.Sort(Comparison<T>) \"Comparison\"", group: MessageRelayGroup);
                if (!(collection is List<T> list))
                    list = new List<T>(collection);
                list.Sort(Comparison);
                collection = list;
            }
            else
            {
                SystemMessageRelay.RelayMessage("using List.Sort()", group: MessageRelayGroup);
                if (!(collection is List<T> list))
                    list = new List<T>(collection);
                list.Sort();
                collection = list;
            }

            SystemMessageRelay.RelayMethodReturn(returnDescription: "sorted collection", group: MessageRelayGroup);
            return collection;
        }
    }
}
