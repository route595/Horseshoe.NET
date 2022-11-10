using System;
using System.Text;

using Horseshoe.NET.Objects;
using Horseshoe.NET.Text;

namespace Horseshoe.NET.DateAndTime
{
    /// <summary>
    /// A complement to the <c>TimeSpan</c> that includes months and years
    /// </summary>
    /// <seealso cref="TimeSpan"/>
    public struct YearSpan
    {
        public DateTime From { get; }
        public DateTime To { get; }
        public TimeSpan YearsTimeSpan { get; }
        public int Years { get; }
        private readonly double _totalYears;
        public double TotalYears => Decimals >= 0
            ? Math.Round(_totalYears, Decimals)
            : _totalYears;
        public TimeSpan MonthsTimeSpan { get; }
        public int Months { get; }
        private readonly double _totalMonths;
        public double TotalMonths => Decimals >= 0
            ? Math.Round(_totalMonths, Decimals)
            : _totalMonths;
        public TimeSpan DaysTimeSpan { get; }
        public int Days => DaysTimeSpan.Days;
        public double TotalDays => Decimals >= 0
            ? Math.Round(YearsTimeSpan.TotalDays, Decimals)
            : YearsTimeSpan.TotalDays;
        public int Hours => DaysTimeSpan.Hours;
        public double TotalHours => Decimals >= 0
            ? Math.Round(YearsTimeSpan.TotalHours, Decimals)
            : YearsTimeSpan.TotalHours;
        public int Minutes => DaysTimeSpan.Minutes;
        public double TotalMinutes => Decimals >= 0
            ? Math.Round(YearsTimeSpan.TotalMinutes, Decimals)
            : YearsTimeSpan.TotalMinutes;
        public int Seconds => DaysTimeSpan.Seconds;
        public double TotalSeconds => Decimals >= 0
            ? Math.Round(YearsTimeSpan.TotalSeconds, Decimals)
            : YearsTimeSpan.TotalSeconds;
        public int Milliseconds => DaysTimeSpan.Milliseconds;
        public double TotalMilliseconds => Decimals >= 0
            ? Math.Round(YearsTimeSpan.TotalMilliseconds, Decimals)
            : YearsTimeSpan.TotalMilliseconds;
        public int Decimals { get; }

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

        /// <summary>
        /// Gets a weighted, fractional number of days "per month" for smoother decimal month calculations. 
        /// </summary>
        /// <param name="from">From date</param>
        /// <param name="to">To date</param>
        /// <param name="daysTimeSpan">Time span</param>
        /// <returns></returns>
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

        public override string ToString()
        {
            return ToString(new RangeOptions { MaxRange = 7 });
        }

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

        public abstract class DisplayOptions
        {
            public string YearLabel { get; set; } = " year";
            public string YearsLabel { get; set; } = " years";
            public string BothYearLabels { set { YearLabel = value; YearsLabel = value; } }
            public string MonthLabel { get; set; } = " month";
            public string MonthsLabel { get; set; } = " months";
            public string BothMonthLabels { set { MonthLabel = value; MonthsLabel = value; } }
            public string DayLabel { get; set; } = " day";
            public string DaysLabel { get; set; } = " days";
            public string BothDayLabels { set { DayLabel = value; DaysLabel = value; } }
            public string HourLabel { get; set; } = " hour";
            public string HoursLabel { get; set; } = " hours";
            public string BothHourLabels { set { HourLabel = value; HoursLabel = value; } }
            public string MinuteLabel { get; set; } = " minute";
            public string MinutesLabel { get; set; } = " minutes";
            public string BothMinuteLabels { set { MinuteLabel = value; MinutesLabel = value; } }
            public string SecondLabel { get; set; } = " second";
            public string SecondsLabel { get; set; } = " seconds";
            public string BothSecondLabels { set { SecondLabel = value; SecondsLabel = value; } }
            public string MillisecondLabel { get; set; } = " millisecond";
            public string MillisecondsLabel { get; set; } = " milliseconds";
            public string BothMillisecondLabels { set { MillisecondLabel = value; MillisecondsLabel = value; } }
        }

        public class RangeOptions : DisplayOptions
        {
            public int MaxRange { get; set; }

            public RangeOptions() { }

            public RangeOptions(RangeOptions options)
            {
                options.MapProperties(this);
            }
        }

        public class SetOptions : DisplayOptions
        {
            public TimePartDisplay DisplayYears { get; set; }
            public TimePartDisplay DisplayMonths { get; set; }
            public TimePartDisplay DisplayDays { get; set; }
            public TimePartDisplay DisplayHours { get; set; }
            public TimePartDisplay DisplayMinutes { get; set; }
            public TimePartDisplay DisplaySeconds { get; set; }
            public TimePartDisplay DisplayMilliseconds { get; set; }

            public SetOptions() { }

            public SetOptions(SetOptions options)
            {
                options.MapProperties(this);
            }

            public static SetOptions AllAuto { get; } = new SetOptions
            {
                DisplayYears = TimePartDisplay.Auto,
                DisplayMonths = TimePartDisplay.Auto,
                DisplayDays = TimePartDisplay.Auto,
                DisplayHours = TimePartDisplay.Auto,
                DisplayMinutes = TimePartDisplay.Auto,
                DisplaySeconds = TimePartDisplay.Auto,
                DisplayMilliseconds = TimePartDisplay.Auto
            };

            public static SetOptions AllOn { get; } = new SetOptions
            {
                DisplayYears = TimePartDisplay.On,
                DisplayMonths = TimePartDisplay.On,
                DisplayDays = TimePartDisplay.On,
                DisplayHours = TimePartDisplay.On,
                DisplayMinutes = TimePartDisplay.On,
                DisplaySeconds = TimePartDisplay.On,
                DisplayMilliseconds = TimePartDisplay.On
            };
        }

        public enum TimePartDisplay
        {
            Off,
            On,
            Auto
        }
    }
}
