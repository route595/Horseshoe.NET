using System;

namespace Horseshoe.NET.Mathematics.Finance.Recurrence
{
    internal class NoRecurrence : Recurrence
    {
        public override DateTime Next(DateTime afterDate)
        {
            return DateTime.MaxValue;
        }
    }
}
