using System;

//using Horseshoe.NET.Finance.Recurrence;

namespace Horseshoe.NET.Finance
{
    public class BudgetPlanningItem
    {
        public string Name { get; set; }
        public decimal Amount { get; set; }
        public DateTime TransactionDate { get; set; }

        /// <summary>
        /// Defines transaction recurrence (from <c>TransactionDate</c>) using simple notation 
        /// </summary>
        /// <remarks>
        /// Examples...
        /// <list type="bullet">
        /// <item>
        /// <strong>w-2</strong> / <strong>d-14</strong> = every 2 weeks / 14 days
        /// </item>
        /// <item>
        /// <strong>q</strong> or <strong>q-1</strong> / <strong>y</strong> or <strong>y-1</strong> = quarterly / yearly
        /// </item>
        /// <item>
        /// <strong>5</strong> / <strong>last</strong> = on the 5th / last day of every month
        /// </item>
        /// <item>
        /// <strong>1,15</strong> / <strong>15,last</strong> = on the 1st and 15th / 15th and last day of every month
        /// </item>
        /// </list>
        /// </remarks>
        public Recurrence.Recurrence Recurrence { get; set; } = Finance.Recurrence.Recurrence.NoRecurrence;
    }
}
