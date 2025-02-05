using System;

using Horseshoe.NET.Text;

namespace Horseshoe.NET.DateAndTime
{
    /// <summary>
    /// Date and time extension methods, some featuring <c>YearSpan</c> functionality.
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Calculates age using an advancing month/year algorithm.
        /// </summary>
        /// <param name="from">A from <c>DateTime</c>.</param>
        /// <param name="to">A to <c>DateTime</c>.</param>
        /// <param name="decimals">How many decimal places to display.</param>
        /// <returns>A <c>YearSpan</c> instance.</returns>
        public static YearSpan GetAge(this DateTime from, DateTime? to = null, int decimals = -1)
        {
            return new YearSpan(from, to ?? DateTime.Now, decimals: decimals);
        }

        /// <summary>
        /// Uses <c>YearSpan</c> to calculate an age rounded down to the year based on a 'from' and optional 'to' date.
        /// </summary>
        /// <param name="from">A from <c>DateTime</c>.</param>
        /// <param name="to">An optional to <c>DateTime</c>, if omitted will calculate the age as of right now</param>
        /// <returns>Number of whole years.</returns>
        public static int GetAgeInYears(this DateTime from, DateTime? to = null)
        {
            return new YearSpan(from, to ?? DateTime.Now).Years;
        }

        /// <summary>
        /// Uses <c>YearSpan</c> to calculate an exact age in years based on a 'from' and optional 'to' date.
        /// </summary>
        /// <param name="from">A from <c>DateTime</c>.</param>
        /// <param name="to">An optional to <c>DateTime</c>, if omitted will calculate the age as of right now</param>
        /// <param name="decimals">Optional number of rounding decimals.</param>
        /// <returns>Age in years with double precision.</returns>
        public static double GetTotalAgeInYears(this DateTime from, DateTime? to = null, int decimals = -1)
        {
            return new YearSpan(from, to ?? DateTime.Now, decimals: decimals).TotalYears;
        }


        /// <summary>
        /// Uses <c>YearSpan</c> to calculate an age rounded down to the month based on a 'from' and optional 'to' date.
        /// </summary>
        /// <param name="from">A from <c>DateTime</c>.</param>
        /// <param name="to">An optional to <c>DateTime</c>, if omitted will calculate the age as of right now</param>
        /// <returns>Number of whole months.</returns>
        public static int GetAgeInMonths(this DateTime from, DateTime? to = null)
        {
            var yearSpan = new YearSpan(from, to ?? DateTime.Now);
            return yearSpan.Years * 12 + yearSpan.Months;
        }

        /// <summary>
        /// Uses <c>YearSpan</c> to calculate an exact age in months based on a 'from' and optional 'to' date.
        /// </summary>
        /// <param name="from">A from <c>DateTime</c>.</param>
        /// <param name="to">An optional to <c>DateTime</c>, if omitted will calculate the age as of right now</param>
        /// <param name="decimals">Optional number of rounding decimals.</param>
        /// <returns>Age in months with double precision.</returns>
        public static double GetTotalAgeInMonths(this DateTime from, DateTime? to = null, int decimals = -1)
        {
            return new YearSpan(from, to ?? DateTime.Now, decimals: decimals).TotalMonths;
        }

        /// <summary>
        /// Calculates an age rounded down to the day based on a 'from' and optional 'to' date.
        /// </summary>
        /// <param name="from">A from <c>DateTime</c>.</param>
        /// <param name="to">An optional to <c>DateTime</c>, if omitted will calculate the age as of right now</param>
        /// <returns>Number of whole days.</returns>
        public static int GetAgeInDays(this DateTime from, DateTime? to = null)
        {
            return ((to ?? DateTime.Now) - from).Days;
        }

        /// <summary>
        /// Calculates an exact age in days based on a 'from' and optional 'to' date.
        /// </summary>
        /// <param name="from">A from <c>DateTime</c>.</param>
        /// <param name="to">An optional to <c>DateTime</c>, if omitted will calculate the age as of right now</param>
        /// <param name="decimals">Optional number of rounding decimals.</param>
        /// <returns>Age in days with double precision.</returns>
        public static double GetTotalAgeInDays(this DateTime from, DateTime? to = null, int decimals = -1)
        {
            var totalDays = ((to ?? DateTime.Now) - from).TotalDays;
            return decimals >= 0
                ? Math.Round(totalDays, decimals)
                : totalDays;
        }

