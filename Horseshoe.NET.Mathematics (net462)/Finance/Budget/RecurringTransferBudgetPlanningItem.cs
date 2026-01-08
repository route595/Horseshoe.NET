using System.Collections.Generic;

namespace Horseshoe.NET.Mathematics.Finance.Budget
{
    public class RecurringTransferBudgetPlanningItem : BudgetPlanningItem, ITransferPlanningItem, IRecurringBudgetPlanningItem
    {
        public Account SourceAccount { get; set; }

        /// <inheritdoc cref="Capabilities.Recurrence"/>
        public Recurrence Recurrence { get; set; }

        public IEnumerable<Variance> Variances { get; set; }
    }
}
