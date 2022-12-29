using System;
using System.Text;

using Horseshoe.NET.Text;

namespace Horseshoe.NET.DateAndTime
{
    /// <summary>
    /// A complement to <c>TimeSpan</c> that includes months and years
    /// </summary>
    /// <seealso cref="TimeSpan"/>
    public readonly struct YearSpan
    {
        /// <summary>
        /// The from date
        /// </summary>
        public DateTime From { get; }

        /// <summary>
        /// The to date
        /// </summary>
        public DateTime To { get; }

        /// <summary>
        /// One of the two interim <c>TimeSpan</c>s used in calculating years and months
        /// </summary>
        public TimeSpan YearsTimeSpan { get; }

        /// <summary>
        /// The number of whole years represented in this <c>YearSpan</c>
        /// </summary>
        public int Years { get; }

        private readonly double _totalYears;

        /// <summary>
        /// The total time represented by this <c>YearSpan</c> expressed in years with double precision 
        /// </summary>
        public double TotalYears => Decimals >= 0
            ? Math.Round(_totalYears, Decimals)
            : _totalYears;

        /// <summary>
        /// One of the two interim <c>TimeSpan</c>s used in calculating years and months
        /// </summary>
        public TimeSpan MonthsTimeSpan { get; }

        /// <summary>
        /// The number of whole months represented in this <c>YearSpan</c>
        /// </summary>
        public int Months { get; }

        private readonly double _totalMonths;

        /// <summary>
        /// The total time represented by this <c>YearSpan</c> expressed in months with double precision 
        /// </summary>
        public double TotalMonths => Decimals >= 0
            ? Math.Round(_totalMonths, Decimals)
            : _totalMonths;

        /// <summary>
        /// The main <c>TimeSpan</c>s used in calculating days, hours, minutes, seconds and milliseconds
        /// </summary>
        public TimeSpan DaysTimeSpan { get; }

        /// <summary>
        /// The number of whole days represented in this <c>YearSpan</c>
        /// </summary>
        public int Days => DaysTimeSpan.Days;

        /// <summary>
        /// The total time represented by this <c>YearSpan</c> expressed in days with double precision 
        /// </summary>
        public double TotalDays => Decimals >= 0
            ? Math.Round(YearsTimeSpan.TotalDays, Decimals)
            : YearsTimeSpan.TotalDays;

        /// <summary>
        /// The number of whole hours represented in this <c>YearSpan</c>
        /// </summary>
        public int Hours => DaysTimeSpan.Hours;

        /// <summary>
        /// The total time represented by this <c>YearSpan</c> expressed in hours with double precision 
        /// </summary>
        public double TotalHours => Decimals >= 0
            ? Math.Round(YearsTimeSpan.TotalHours, Decimals)
            : YearsTimeSpan.TotalHours;

        /// <summary>
        /// The number of whole minutes represented in this <c>YearSpan</c>
        /// </summary>
        public int Minutes => DaysTimeSpan.Minutes;

        /// <summary>
        /// The total time represented by this <c>YearSpan</c> expressed in minutes with double precision 
        /// </summary>
        public double TotalMinutes => Decimals >= 0
            ? Math.Round(YearsTimeSpan.TotalMinutes, Decimals)
            : YearsTimeSpan.TotalMinutes;

        /// <summary>
        /// The number of whole seconds represented in this <c>YearSpan</c>
        /// </summary>
        public int Seconds => DaysTimeSpan.Seconds;

        /// <summary>
        /// The total time represented by this <c>YearSpan</c> expressed in seconds with double precision 
        /// </summary>
        public double TotalSeconds => Decimals >= 0
            ? Math.Round(YearsTimeSpan.TotalSeconds, Decimals)
            : YearsTimeSpan.TotalSeconds;

        /// <summary>
        /// The number of milliseconds represented in this <c>YearSpan</c>
        /// </summary>
        public int Milliseconds => DaysTimeSpan.Milliseconds;

        /// <summary>
        /// The total time represented by this <c>YearSpan</c> expressed in milliseconds 
        /// </summary>
        public double TotalMilliseconds => Decimals >= 0
            ? Math.Round(YearsTimeSpan.TotalMilliseconds, Decimals)
            : YearsTimeSpan.TotalMilliseconds;

        /// <summary>
        /// Optional, the number of decimals to use for rounding
        /// </summary>
        public int Decimals { get; }

        /// <summary>
        /// Creates a new <c>YearSpan</c>
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="decimals"></param>
        public YearSpan(DateTime from, DateTime to, int decimals = -1)
        {
            DateTime temp;

            // base info
            if (from > to)
            {
                temp = from;
                from = to;
                to = temp;
            }
            From = from;
            To = to;
            YearsTimeSpan = to - from;
            Decimals = decimals;

            // calculate years
            Years = 0;
            temp = from.AddYears(1);
            while (temp < to)
            {
                Years++;
                temp = from.AddYears(Years + 1);
            }
            if (Years > 0)
            {
                from = from.AddYears(Years);
                MonthsTimeSpan = to - from;
            }
            else
            {
                MonthsTimeSpan = YearsTimeSpan;
            }

            // calculate months
            Months = 0;
            temp = from.AddMonths(1);
            while (temp < to)
            {
                Months++;
                temp = from.AddMonths(Months + 1);
            }
            if (Months > 0)
            {
                from = from.AddMonths(Months);
                DaysTimeSpan = to - from;
            }
            else
            {
                DaysTimeSpan = MonthsTimeSpan;
            }

            // calculate total months
            var _tempTotalMonths = Months + DaysTimeSpan.TotalDays / CalculateDaysInMonth(from, to, DaysTimeSpan);
            _totalMonths = Years * 12.0 + _tempTotalMonths;

            // calculate total years
            _totalYears = Years + _tempTotalMonths / 12.0;
        }

        // Gets a weighted, fractional number of days "per month" for smoother decimal month calculations. 
        private static double CalculateDaysInMonth(DateTime from, DateTime to, TimeSpan daysTimeSpan)
        {
            var fromLength = DateUtil.GetNumberOfDaysInMonth(from.Year, from.Month);
            var toLength = DateUtil.GetNumberOfDaysInMonth(to.Year, to.Month);
            if (fromLength == toLength)
            {
                return fromLength;
            }
            var toRatio = (double)to.Day / daysTimeSpan.Days;  // Note: The from diff is calculated from the inverse
            var fromDiff = (toLength - fromLength) * toRatio;  //       of the 'from' ratio i.e. the 'to' ratio.
            return fromLength + fromDiff;
        }

        /// <summary>
        /// Gets a common <c>string</c> representation of this <c>YearSpan</c>, for more display options see <see cref="ToString(DisplayOptions)"/>
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return ToString(new RangeOptions { MaxRange = 7 });
        }

