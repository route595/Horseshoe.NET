using System;
using System.Collections.Generic;
using System.Linq;

using Horseshoe.NET.DateAndTime;

namespace Horseshoe.NET
{
    /// <summary>
    /// Provides utility methods and constants for working with date values within predefined boundaries.
    /// </summary>
    public static class Guardrails
    {
        /// <summary>
        /// A low date boundary.
        /// </summary>
        public static DateTime MinDate => new DateTime(1900, 01, 01);

        /// <summary>
        /// A high date boundary.
        /// </summary>
        public static DateTime MaxDate => new DateTime(2190, 12, 31);

        /// <summary>
        /// Brings a date/time value within predefined low and high date boundaries.
        /// Values below <see cref="MinDate"/> (including <see cref="DateTime.MinValue"/>) and <c>null</c> are replaced with <see cref="MinDate"/>.
        /// Values above <see cref="MaxDate"/> (including <see cref="DateTime.MaxValue"/>) are replaced with <see cref="MaxDate"/>.
        /// </summary>
        /// <param name="date">A <c>nullable</c> date/time value</param>
        /// <param name="defaultToMax">If <c>true</c>, converts <c>null</c> and <c>DateTime.MinValue</c> to <see cref="MaxDate"/> instead of <see cref="MinDate"/>.  Default is <c>false</c>.</param>
        /// <param name="strict">If <c>true</c>, throws an exception if <c>date</c> falls outside of the predefined date boundaries not including <c>null</c>, <see cref="DateTime.MinValue"/> or <see cref="DateTime.MaxValue"/></param>
        public static DateTime Wrangle(DateTime? date, bool defaultToMax = false, bool strict = false)
        {
            if (date == null)
                return defaultToMax ? MaxDate : MinDate;

            if (date < MinDate)
            {
                if (date == DateTime.MinValue)
                    return defaultToMax ? MaxDate : MinDate;
                else if (strict)
                    throw new ArgumentOutOfRangeException(nameof(date), $"Date {date} is outside the predefined low date boundary: {MinDate:d}.");
                return MinDate;
            }

            if (date > MaxDate)
            {
                if (date == DateTime.MaxValue)
                    return MaxDate;
                else if (strict)
                    throw new ArgumentOutOfRangeException(nameof(date), $"Date {date} is outside the predefined high date boundary: {MaxDate:d}.");
                return MinDate;
            }

            return date.Value;
        }

        /// <summary>
        /// Returns the earliest <see cref="DateTime"/> value from the specified date array (wrangled to dates within predefined low and high date boundaries).
        /// </summary>
        /// <param name="dates">An array of <see cref="DateTime"/> values to evaluate.</param>
        /// <returns>
        /// The minimum <see cref="DateTime"/> value in the <paramref name="dates"/> array, or 
        /// <c><see cref="MinDate"/></c> if no dates were supplied.
        /// </returns>
        public static DateTime Min(params DateTime[] dates)
        {
            return Min(dates.AsEnumerable());
        }

        /// <summary>
        /// Returns the earliest <see cref="DateTime"/> value from the specified date collection (wrangled to dates within predefined low and high date boundaries).
        /// </summary>
        /// <param name="dates">A collection of <see cref="DateTime"/> values to evaluate.</param>
        /// <returns>
        /// The minimum <see cref="DateTime"/> value in the <paramref name="dates"/> collection, or 
        /// <c><see cref="MinDate"/></c> if no dates were supplied.
        /// </returns>
        public static DateTime Min(IEnumerable<DateTime> dates)
        {
            if (dates == null)
                return MinDate;

            return Wrangle(DateUtil.Min2(dates.Select(date => Wrangle(date))));
        }

        /// <summary>
        /// Returns the earliest <see cref="DateTime"/> value from the specified date array (wrangled to dates within predefined low and high date boundaries).
        /// </summary>
        /// <param name="dates">An array of <see cref="DateTime"/> values to evaluate.</param>
        /// <returns>
        /// The minimum <see cref="DateTime"/> value in the <paramref name="dates"/> array, or 
        /// <c><see cref="MinDate"/></c> if no dates were supplied.
        /// </returns>
        public static DateTime Max(params DateTime[] dates)
        {
            return Max(dates.AsEnumerable());
        }

        /// <summary>
        /// Returns the earliest <see cref="DateTime"/> value from the specified date collection (wrangled to dates within predefined low and high date boundaries).
        /// </summary>
        /// <param name="dates">A collection of <see cref="DateTime"/> values to evaluate.</param>
        /// <returns>
        /// The minimum <see cref="DateTime"/> value in the <paramref name="dates"/> collection, or 
        /// <c><see cref="MinDate"/></c> if no dates were supplied.
        /// </returns>
        public static DateTime Max(IEnumerable<DateTime> dates)
        {
            if (dates == null)
                return MinDate;

            return Wrangle(DateUtil.Max2(dates.Select(date => Wrangle(date))));
        }
    }
}
