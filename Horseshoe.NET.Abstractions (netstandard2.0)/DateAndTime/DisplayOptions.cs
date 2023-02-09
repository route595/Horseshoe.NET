namespace Horseshoe.NET.DateAndTime
{
    /// <summary>
    /// The base class for the several diffent options for displaying <c>YearSpan</c> as text
    /// </summary>
    public abstract class DisplayOptions
    {
        /// <summary>
        /// Label for the 'year' date time part for single year instances, default is " year"
        /// </summary>
        public string YearLabel { get; set; } = " year";

        /// <summary>
        /// Label for the 'year' date time part for all but single year instances, default is " years"
        /// </summary>
        public string YearsLabel { get; set; } = " years";

        /// <summary>
        /// Sets both 'year' and 'years' labels at the same time
        /// </summary>
        public string BothYearLabels { set { YearLabel = value; YearsLabel = value; } }

        /// <summary>
        /// Label for the 'month' date time part for single month instances, default is " month"
        /// </summary>
        public string MonthLabel { get; set; } = " month";

        /// <summary>
        /// Label for the 'month' date time part for all but single month instances, default is " months"
        /// </summary>
        public string MonthsLabel { get; set; } = " months";

        /// <summary>
        /// Sets both 'month' and 'months' labels at the same time
        /// </summary>
        public string BothMonthLabels { set { MonthLabel = value; MonthsLabel = value; } }

        /// <summary>
        /// Label for the 'day' date time part for single day instances, default is " day"
        /// </summary>
        public string DayLabel { get; set; } = " day";

        /// <summary>
        /// Label for the 'day' date time part for all but single day instances, default is " days"
        /// </summary>
        public string DaysLabel { get; set; } = " days";

        /// <summary>
        /// Sets both 'day' and 'days' labels at the same time
        /// </summary>
        public string BothDayLabels { set { DayLabel = value; DaysLabel = value; } }

        /// <summary>
        /// Label for the 'hour' date time part for single hour instances, default is " hour"
        /// </summary>
        public string HourLabel { get; set; } = " hour";

        /// <summary>
        /// Label for the 'hour' date time part for all but single hour instances, default is " hours"
        /// </summary>
        public string HoursLabel { get; set; } = " hours";

        /// <summary>
        /// Sets both 'hour' and 'hours' labels at the same time
        /// </summary>
        public string BothHourLabels { set { HourLabel = value; HoursLabel = value; } }

        /// <summary>
        /// Label for the 'minute' date time part for single minute instances, default is " minute"
        /// </summary>
        public string MinuteLabel { get; set; } = " minute";

        /// <summary>
        /// Label for the 'minute' date time part for all but single minute instances, default is " minutes"
        /// </summary>
        public string MinutesLabel { get; set; } = " minutes";

        /// <summary>
        /// Sets both 'minute' and 'minutes' labels at the same time
        /// </summary>
        public string BothMinuteLabels { set { MinuteLabel = value; MinutesLabel = value; } }

        /// <summary>
        /// Label for the 'second' date time part for single second instances, default is " second"
        /// </summary>
        public string SecondLabel { get; set; } = " second";

        /// <summary>
        /// Label for the 'second' date time part for all but single second instances, default is " seconds"
        /// </summary>
        public string SecondsLabel { get; set; } = " seconds";

        /// <summary>
        /// Sets both 'second' and 'seconds' labels at the same time
        /// </summary>
        public string BothSecondLabels { set { SecondLabel = value; SecondsLabel = value; } }

        /// <summary>
        /// Label for the 'millisecond' date time part for single millisecond instances, default is " millisecond"
        /// </summary>
        public string MillisecondLabel { get; set; } = " millisecond";

        /// <summary>
        /// Label for the 'millisecond' date time part for all but single millisecond instances, default is " milliseconds"
        /// </summary>
        public string MillisecondsLabel { get; set; } = " milliseconds";

        /// <summary>
        /// Sets both 'millisecond' and 'milliseconds' labels at the same time
        /// </summary>
        public string BothMillisecondLabels { set { MillisecondLabel = value; MillisecondsLabel = value; } }
    }
}
