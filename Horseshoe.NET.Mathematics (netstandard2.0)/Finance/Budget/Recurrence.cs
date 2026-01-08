using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using Horseshoe.NET.DateAndTime;
using static Horseshoe.NET.Guardrails;
using Horseshoe.NET.ObjectsTypesAndValues;
using Horseshoe.NET.Text;

namespace Horseshoe.NET.Mathematics.Finance.Budget
{
    /// <summary>
    /// Represents a pattern of repeating transactions over time for budgeting purposes.
    /// </summary>
    /// <remarks>
    /// <see cref="Recurrence"/> supports two primary types of recurrence: 
    /// <list type="bullet">
    /// <item>
    /// <description>
    /// <b>Periodic Recurrence:</b>
    /// Transactions occurring at regular intervals, such as every X days, weeks, months, or years.
    /// </description>
    /// </item>
    /// <item>
    /// <description>
    /// <b>Days-of-Month Recurrence:</b> 
    /// Transactions occurring on specific days of the month, such as the 1st and 15th, or the last day of the month.
    /// </description>
    /// </item>
    /// </list>
    /// </remarks>
    public struct Recurrence
    {
        public RecurrenceType Type { get; }

        public DateTime StartDate { get; }

        public DateTime EndDate { get; }

        // properties for Periodic recurrence
        public RecurrenceInterval Interval { get; set; }

        public int X { get; }

        public int Month { get; }  // relevant only for yearly recurrences

        public int Day { get; }    // relevant only for monthly / yearly recurrences

        // properties for DaysOfMonth recurrence
        public int[] DaysOfMonth { get; }

        /// <summary>
        /// Creates a days-of-month recurrence.
        /// </summary>
        /// <param name="daysOfMonth"></param>
        private Recurrence(int[] daysOfMonth, DateTime? startDate = null, DateTime? endDate = null)
        {
            Type = RecurrenceType.DaysOfMonth;
            StartDate = Wrangle(startDate);
            EndDate = Wrangle(endDate, defaultToMax: true);
            Interval = default;
            X = default;
            Month = default;
            Day = default;
            DaysOfMonth = daysOfMonth;
        }

        /// <summary>
        /// Creates a periodic recurrence
        /// </summary>
        /// <param name="interval"></param>
        /// <param name="x"></param>
        /// <param name="month"></param>
        /// <param name="day"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        public Recurrence(RecurrenceInterval interval, int x, int month, int day, DateTime? startDate = null, DateTime? endDate = null)
        {
            Type = RecurrenceType.Periodic;
            StartDate = Wrangle(startDate);
            EndDate = Wrangle(endDate, defaultToMax: true);
            Interval = interval;
            X = x;
            Month = month;
            Day = day;
            DaysOfMonth = null;
        }

        /// <summary>
        /// Creates a month/day yearly recurrence
        /// </summary>
        /// <param name="month"></param>
        /// <param name="day"></param>
        public Recurrence(int month, int day, DateTime? startDate = null, DateTime? endDate = null)
        {
            Type = RecurrenceType.Periodic;
            StartDate = Wrangle(startDate);
            EndDate = Wrangle(endDate, defaultToMax: true);
            Interval = RecurrenceInterval.Every_X_Years;
            X = 1;
            Month = month;
            Day = day;
            DaysOfMonth = null;
        }

        /// <summary>
        /// Add start and end dates to a recurrence.  Only has effect for periodic recurrences.
        /// </summary>
        /// <param name="startDate">Start date</param>
        /// <param name="endDate">End date</param>
        /// <returns></returns>
        public Recurrence SetDateRange(DateTime? startDate = null, DateTime? endDate = null)
        {
            return new Recurrence(Interval, X, Month, Day, startDate, endDate);
        }

        public DateTime Next(DateTime date)
        {
            switch (Type)
            {
                case RecurrenceType.NoRecurrence:
                default:
                    return MaxDate;

                case RecurrenceType.Periodic:
                    if (X < 1)
                        return MaxDate;
                    switch (Interval)
                    {
                        case RecurrenceInterval.Every_X_Days:
                        default:
                            return date.AddDays(X);
                        case RecurrenceInterval.Every_X_Weeks:
                            return date.AddDays(X * 7);
                        case RecurrenceInterval.Every_X_Months:
                            return date.AddMonths(X);
                        case RecurrenceInterval.Every_X_Quarters:
                            return date.AddMonths(X * 3);
                        case RecurrenceInterval.Every_X_Years:
                            return date.AddYears(X);
                    }

                case RecurrenceType.DaysOfMonth:
                    // handle invalid set
                    if (DaysOfMonth == null || DaysOfMonth.Length == 0)
                        return MaxDate;

                    // scan subsequent dates until match found
                    date = date.AddDays(1);
                    while (true)
                    {
                        // handle exact day match
                        if (DaysOfMonth.Contains(date.Day))
                            return date;

                        // handle last-day-of-month-based match e.g. 0 = last, -1 = second to last, etc.
                        if (DaysOfMonth.Any(day => day < 1 && date == date.MonthStart().AddMonths(1).AddDays(day - 1)))
                            return date;

                        // if no match move to next day
                        date = date.AddDays(1);
                    }
            }
        }