        /// <summary>
        /// Gets the number of days in the month and year referred to in <c>date</c>.
        /// </summary>
        /// <param name="date"></param>
        /// <returns>Number of days.</returns>
        public static int GetNumberOfDaysInMonth(this DateTime date)
        {
            return DateUtil.GetNumberOfDaysInMonth(date.Year, date.Month);
        }

        /// <summary>
        /// Gets whether year referred to in <c>date</c> is a leap year.
        /// </summary>
        /// <param name="date">A date/time.</param>
        /// <returns><c>true</c> or <c>false</c></returns>
        public static bool IsInLeapYear(this DateTime date)
        {
            return DateUtil.IsLeapYear(date);
        }

        /// <summary>
        /// Equality check of two dates comparing the year, month and day.
        /// </summary>
        /// <param name="date">A date/time.</param>
        /// <param name="other">Another date/time.</param>
        /// <returns><c>true</c> or <c>false</c></returns>
        public static bool IsSameDay(this DateTime date, DateTime other)
        {
            return DateUtil.SameDay(date, other);
        }

        /// <summary>
        /// Equality check of two dates comparing the year and month only.
        /// </summary>
        /// <param name="date">A date/time.</param>
        /// <param name="other">Another date/time.</param>
        /// <returns><c>true</c> or <c>false</c></returns>
        public static bool IsSameMonth(this DateTime date, DateTime other)
        {
            return DateUtil.SameMonth(date, other);
        }

        /// <summary>
        /// Fast, flexible date formatting based on the current culture by default.
        /// </summary>
        /// <param name="date">A date/time.</param>
        /// <param name="provider">An optional format provider, e.g. <c>CultureInfo.GetCultureInfo("en-US")</c>.</param>
        /// <param name="locale">An optional locale (e.g. "en-US"), this is only used if <c>provicer == null</c>.</param>
        /// <returns>A formatted date/time.</returns>
        public static string ToFlexDateString(this DateTime date, IFormatProvider provider = null, string locale = null)
        {
            provider = TextUtil.GetProvider(provider, locale);
            return provider != null ? date.ToString("d", provider) : date.ToString("d");
        }

        /// <summary>
        /// Fast, flexible time formatting based on the current culture by default.  
        /// Does not display milliseconds and shortens the output to just hours and minutes if seconds == 0.
        /// </summary>
        /// <param name="date">A date/time.</param>
        /// <param name="provider">An optional format provider, e.g. <c>CultureInfo.GetCultureInfo("en-US")</c>.</param>
        /// <param name="locale">An optional locale (e.g. "en-US"), this is only used if <c>provicer == null</c>.</param>
        /// <returns>A formatted date/time.</returns>
        public static string ToFlexTimeString(this DateTime date, IFormatProvider provider = null, string locale = null)
        {
            provider = TextUtil.GetProvider(provider, locale);
            if (date.Second > 0)
                return provider != null ? date.ToString("T", provider) : date.ToString("T");
            return provider != null ? date.ToString("t", provider) : date.ToString("t");
        }

        /// <summary>
        /// Fast, flexible date/time formatting based on the current culture by default.  
        /// Does not display milliseconds and shortens the output to just hours and minutes if seconds 
        /// equals 0 (zero) or just the date if the hours and minutes also equal 0 (zero).
        /// </summary>
        /// <param name="date">A date/time.</param>
        /// <param name="provider">An optional format provider, e.g. <c>CultureInfo.GetCultureInfo("en-US")</c>.</param>
        /// <param name="locale">An optional locale (e.g. "en-US"), this is only used if <c>provicer == null</c>.</param>
        /// <returns>A formatted date/time.</returns>
        public static string ToFlexDateTimeString(this DateTime date, IFormatProvider provider = null, string locale = null)
        {
            provider = TextUtil.GetProvider(provider, locale);
            if (date.Second > 0)
                return provider != null ? date.ToString("G", provider) : date.ToString("G");
            if (date.Minute > 0 || date.Hour > 0)
                return provider != null ? date.ToString("g", provider) : date.ToString("g");
            return ToFlexDateString(date, provider: provider);
        }

        /// <summary>
        /// Returns <c>true</c> if at least one of the hour, minute, second or millisecond values of <c>date</c> is greater than 0
        /// </summary>
        /// <param name="date">A date/time</param>
        /// <returns><c>true</c> or <c>false</c></returns>
        public static bool HasTime(this DateTime date)
        {
            return !(date.Hour == 0 && date.Minute == 0 && date.Second == 0 && date.Millisecond == 0);
        }
    }
}
