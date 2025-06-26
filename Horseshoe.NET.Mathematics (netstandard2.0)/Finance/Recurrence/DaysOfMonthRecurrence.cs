using System;
using System.Linq;

namespace Horseshoe.NET.Mathematics.Finance.Recurrence
{
    public class DaysOfMonthRecurrence : Recurrence
    {
        public int[] DaysOfMonth { get; set; }

        public const int LastDayOfMonth = 32;

        public override DateTime Next(DateTime afterDate)
        {
            if (DaysOfMonth == null || DaysOfMonth.Length == 0 || !DaysOfMonth.Any(i => i >= 1 && i <= LastDayOfMonth)) 
                return DateTime.MaxValue;
            afterDate = afterDate.AddDays(1);
            while (true)
            {
                if (DaysOfMonth.Contains(afterDate.Day))
                    return afterDate;
                if (DaysOfMonth.Contains(LastDayOfMonth) && afterDate.Month != afterDate.AddDays(1).Month) 
                    return afterDate;
                afterDate = afterDate.AddDays(1);
            }
        }
    }
}
