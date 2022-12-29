using System;
using System.Collections.Generic;
using System.Linq;

namespace Horseshoe.NET.Iterator
{
    /// <summary>
    /// Extension methods for iterators over collections
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Iterates over a collection. <c>ControlInterface ci</c> contains properties and methods for interacting with the iterator.
        /// controls for breaking and continuing.
        /// </summary>
        /// <typeparam name="T">Type of items</typeparam>
        /// <param name="collection">A collection</param>
        /// <param name="action">Action to perform on each iteration</param>
        public static void Iterate<T>(this IEnumerable<T> collection, Action<T, IteratorMetadata> action)
        {
            var meta = new IteratorMetadata { Count = collection.Count() };

            if (collection is IList<T> list)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    meta.Index = i;
                    try
                    {
                        action.Invoke(list[i], meta);
                    }
                    catch (ContinueNextException)
                    {
                        continue;
                    }
                    catch (ExitIterationException)
                    {
                        break;
                    }
                }
            }
            else
            {
                var counter = -1;
                foreach (T t in collection)
                {
                    meta.Index = ++counter;
                    try
                    {
                        action.Invoke(t, meta);
                    }
                    catch (ContinueNextException)
                    {
                        continue;
                    }
                    catch (ExitIterationException)
                    {
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Iterates backwards over a collection. <c>ControlInterface ci</c> contains properties and methods for interacting with the iterator.
        /// controls for breaking and continuing.
        /// </summary>
        /// <typeparam name="T">Type of items</typeparam>
        /// <param name="collection">A collection</param>
        /// <param name="action">Action to perform on each iteration</param>
        public static void ReverseIterate<T>(this IEnumerable<T> collection, Action<T, IteratorMetadata> action)
        {
            var list = collection is List<T> _list
                ? _list
                : new List<T>(collection ?? Enumerable.Empty<T>());
            IteratorMetadata meta = new IteratorMetadata { Count = list.Count };
            var counter = meta.Count;
            while (counter > 0)
            {
                meta.Index = --counter;
                try
                {
                    action.Invoke(list[counter], meta);
                }
                catch (ContinueNextException)
                {
                    continue;
                }
                catch (ExitIterationException)
                {
                    break;
                }
            }
        }
    }
}