        public IEnumerable<DateTime> GetAllOccurrances(DateTime windowStartDate, DateTime windowEndDate)
        {
            var occurrances = new List<DateTime>();
            DateTime date;
            switch (Type)
            {

                case RecurrenceType.Periodic:
                    date = StartDate == MinDate && Month > 0
                        ? new DateTime(windowStartDate.Year, Month, Day)
                        : StartDate;
                    while (date <= windowEndDate)
                    {
                        if (date >= windowStartDate)
                        {
                            occurrances.Add(date);
                        }

                        date = Next(date);
                    }
                    break;

                case RecurrenceType.DaysOfMonth:
                    date = windowStartDate;

                    while (date <= windowEndDate)
                    {
                        // match exact day
                        if (DaysOfMonth.Contains(date.Day))
                        {
                            occurrances.Add(date);
                        }

                        // match special value representing last day of month
                        else if (DaysOfMonth.Any(day => day < 1 && date == date.MonthStart().AddMonths(1).AddDays(day - 1)))
                        {
                            occurrances.Add(date);
                        }

                        date = date.AddDays(1);
                    }
                    break;

                case RecurrenceType.NoRecurrence:
                default:
                    break;
            }
            return occurrances;
        }

        public override string ToString()
        {
            string[] propertiesToInclude = null;
            string[] propertiesToExclude = null;
            switch (Type)
            {
                case RecurrenceType.NoRecurrence:
                default:
                    propertiesToInclude = new[] { nameof(Type) }; break;
                case RecurrenceType.Periodic:
                    propertiesToExclude = new[] { nameof(DaysOfMonth) }; break;
                case RecurrenceType.DaysOfMonth:
                    propertiesToExclude = new[] { nameof(X), nameof(Month), nameof(Day) }; break;
            }
            return 
                typeof(Recurrence).Name + ": " + 
                ObjectUtil.DumpToString
                (
                    this, 
                    propertiesToInclude: propertiesToInclude, 
                    propertiesToExclude: propertiesToExclude, 
                    excludeAdditionalPropertiesIf: (prop, value) => value is DateTime dateTimeValue && dateTimeValue == MinDate
                );
        }

        /// <summary>
        /// Text pattern representing periodic recurrence based on start date.
        /// </summary>
        /// <remarks>
        /// Examples:
        /// <list type="bullet">
        /// <item><strong>y</strong> / <strong>2y</strong> - yearly / bienially</item>
        /// <item><strong>q</strong> - quarterly</item>
        /// <item><strong>m</strong> / <strong>2m</strong> - monthly / every 2 months</item>
        /// <item><strong>w</strong> / <strong>2w</strong> - weekly / every 2 weeks</item>
        /// <item><strong>d</strong> / <strong>14d</strong> - daily / every 14 days</item>
        /// </list>
        /// </remarks>
        public static Regex PeriodicPattern { get; } = new Regex("^[0-9]*[dwmqy]$");

        // e.g. "bimonthly", "last", "5", "1,15", "15,last", "15,{month.length-1}", etc.
        public static Regex DaysOfMonthPattern { get; } = new Regex("^(bimonthly|((([1-9]|[1-2][0-9]|3[0-1]),)*([1-9]|[1-2][0-9]|3[0-1]|last|(\\{[a-z.]+(-[1-9])?\\}))))$");

        // same day of every month based on StartDate e.g. "8/5" (every year on Aug 5), "2/last" (every year on the last day of February)
        public static Regex MonthDayPattern { get; } = new Regex("^(([1-9]|1[0-2])\\/([1-9]|[1-2][0-9]|3[0-1]|last|(\\{[a-z.]+(-[1-9])?\\})))$");

        public static DateTime InferDate(int year, int month, int day)
        {
            if (day < 1)
            {
                var date = DateUtil.MonthEnd(year, month);
                if (day < 0)
                {
                    date = date.AddDays(day);
                }
                return date;
            }
            return new DateTime(year, month, day);
        }

