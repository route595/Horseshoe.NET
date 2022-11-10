using System;

namespace Horseshoe.NET.Iterator.Memory
{
    public static class Extensions
    {
        /// <summary>
        /// Iterates over a span. <c>ControlInterface ci</c> contains properties and methods for interacting with the iterator.
        /// </summary>
        /// <typeparam name="T">Type of items</typeparam>
        /// <param name="span">A span</param>
        /// <param name="action">Action to perform on each iteration</param>
        public static void Iterate<T>(this ReadOnlySpan<T> span, Action<T, ControlInterface> action)
        {
            var ci = new ControlInterface { Count = span.Length };

            for (int i = 0; i < ci.Count; i++)
            {
                ci.Index = i;
                try
                {
                    action.Invoke(span[i], ci);
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
        public static void ReverseIterate<T>(this ReadOnlySpan<T> span, Action<T, ControlInterface> action)
        {
            var ci = new ControlInterface { Count = span.Length };
            for (int i = ci.Count - 1; i >= 0; i--)
            {
                ci.Index = i;
                try
                {
                    action.Invoke(span[i], ci);
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
