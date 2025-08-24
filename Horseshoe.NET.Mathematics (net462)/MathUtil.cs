using System;
using System.Collections.Generic;

namespace Horseshoe.NET.Mathematics
{
    /// <summary>
    /// A suite of math utility methods, some being extensions of the ones built into .NET
    /// </summary>
    public static class MathUtil
    {
        /// <summary>
        /// Returns the largest argument (extends <c>Math.Max()</c> which only accepts two arguments).
        /// </summary>
        /// <param name="values">A parameter array of values to compare</param>
        public static int Max(params int[] values)
        {
            return Max(values as IEnumerable<int>);
        }

        /// <summary>
        /// Returns the largest item in the collection (extends <c>Math.Max()</c> which only accepts two arguments).
        /// </summary>
        /// <param name="values">A collection of values to compare</param>
        public static int Max(IEnumerable<int> values)
        {
            int max = 0;

            if (values != null)
            {
                var enumerator = values.GetEnumerator();
                if (enumerator.MoveNext())
                {
                    max = enumerator.Current;
                    while (enumerator.MoveNext())
                        max = Math.Max(max, enumerator.Current);
                }
            }

            return max;
        }

        /// <summary>
        /// Returns the largest argument (extends <c>Math.Max()</c> which only accepts two arguments).
        /// </summary>
        /// <param name="values">A parameter array of values to compare</param>
        public static double Max(params double[] values)
        {
            return Max(values as IEnumerable<double>);
        }

        /// <summary>
        /// Returns the largest item in the collection (extends <c>Math.Max()</c> which only accepts two arguments).
        /// </summary>
        /// <param name="values">A collection of values to compare</param>
        public static double Max(IEnumerable<double> values)
        {
            double max = 0.0;

            if (values != null)
            {
                var enumerator = values.GetEnumerator();
                if (enumerator.MoveNext())
                {
                    max = enumerator.Current;
                    while (enumerator.MoveNext())
                        max = Math.Max(max, enumerator.Current);
                }
            }

            return max;
        }

        /// <summary>
        /// Returns the largest argument (extends <c>Math.Max()</c> which only accepts two arguments).
        /// </summary>
        /// <param name="values">A parameter array of values to compare</param>
        public static decimal Max(params decimal[] values)
        {
            return Max(values as IEnumerable<decimal>);
        }

        /// <summary>
        /// Returns the largest item in the collection (extends <c>Math.Max()</c> which only accepts two arguments).
        /// </summary>
        /// <param name="values">A collection of values to compare</param>
        public static decimal Max(IEnumerable<decimal> values)
        {
            decimal max = 0.0m;

            if (values != null)
            {
                var enumerator = values.GetEnumerator();
                if (enumerator.MoveNext())
                {
                    max = enumerator.Current;
                    while (enumerator.MoveNext())
                        max = Math.Max(max, enumerator.Current);
                }
            }

            return max;
        }

        /// <summary>
        /// Returns the smallest argument (extends <c>Math.Max()</c> which only accepts two arguments).
        /// </summary>
        /// <param name="values">A parameter array of values to compare</param>
        public static int Min(params int[] values)
        {
            return Min(values as IEnumerable<int>);
        }

        /// <summary>
        /// Returns the smallest item in the collection (extends <c>Math.Max()</c> which only accepts two arguments).
        /// </summary>
        /// <param name="values">A collection of values to compare</param>
        public static int Min(IEnumerable<int> values)
        {
            int min = 0;

            if (values != null)
            {
                var enumerator = values.GetEnumerator();
                if (enumerator.MoveNext())
                {
                    min = enumerator.Current;
                    while (enumerator.MoveNext())
                        min = Math.Min(min, enumerator.Current);
                }
            }

            return min;
        }

        /// <summary>
        /// Returns the smallest argument (extends <c>Math.Max()</c> which only accepts two arguments).
        /// </summary>
        /// <param name="values">A parameter array of values to compare</param>
        public static double Min(params double[] values)
        {
            return Min(values as IEnumerable<double>);
        }

        /// <summary>
        /// Returns the smallest item in the collection (extends <c>Math.Max()</c> which only accepts two arguments).
        /// </summary>
        /// <param name="values">A collection of values to compare</param>
        public static double Min(IEnumerable<double> values)
        {
            double min = 0.0;

            if (values != null)
            {
                var enumerator = values.GetEnumerator();
                if (enumerator.MoveNext())
                {
                    min = enumerator.Current;
                    while (enumerator.MoveNext())
                        min = Math.Min(min, enumerator.Current);
                }
            }

            return min;
        }

        /// <summary>
        /// Returns the smallest argument (extends <c>Math.Max()</c> which only accepts two arguments).
        /// </summary>
        /// <param name="values">A parameter array of values to compare</param>
        public static decimal Min(params decimal[] values)
        {
            return Min(values as IEnumerable<decimal>);
        }

        /// <summary>
        /// Returns the smallest item in the collection (extends <c>Math.Max()</c> which only accepts two arguments).
        /// </summary>
        /// <param name="values">A collection of values to compare</param>
        public static decimal Min(IEnumerable<decimal> values)
        {
            decimal min = 0.0m;

            if (values != null)
            {
                var enumerator = values.GetEnumerator();
                if (enumerator.MoveNext())
                {
                    min = enumerator.Current;
                    while (enumerator.MoveNext())
                        min = Math.Min(min, enumerator.Current);
                }
            }

            return min;
        }
    }
}
