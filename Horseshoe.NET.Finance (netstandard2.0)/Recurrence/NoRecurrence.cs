using System;

namespace Horseshoe.NET.Finance.Recurrence
{
    internal class NoRecurrence : Recurrence
    {
        public override DateTime Next(DateTime afterDate)
        {
            return DateTime.MaxValue;
        }
    }
}
