using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Horseshoe.NET.DateAndTime
{
    public static class DateUtil
    {
        public static bool IsLeapYear(DateTime date)
        {
            return IsLeapYear(date.Year);
        }

        public static bool IsLeapYear(int year)
        {
            if (year % 400 == 0) return true;
            if (year % 100 == 0) return false;
            if (year % 4 == 0) return true;
            return false;
        }

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

        public static bool SameDay(DateTime date1, DateTime date2)
        {
            return date1.Year == date2.Year && date1.Month == date2.Month && date1.Day == date2.Day;
        }

        public static bool SameMonth(DateTime date1, DateTime date2)
        {
            return date1.Year == date2.Year && date1.Month == date2.Month;
        }

        public static int GetNumberOfDaysInMonth(DateTime dateTime)
        {
            return GetNumberOfDaysInMonth(dateTime.Year, dateTime.Month);
        }

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
                    return 31;
                case 4:   // Apr
                case 6:   // Jun
                case 9:   // Sep
                case 11:  // Nov
                    return 30;
                case 2:   // Feb
                    return IsLeapYear(year) ? 29 : 28;
                default:
                    return -1;
            }
        }
    }
}
