using System;

namespace Horseshoe.NET.Finance.Recurrence
{
    public class PeriodicRecurrence : Recurrence
    {
        public RecurrenceInterval Interval { get; set; }

        public int X { get; set; } = 1;

        public override DateTime Next(DateTime afterDate)
        {
            switch (Interval)
            {
                case RecurrenceInterval.Every_X_Days:
                default:
                    return afterDate.AddDays(X);
                case RecurrenceInterval.Every_X_Weeks:
                    return afterDate.AddDays(X * 7);
                case RecurrenceInterval.Every_X_Months:
                    return afterDate.AddMonths(X);
                case RecurrenceInterval.Every_X_Quarters:
                    return afterDate.AddMonths(X * 3);
                case RecurrenceInterval.Every_X_Years:
                    return afterDate.AddYears(X);
            }
        }
    }
}