        /// <summary>
        /// Gets a fine-tuned <c>string</c> representation of this <c>YearSpan</c> with options for range, specific sets of date/time parts
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        /// <remarks>
        /// <seealso cref="RangeOptions" />
        /// <seealso cref="SetOptions" />
        /// </remarks>
        public string ToString(DisplayOptions options)
        {
            var sb = new StringBuilder();
            if (options is RangeOptions rangeOptions)
            {
                var levelsDeep = 0;
                if (Years > 0 && levelsDeep < rangeOptions.MaxRange)
                {
                    levelsDeep++;
                    sb.Append(Years).Append(Years == 1 ? options.YearLabel : options.YearsLabel);
                }
                if ((Months > 0 || levelsDeep > 0) && levelsDeep < rangeOptions.MaxRange)
                {
                    levelsDeep++;
                    sb.AppendIf(sb.Length > 0, " ").Append(Months).Append(Months == 1 ? options.MonthLabel : options.MonthsLabel);
                }
                if ((Days > 0 || levelsDeep > 0) && levelsDeep < rangeOptions.MaxRange)
                {
                    levelsDeep++;
                    sb.AppendIf(sb.Length > 0, " ").Append(Days).Append(Days == 1 ? options.DayLabel : options.DaysLabel);
                }
                if ((Hours > 0 || levelsDeep > 0) && levelsDeep < rangeOptions.MaxRange)
                {
                    levelsDeep++;
                    sb.AppendIf(sb.Length > 0, " ").Append(Hours).Append(Hours == 1 ? options.HourLabel : options.HoursLabel);
                }
                if ((Minutes > 0 || levelsDeep > 0) && levelsDeep < rangeOptions.MaxRange)
                {
                    levelsDeep++;
                    sb.AppendIf(sb.Length > 0, " ").Append(Minutes).Append(Minutes == 1 ? options.MinuteLabel : options.MinutesLabel);
                }
                if ((Seconds > 0 || levelsDeep > 0) && levelsDeep < rangeOptions.MaxRange)
                {
                    levelsDeep++;
                    sb.AppendIf(sb.Length > 0, " ").Append(Seconds).Append(Seconds == 1 ? options.SecondLabel : options.SecondsLabel);
                }
                if ((Milliseconds > 0 || levelsDeep > 0) && levelsDeep < rangeOptions.MaxRange)
                {
                    sb.AppendIf(sb.Length > 0, " ").Append(Milliseconds).Append(Milliseconds == 1 ? options.MillisecondLabel : options.MillisecondsLabel);
                }
            }
            else if (options is SetOptions setOptions)
            {
                if ((Years > 0 || setOptions.DisplayYears == TimePartDisplay.On) && setOptions.DisplayYears != TimePartDisplay.Off)
                {
                    sb.Append(Years).Append(Years == 1 ? options.YearLabel : options.YearsLabel);
                }
                if ((Months > 0 || setOptions.DisplayMonths == TimePartDisplay.On) && setOptions.DisplayMonths != TimePartDisplay.Off)
                {
                    sb.AppendIf(sb.Length > 0, " ").Append(Months).Append(Months == 1 ? options.MonthLabel : options.MonthsLabel);
                }
                if ((Days > 0 || setOptions.DisplayDays == TimePartDisplay.On) && setOptions.DisplayDays != TimePartDisplay.Off)
                {
                    sb.AppendIf(sb.Length > 0, " ").Append(Days).Append(Days == 1 ? options.DayLabel : options.DaysLabel);
                }
                if ((Hours > 0 || setOptions.DisplayHours == TimePartDisplay.On) && setOptions.DisplayHours != TimePartDisplay.Off)
                {
                    sb.AppendIf(sb.Length > 0, " ").Append(Hours).Append(Hours == 1 ? options.HourLabel : options.HoursLabel);
                }
                if ((Minutes > 0 || setOptions.DisplayMinutes == TimePartDisplay.On) && setOptions.DisplayMinutes != TimePartDisplay.Off)
                {
                    sb.AppendIf(sb.Length > 0, " ").Append(Minutes).Append(Minutes == 1 ? options.MinuteLabel : options.MinutesLabel);
                }
                if ((Seconds > 0 || setOptions.DisplaySeconds == TimePartDisplay.On) && setOptions.DisplaySeconds != TimePartDisplay.Off)
                {
                    sb.AppendIf(sb.Length > 0, " ").Append(Seconds).Append(Seconds == 1 ? options.SecondLabel : options.SecondsLabel);
                }
                if ((Milliseconds > 0 || setOptions.DisplayMilliseconds == TimePartDisplay.On) && setOptions.DisplayMilliseconds != TimePartDisplay.Off)
                {
                    sb.AppendIf(sb.Length > 0, " ").Append(Milliseconds).Append(Milliseconds == 1 ? options.MillisecondLabel : options.MillisecondsLabel);
                }
            }
            else
            {
                throw new ArgumentException("Invalid DisplayOptions instance, must be an instance of RangeOptions or SetOptions");
            }
            return sb.ToString();
        }
    }
}
