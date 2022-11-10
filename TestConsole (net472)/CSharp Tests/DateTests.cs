using System;
using System.Collections.Generic;

using Horseshoe.NET.ConsoleX;
using Horseshoe.NET.DateAndTime;

namespace TestConsole.CSharpTests
{
    class DateTests : RoutineX
    {
        public override IList<MenuObject> Menu => new[]
        {
            BuildMenuRoutine
            (
                "Max Age",
                () =>
                {
                    Console.WriteLine("int " + int.MaxValue);
                    var maxIntAgeInDays = (double)int.MaxValue / (1000 * 60 * 60 * 24);
                    var maxIntAgeInYears = maxIntAgeInDays / 365;
                    Console.WriteLine(string.Format("max age: {0:0.##} days or {1:0.##} years", maxIntAgeInDays, maxIntAgeInYears));
                    Console.WriteLine();
                    Console.WriteLine("long " + long.MaxValue);
                    var maxLongAgeInDays = (double)long.MaxValue / (1000 * 60 * 60 * 24);
                    var maxLongAgeInYears = maxLongAgeInDays / 365;
                    Console.WriteLine(string.Format("max age: {0:0.##} days or {1:0.##} years", maxLongAgeInDays, maxLongAgeInYears));
                }
            ),
            BuildMenuRoutine
            (
                "YearSpan test - birthdates",
                () =>
                {
                    var birthdate = new DateTime(2012, 4, 16);
                    var yearSpans = new[]
                    {
                        new YearSpan(DateTime.Today.AddYears(-20).AddMonths(-5).AddDays(-10), DateTime.Today, decimals: 2),
                        new YearSpan(DateTime.Today.AddMonths(-4).AddDays(-16), DateTime.Today, decimals: 2),
                        new YearSpan(DateTime.Today.AddYears(-27).AddMonths(-9).AddDays(-6).AddMinutes(-3), DateTime.Now, decimals: 3),
                    };
                    foreach(var yearSpan in yearSpans)
                    {
                        RenderX.ListTitle("Birthdate: " + yearSpan.From.ToString(), padBefore: 1);
                        Console.WriteLine("Years Time Span  : " + yearSpan.YearsTimeSpan);
                        Console.WriteLine("Months Time Span : " + yearSpan.MonthsTimeSpan);
                        Console.WriteLine("Days Time Span   : " + yearSpan.DaysTimeSpan);
                        Console.WriteLine("ToString()       : " + yearSpan);
                        Console.WriteLine("ToString(max=3)  : " + yearSpan.ToString(new YearSpan.RangeOptions { MaxRange = 3 }));
                        Console.WriteLine("ToString(auto)   : " + yearSpan.ToString(YearSpan.SetOptions.AllAuto));
                        Console.WriteLine("ToString(on)     : " + yearSpan.ToString(YearSpan.SetOptions.AllOn));
                        Console.WriteLine("ToString(custom) : " + yearSpan.ToString(new YearSpan.SetOptions{ DisplayYears = YearSpan.TimePartDisplay.Auto, BothYearLabels = "y", DisplayMonths = YearSpan.TimePartDisplay.Auto, BothMonthLabels = "m", DisplayDays = YearSpan.TimePartDisplay.On, BothDayLabels = "d" }));
                        Console.WriteLine("Years            : " + yearSpan.Years);
                        Console.WriteLine("Months           : " + yearSpan.Months);
                        Console.WriteLine("Days             : " + yearSpan.Days);
                        Console.WriteLine("Hours            : " + yearSpan.Hours);
                        Console.WriteLine("Minutes          : " + yearSpan.Minutes);
                        Console.WriteLine("Seconds          : " + yearSpan.Seconds);
                        Console.WriteLine("Milliseconds     : " + yearSpan.Milliseconds);
                        Console.WriteLine("TotalYears       : " + yearSpan.TotalYears);
                        Console.WriteLine("TotalMonths      : " + yearSpan.TotalMonths);
                        Console.WriteLine("TotalDays        : " + yearSpan.TotalDays);
                        Console.WriteLine("TotalHours       : " + yearSpan.TotalHours);
                        Console.WriteLine("TotalMinutes     : " + yearSpan.TotalMinutes);
                        Console.WriteLine("TotalSeconds     : " + yearSpan.TotalSeconds);
                        Console.WriteLine("TotalMilliseconds: " + yearSpan.TotalMilliseconds);
                    }
                }
            ),
            BuildMenuRoutine
            (
                "Compare YearSpan w/ TimeSpans",
                () =>
                {
                    var now = DateTime.Now;
                    var compareDates = new[] 
                    { 
                        DateTime.Today.AddYears(-20).AddMonths(-5).AddDays(-10), 
                        DateTime.Today.AddMonths(-4).AddDays(-16), 
                        DateTime.Today.AddYears(-27).AddMonths(-9).AddDays(-6).AddMinutes(-3) 
                    };
                    foreach (var date in compareDates)
                    {
                        var yearSpan = date.GetAge(to: now, decimals: 3);
                        var span = now - date;

                        RenderX.ListTitle(date.ToString(), padBefore: 1);
                        Console.WriteLine("  * Age: " + yearSpan);
                        Console.WriteLine("    - Years        : " + yearSpan.Years);
                        Console.WriteLine("    - Months       : " + yearSpan.Months);
                        Console.WriteLine("    - Days         : " + yearSpan.Days);
                        Console.WriteLine("    - Hours        : " + yearSpan.Hours);
                        Console.WriteLine("    - Minutes      : " + yearSpan.Minutes);
                        Console.WriteLine("    - Seconds      : " + yearSpan.Seconds);
                        Console.WriteLine("    - TotalYears   : " + yearSpan.TotalYears);
                        Console.WriteLine("    - TotalMonths  : " + yearSpan.TotalMonths);
                        Console.WriteLine("    - TotalDays    : " + yearSpan.TotalDays);
                        Console.WriteLine("    - TotalHours   : " + yearSpan.TotalHours);
                        Console.WriteLine("    - TotalMinutes : " + yearSpan.TotalMinutes);
                        Console.WriteLine("    - TotalSeconds : " + yearSpan.TotalSeconds);
                        Console.WriteLine("  * Span: " + span);
                        Console.WriteLine("    - Days         : " + span.Days);
                        Console.WriteLine("    - Hours        : " + span.Hours);
                        Console.WriteLine("    - Minutes      : " + span.Minutes);
                        Console.WriteLine("    - Seconds      : " + span.Seconds);
                        Console.WriteLine("    - TotalDays    : " + span.TotalDays);
                        Console.WriteLine("    - TotalHours   : " + span.TotalHours);
                        Console.WriteLine("    - TotalMinutes : " + span.TotalMinutes);
                        Console.WriteLine("    - TotalSeconds : " + span.TotalSeconds);
                    }
                }
            )
        };
    }
}