        /// <summary>
        /// Extrapolates a <see cref="Recurrence"/> from the supplied notation.
        /// </summary>
        /// <param name="input">Text representing the recurrence.</param>
        /// <param name="startDate">An optional recurrence start date.</param>
        /// <param name="endDate">An optional recurrence end date.</param>
        /// <remarks>
        /// Examples of recurrence notation:
        /// <list type="bullet">
        /// <item>
        /// <strong>2w</strong> / <strong>14d</strong> = every 2 weeks / 14 days
        /// </item>
        /// <item>
        /// <strong>q</strong> / <strong>y</strong> or <strong>1y</strong> = quarterly / yearly
        /// </item>
        /// <item>
        /// <strong>5</strong> / <strong>last</strong> / <strong>{month.length-1}</strong> = on the 5th / last / second to last day of every month
        /// </item>
        /// <item>
        /// <strong>1,15</strong> or <strong>bimonthly</strong> / <strong>15,last</strong> / <strong>15,{month.length-1}</strong> = on the 1st and 15th / 15th and last / 15th and second to last day of every month
        /// </item>
        /// </list>
        /// </remarks>
        public static Recurrence Parse(string input, DateTime? startDate = null, DateTime? endDate = null)
        {
            TextAttribute.TryParseStartsWith(input, out TextAttribute.List list, out var textRemainder);
            input = textRemainder;

            Match match = PeriodicPattern.Match(input);
            if (match.Success)
            {
                RecurrenceInterval interval;
                int x = match.Value.Length > 1
                    ? Convert.ToInt32(match.Value.Substring(0, match.Value.Length - 1))
                    : 1;
                switch (match.Value.Last())
                {
                    case 'd':
                    default:
                        interval = RecurrenceInterval.Every_X_Days;
                        break;
                    case 'w':
                        interval = RecurrenceInterval.Every_X_Weeks;
                        break;
                    case 'm':
                        interval = RecurrenceInterval.Every_X_Months;
                        break;
                    case 'q':
                        interval = RecurrenceInterval.Every_X_Quarters;
                        break;
                    case 'y':
                        interval = RecurrenceInterval.Every_X_Years;
                        break;
                }

                // periodic recurrence
                return new Recurrence
                (
                    interval, 
                    x, 
                    0, 
                    0, 
                    startDate ?? list?.GetValue<DateTime>("startDate"), 
                    endDate ?? list?.GetValue<DateTime>("endDate")
                );
            }

            match = DaysOfMonthPattern.Match(input);
            if (match.Success)
            {
                string matchValue;

                switch (match.Value)
                {
                    case "bimonthly":
                        matchValue = "1,15";
                        break;
                    default:
                        matchValue = match.Value;
                        break;
                }

                // days of month recurrence
                return new Recurrence 
                ( 
                    matchValue
                        .Split(',')
                        .Select(s => ParseDayOfMonth(s))
                        .ToArray() 
                );
            }

            match = MonthDayPattern.Match(input);
            if (match.Success)
            {
                var split = match.Value.Split('/');

                // periodic recurrence
                return new Recurrence
                (
                    RecurrenceInterval.Every_X_Years,
                    1,
                    Convert.ToInt32(split[0]),
                    ParseDayOfMonth(split[1]),
                    startDate ?? list?.GetValue<DateTime>("startDate"),
                    endDate ?? list?.GetValue<DateTime>("endDate")
                );
            }

            throw new ParseException($"Invalid recurrence: \"{input}\". Please see documentation / examples.");

            int ParseDayOfMonth(string rawDayOfMonth, int monthIndex = 0)  // 1-31, 0 = last, -1 = last - 1, etc.
            {
                if (int.TryParse(rawDayOfMonth, out int dayOfMonth))
                {
                    if (dayOfMonth < 0)
                        throw new ThisShouldNeverHappenException($"Invalid day of month: {dayOfMonth}");
                    
                    if (dayOfMonth == 0)
                        throw new ParseException("Invalid day of month: 0");

                    switch (monthIndex)
                    {
                        case 0:
                            if (dayOfMonth > 31)
                                throw new ParseException($"Invalid day of month: {dayOfMonth}");
                            break;
                        case 2:
                            if (dayOfMonth > 29)
                                throw new ParseException($"Invalid day for month of {new DateTime(1900, 2, 1):MMMM}: {dayOfMonth}");
                            break;
                        case 1:
                        case 3:
                        case 5:
                        case 7:
                        case 8:
                        case 10:
                        case 12:
                            if (dayOfMonth > 31)
                                throw new ParseException($"Invalid day for month of {new DateTime(1900, monthIndex, 1):MMMM}: {dayOfMonth}");
                            break;
                        case 4:
                        case 6:
                        case 9:
                        case 11:
                            if (dayOfMonth > 30)
                                throw new ParseException($"Invalid day for month of {new DateTime(1900, monthIndex, 1):MMMM}: {dayOfMonth}");
                            break;
                    }
                    return dayOfMonth;
                }

                var rawDayOfMonthLo = rawDayOfMonth.ToLower();

                if (string.Equals(rawDayOfMonthLo, "last"))
                    return 0;

                if (rawDayOfMonthLo.StartsWith("{") && rawDayOfMonthLo.EndsWith("}"))
                {
                    var subSplitLo = rawDayOfMonthLo
                        .Substring(1, rawDayOfMonthLo.Length - 2)
                        .Split('-');

                    switch (subSplitLo[0])
                    {
                        case "month.length":
                            return subSplitLo.Length == 2
                                ? -int.Parse(subSplitLo[1])  // e.g. month.length-1
                                : 0;
                        default:
                            throw new ParseException($"Invalid token: \"{rawDayOfMonth.Substring(1, rawDayOfMonth.Length - 2).Split('-')[0]}\". Please see documentation / examples.");
                    }
                }

                throw new ParseException($"Invalid day of month: \"{rawDayOfMonth}\". Please see documentation / examples.");
            }
        }

        public static implicit operator Recurrence(string input) => Parse(input);
    }
}
