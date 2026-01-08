using System;
using System.Collections.Generic;
using System.Linq;

namespace Horseshoe.NET.DateAndTime
{
    /// <summary>
    /// Date / time utility methods
    /// </summary>
    public static class DateUtil
    {
        /// <summary>
        /// Gets whether year referred to in <c>date</c> is a leap year
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static bool IsLeapYear(DateTime date)
        {
            return IsLeapYear(date.Year);
        }

        /// <summary>
        /// Gets whether the specified year is a leap year
        /// </summary>
        /// <param name="year"></param>
        /// <returns></returns>
        public static bool IsLeapYear(int year)
        {
            if (year % 400 == 0) return true;
            if (year % 100 == 0) return false;
            if (year % 4 == 0) return true;
            return false;
        }

        /// <summary>
        /// Gets how many leap days occur in the specified date range
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public static int CountLeapDaysBetween(DateTime from, DateTime to)
        {
            if (from > to)
            {
                var temp = from;
                from = to;
                to = temp;
            }
            to = new DateTime(to.Year, to.Month, 1);
            int leapDayCounter = 0;
            while (from < to)
            {
                if (IsLeapYear(from.Year))
                {
                    if (from.Month < 2)
                    {
                        from = from.AddMonths(1);
                        continue;
                    }
                    else if (from.Month == 2)
                    {
                        leapDayCounter++;
                    }
                }
                from = new DateTime(from.Year + 1, 1, 1);
            }
            return leapDayCounter;
        }

        /// <summary>
        /// Equality check of two dates comparing the year, month and day
        /// </summary>
        /// <param name="date">a date/time</param>
        /// <param name="other">another date/time</param>
        /// <returns></returns>
        public static bool SameDay(DateTime date, DateTime other)
        {
            return date.Year == other.Year && date.Month == other.Month && date.Day == other.Day;
        }

        /// <summary>
        /// Equality check of two dates comparing the year and month only
        /// </summary>
        /// <param name="date">a date/time</param>
        /// <param name="other">another date/time</param>
        /// <returns></returns>
        public static bool SameMonth(DateTime date, DateTime other)
        {
            return date.Year == other.Year && date.Month == other.Month;
        }

        /// <summary>
        /// Gets the number of days in the month and year referred to in <c>date</c>
        /// </summary>
        /// <param name="dateTime">a date/time</param>
        /// <returns></returns>
        public static int GetNumberOfDaysInMonth(DateTime dateTime)
        {
            return GetNumberOfDaysInMonth(dateTime.Year, dateTime.Month);
        }

        /// <summary>
        /// Gets the number of days in the specified month and year
        /// </summary>
        /// <param name="year">a year</param>
        /// <param name="month">a month</param>
        /// <returns></returns>
        public static int GetNumberOfDaysInMonth(int year, int month)
        {
            switch (month)
            {
                case 1:   // Jan
                case 3:   // Mar
                case 5:   // May
                case 7:   // Jul
                case 8:   // Aug
                case 10:  // Oct
                case 12:  // Dec
                default:
                    return 31;
                case 4:   // Apr
                case 6:   // Jun
                case 9:   // Sep
                case 11:  // Nov
                    return 30;
                case 2:   // Feb
                    return IsLeapYear(year) ? 29 : 28;
            }
        }

        /// <summary>
        /// Gets the <c>DateTime</c> representing the first day of the month of a supplied <c>DateTime</c>, if any, otherwise of the month of the current system date.
        /// </summary>
        /// <param name="basedOnDateTime">An optional <c>DateTime</c> upon which to base the result <c>DateTime</c>.</param>
        /// <returns>A <c>DateTime</c>.</returns>
        public static DateTime MonthStart(DateTime? basedOnDateTime = null)
        {
            if (!basedOnDateTime.HasValue)
            {
                basedOnDateTime = DateTime.Today;
            }
            return MonthStart(basedOnDateTime.Value.Year, basedOnDateTime.Value.Month);
        }

