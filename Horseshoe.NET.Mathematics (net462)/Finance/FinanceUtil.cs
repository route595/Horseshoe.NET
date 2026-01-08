using System;
using System.Collections.Generic;
using System.Linq;

using static Horseshoe.NET.Guardrails;

namespace Horseshoe.NET.Mathematics.Finance
{
    public static class FinanceUtil
    {
        /// <summary>
        /// Returns the earliest <see cref="DateTime"/> value from the specified date array.
        /// </summary>
        /// <remarks>
        /// Note, this method ignores date values not between <c><see cref="MinDate"/></c> and <c><see cref="MaxDate"/></c>, exclusive.
        /// </remarks>
        /// <param name="dates">An array of <see cref="DateTime"/> values to evaluate.</param>
        /// <returns>
        /// The minimum <see cref="DateTime"/> value in the <paramref name="dates"/> array, or 
        /// <c><see cref="DateTime.MinValue"/></c> if no dates were supplied.
        /// </returns>
        public static DateTime Min(params DateTime[] dates)
        {
            return Min(dates.AsEnumerable());
        }

        /// <summary>
        /// Returns the earliest <see cref="DateTime"/> value from the specified date collection.
        /// </summary>
        /// <remarks>
        /// Note, this method ignores date values not between <c><see cref="MinDate"/></c> and <c><see cref="MaxDate"/></c>, exclusive.
        /// </remarks>
        /// <param name="dates">A collection of <see cref="DateTime"/> values to evaluate.</param>
        /// <returns>
        /// The minimum <see cref="DateTime"/> value in the <paramref name="dates"/> collection, or 
        /// <c><see cref="DateTime.MinValue"/></c> if no dates were supplied.
        /// </returns>
        public static DateTime Min(IEnumerable<DateTime> dates)
        {
            if (dates == null)
                return MinDate;

            DateTime? min = null;

            foreach (var date in dates)
            {
                if (date > MinDate && date < MaxDate && (min == null || date < min))
                    min = date;
            }
            return min ?? MinDate;
        }

        /// <summary>
        /// Returns the earliest <see cref="DateTime"/> value from the specified date array.
        /// </summary>
        /// <remarks>
        /// Note, this method does not ignore date values not between <c><see cref="MinDate"/></c> and <c><see cref="MaxDate"/></c>, exclusive.
        /// </remarks>
        /// <param name="dates">An array of <see cref="DateTime"/> values to evaluate.</param>
        /// <returns>
        /// The minimum <see cref="DateTime"/> value in the <paramref name="dates"/> array, or 
        /// <c><see cref="MinDate"/></c> if no dates were supplied.
        /// </returns>
        public static DateTime Min2(params DateTime[] dates)
        {
            return Min2(dates.AsEnumerable());
        }

        /// <summary>
        /// Returns the earliest <see cref="DateTime"/> value from the specified date collection.
        /// </summary>
        /// <remarks>
        /// Note, this method does not ignore date values not between <c><see cref="GuardLowDate"/></c> and <c><see cref="MaxDate"/></c>, exclusive.
        /// </remarks>
        /// <param name="dates">A collection of <see cref="DateTime"/> values to evaluate.</param>
        /// <returns>
        /// The minimum <see cref="DateTime"/> value in the <paramref name="dates"/> collection, or 
        /// <c><see cref="MinDate"/></c> if no dates were supplied.
        /// </returns>
        public static DateTime Min2(IEnumerable<DateTime> dates)
        {
            if (dates == null)
                return MinDate;

            DateTime? min = null;

            foreach (var date in dates)
            {
                if (min == null || date < min)
                    min = date;
            }
            return Wrangle(min);
        }

        /// <summary>
        /// Returns the latest <see cref="DateTime"/> value from the specified date array.
        /// </summary>
        /// <remarks>
        /// Note, this method ignores date values equal to <c><see cref="DateTime.MinValue"/></c> and <c><see cref="DateTime.MaxValue"/></c>.
        /// </remarks>
        /// <param name="dates">An array of <see cref="DateTime"/> values to evaluate.</param>
        /// <returns>
        /// The maximum <see cref="DateTime"/> value in the <paramref name="dates"/> array, or 
        /// <c><see cref="DateTime.MinValue"/></c> if no dates were supplied.
        /// </returns>
        public static DateTime Max(params DateTime[] dates)
        {
            return Max(dates.AsEnumerable());
        }

        /// <summary>
        /// Returns the latest <see cref="DateTime"/> value from the specified date collection.
        /// </summary>
        /// <remarks>
        /// Note, this method ignores date values equal to <c><see cref="DateTime.MinValue"/></c> and <c><see cref="DateTime.MaxValue"/></c>.
        /// </remarks>
        /// <param name="dates">A collection of <see cref="DateTime"/> values to evaluate.</param>
        /// <returns>
        /// The maximum <see cref="DateTime"/> value in the <paramref name="dates"/> collection, or 
        /// <c><see cref="DateTime.MinValue"/></c> if no dates were supplied.
        /// </returns>
        public static DateTime Max(IEnumerable<DateTime> dates)
        {
            if (dates == null)
                return DateTime.MinValue;

            DateTime? max = null;

            foreach (var date in dates)
            {
                if (date > DateTime.MinValue && date < DateTime.MaxValue && (max == null || date > max))
                    max = date;
            }
            return max ?? DateTime.MinValue;
        }

        /// <summary>
        /// Returns the latest <see cref="DateTime"/> value from the specified date array.
        /// </summary>
        /// <remarks>
        /// Note, this method does not ignore date values equal to <c><see cref="DateTime.MinValue"/></c> and <c><see cref="DateTime.MaxValue"/></c>.
        /// </remarks>
        /// <param name="dates">An array of <see cref="DateTime"/> values to evaluate.</param>
        /// <returns>
        /// The maximum <see cref="DateTime"/> value in the <paramref name="dates"/> array, or 
        /// <c><see cref="DateTime.MinValue"/></c> if no dates were supplied.
        /// </returns>
        public static DateTime Max2(params DateTime[] dates)
        {
            return Max2(dates.AsEnumerable());
        }

        /// <summary>
        /// Returns the latest <see cref="DateTime"/> value from the specified date collection.
        /// </summary>
        /// <remarks>
        /// Note, this method does not ignore date values equal to <c><see cref="DateTime.MinValue"/></c> and <c><see cref="DateTime.MaxValue"/></c>.
        /// </remarks>
        /// <param name="dates">A collection of <see cref="DateTime"/> values to evaluate.</param>
        /// <returns>
        /// The maximum <see cref="DateTime"/> value in the <paramref name="dates"/> collection, or 
        /// <c><see cref="DateTime.MinValue"/></c> if no dates were supplied.
        /// </returns>
        public static DateTime Max2(IEnumerable<DateTime> dates)
        {
            if (dates == null)
                return DateTime.MinValue;

            DateTime? max = null;

            foreach (var date in dates)
            {
                if (max == null || date > max)
                    max = date;
            }
            return max ?? DateTime.MinValue;
        }
    }
}
