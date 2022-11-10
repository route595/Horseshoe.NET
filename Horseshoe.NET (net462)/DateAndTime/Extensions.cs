using System;

namespace Horseshoe.NET.DateAndTime
{
    public static class Extensions
    {
        /// <summary>
        /// Calculates age using an advancing month/year algorithm
        /// </summary>
        /// <param name="from">A from date</param>
        /// <param name="to">A to date</param>
        /// <param name="decimals">How many decimal places to display</param>
        /// <returns>A <c>YearSpan</c> instance</returns>
        public static YearSpan GetAge(this DateTime from, DateTime? to = null, int decimals = -1)
        {
            return new YearSpan(from, to ?? DateTime.Now, decimals: decimals);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public static int GetAgeInYears(this DateTime from, DateTime? to = null)
        {
            return new YearSpan(from, to ?? DateTime.Now).Years;
        }

        public static double GetTotalAgeInYears(this DateTime from, DateTime? to = null, int decimals = -1)
        {
            return new YearSpan(from, to ?? DateTime.Now, decimals: decimals).TotalYears;
        }

        public static int GetAgeInMonths(this DateTime from, DateTime? to = null)
        {
            var yearSpan = new YearSpan(from, to ?? DateTime.Now);
            return yearSpan.Years * 12 + yearSpan.Months;
        }

        public static double GetTotalAgeInMonths(this DateTime from, DateTime? to = null, int decimals = -1)
        {
            return new YearSpan(from, to ?? DateTime.Now, decimals: decimals).TotalMonths;
        }

        public static int GetAgeInDays(this DateTime from, DateTime? to = null)
        {
            return ((to ?? DateTime.Now) - from).Days;
        }

        public static double GetTotalAgeInDays(this DateTime from, DateTime? to = null, int decimals = -1)
        {
            var totalDays = ((to ?? DateTime.Now) - from).TotalDays;
            return decimals >= 0
                ? Math.Round(totalDays, decimals)
                : totalDays;
        }

        public static int GetNumberOfDaysInMonth(this DateTime date)
        {
            return DateUtil.GetNumberOfDaysInMonth(date.Year, date.Month);
        }

        public static bool IsInLeapYear(this DateTime date)
        {
            return DateUtil.IsLeapYear(date);
        }

        public static bool IsSameDay(this DateTime date, DateTime other)
        {
            return DateUtil.SameDay(date, other);
        }

        public static bool IsSameMonth(this DateTime date, DateTime other)
        {
            return DateUtil.SameMonth(date, other);
        }
    }
}
