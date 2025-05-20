using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Horseshoe.NET.Finance.Recurrence
{
    public abstract class Recurrence
    {
        public abstract DateTime Next(DateTime afterDate);

        public static Recurrence NoRecurrence { get; } = new NoRecurrence();

        private static Regex PeriodicPattern { get; } = new Regex("^[dwmqy](-[0-9]+)?$");  // e.g. "y", "y-1", "d-14", etc.
        private static Regex DaysOfMonthPattern { get; } = new Regex("^(([0-9]+)|last)(,(([0-9]+)|last))*$");  // e.g. "5", "1,15", "15,last", etc.

        internal static Recurrence Parse(string notation)
        {
            Match match = PeriodicPattern.Match(notation);
            if (match.Success)
            {
                RecurrenceInterval interval;
                switch (match.Value[0])
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
                return match.Value.Contains("-")
                    ? new PeriodicRecurrence { Interval = interval, X = Convert.ToInt32(match.Value.Split('-')[1]) } 
                    : new PeriodicRecurrence { Interval = interval };
            }

            match = DaysOfMonthPattern.Match(notation);
            if (match.Success)
            {
                return new DaysOfMonthRecurrence { DaysOfMonth = match.Value.Split(',').Select(s => s.Equals("last") ? DaysOfMonthRecurrence.LastDayOfMonth : Convert.ToInt32(s)).ToArray() };
            }

            return NoRecurrence;
        }

        public static implicit operator Recurrence(string notation) => Parse(notation);
    }
}