        /// <summary>
        /// Gets the <c>DateTime</c> representing the beginning of the specified year and month.
        /// </summary>
        /// <param name="year">A year</param>
        /// <param name="month">A month</param>
        /// <returns>A <c>DateTime</c>.</returns>
        public static DateTime MonthStart(int year, int month)
        {
            return new DateTime(year, month, 1);
        }

        /// <summary>
        /// Gets the <c>DateTime</c> representing the last day of the month of a supplied <c>DateTime</c>, if any, otherwise of the month of the current system date.
        /// </summary>
        /// <param name="basedOnDateTime">An optional <c>DateTime</c> upon which to base the result <c>DateTime</c>.</param>
        /// <returns>A <c>DateTime</c>.</returns>
        public static DateTime MonthEnd(DateTime? basedOnDateTime = null)
        {
            if (!basedOnDateTime.HasValue)
            {
                basedOnDateTime = DateTime.Today;
            }
            return MonthEnd(basedOnDateTime.Value.Year, basedOnDateTime.Value.Month);
        }

        /// <summary>
        /// Gets the <c>DateTime</c> representing the last day of the specified year and month.
        /// </summary>
        /// <param name="year">A year</param>
        /// <param name="month">A month</param>
        /// <returns>A <c>DateTime</c>.</returns>
        public static DateTime MonthEnd(int year, int month)
        {
            return new DateTime(year, month, GetNumberOfDaysInMonth(year, month));
        }

        /// <summary>
        /// Returns the earliest <see cref="DateTime"/> value from the specified date array.
        /// </summary>
        /// <remarks>
        /// Note, this method ignores date values equal to <c><see cref="DateTime.MinValue"/></c> and <c><see cref="DateTime.MaxValue"/></c>.
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
        /// Note, this method ignores date values equal to <c><see cref="DateTime.MinValue"/></c> and <c><see cref="DateTime.MaxValue"/></c>.
        /// </remarks>
        /// <param name="dates">A collection of <see cref="DateTime"/> values to evaluate.</param>
        /// <returns>
        /// The minimum <see cref="DateTime"/> value in the <paramref name="dates"/> collection, or 
        /// <c><see cref="DateTime.MinValue"/></c> if no dates were supplied.
        /// </returns>
        public static DateTime Min(IEnumerable<DateTime> dates)
        {
            if (dates == null)
                return DateTime.MinValue;

            return Min2(dates.Where(date => date != DateTime.MinValue && date != DateTime.MaxValue));
        }

        /// <summary>
        /// Returns the earliest <see cref="DateTime"/> value from the specified date array.
        /// </summary>
        /// <remarks>
        /// Note, this method does not ignore date values equal to <c><see cref="DateTime.MinValue"/></c> and <c><see cref="DateTime.MaxValue"/></c>.
        /// </remarks>
        /// <param name="dates">An array of <see cref="DateTime"/> values to evaluate.</param>
        /// <returns>
        /// The minimum <see cref="DateTime"/> value in the <paramref name="dates"/> array, or 
        /// <c><see cref="DateTime.MinValue"/></c> if no dates were supplied.
        /// </returns>
        public static DateTime Min2(params DateTime[] dates)
        {
            return Min2(dates.AsEnumerable());
        }

        /// <summary>
        /// Returns the earliest <see cref="DateTime"/> value from the specified date collection.
        /// </summary>
        /// <remarks>
        /// Note, this method does not ignore date values equal to <c><see cref="DateTime.MinValue"/></c> and <c><see cref="DateTime.MaxValue"/></c>.
        /// </remarks>
        /// <param name="dates">A collection of <see cref="DateTime"/> values to evaluate.</param>
        /// <returns>
        /// The minimum <see cref="DateTime"/> value in the <paramref name="dates"/> collection, or 
        /// <c><see cref="DateTime.MinValue"/></c> if no dates were supplied.
        /// </returns>
        public static DateTime Min2(IEnumerable<DateTime> dates)
        {
            if (dates == null)
                return DateTime.MinValue;

            DateTime? min = null;

            foreach (var date in dates)
            {
                if (min == null || date < min)
                    min = date;
            }
            return min ?? DateTime.MinValue;
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

            return Max2(dates.Where(date => date != DateTime.MinValue && date != DateTime.MaxValue));
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
