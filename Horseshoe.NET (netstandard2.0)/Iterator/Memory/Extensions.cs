using System;

namespace Horseshoe.NET.Iterator.Memory
{
    /// <summary>
    /// Extension methods for memory efficient iterators over <c>ReadOnlySpan</c>
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Iterates over a <c>ReadOnlySpan</c>. <c>ControlInterface ci</c> contains properties and methods for interacting with the iterator.
        /// </summary>
        /// <typeparam name="T">Type of items</typeparam>
        /// <param name="span">A <c>ReadOnlySpan</c></param>
        /// <param name="action">Action to perform on each iteration</param>
        public static void Iterate<T>(this ReadOnlySpan<T> span, Action<T, IteratorMetadata> action)
        {
            var meta = new IteratorMetadata { Count = span.Length };

            for (int i = 0; i < meta.Count; i++)
            {
                meta.Index = i;
                try
                {
                    action.Invoke(span[i], meta);
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

        /// <summary>
        /// Iterates backwards over a span. <c>ControlInterface ci</c> contains properties and methods for interacting with the iterator.
        /// </summary>
        /// <typeparam name="T">Type of items</typeparam>
        /// <param name="span">A span</param>
        /// <param name="action">Action to perform on each iteration</param>
        public static void ReverseIterate<T>(this ReadOnlySpan<T> span, Action<T, IteratorMetadata> action)
        {
            var meta = new IteratorMetadata { Count = span.Length };
            for (int i = meta.Count - 1; i >= 0; i--)
            {
                meta.Index = i;
                try
                {
                    action.Invoke(span[i], meta);
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
