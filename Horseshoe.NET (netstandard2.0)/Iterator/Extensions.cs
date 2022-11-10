using System;
using System.Collections.Generic;
using System.Linq;

namespace Horseshoe.NET.Iterator
{
    public static class Extensions
    {
        /// <summary>
        /// Iterates over a collection. <c>ControlInterface ci</c> contains properties and methods for interacting with the iterator.
        /// controls for breaking and continuing.
        /// </summary>
        /// <typeparam name="T">Type of items</typeparam>
        /// <param name="collection">A collection</param>
        /// <param name="action">Action to perform on each iteration</param>
        public static void Iterate<T>(this IEnumerable<T> collection, Action<T, ControlInterface> action)
        {
            var ci = new ControlInterface { Count = collection.Count() };

            if (collection is IList<T> list)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    ci.Index = i;
                    try
                    {
                        action.Invoke(list[i], ci);
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
                    ci.Index = ++counter;
                    try
                    {
                        action.Invoke(t, ci);
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
        public static void ReverseIterate<T>(this IEnumerable<T> collection, Action<T, ControlInterface> action)
        {
            var list = collection is List<T> _list
                ? _list
                : new List<T>(collection ?? Enumerable.Empty<T>());
            ControlInterface ci = new ControlInterface { Count = list.Count };
            var counter = ci.Count;
            while (counter > 0)
            {
                ci.Index = --counter;
                try
                {
                    action.Invoke(list[counter], ci);
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
