using System;

namespace Horseshoe.NET.Mathematics.Finance.Budget
{
    public class OneTimeTransferBudgetPlanningItem : BudgetPlanningItem, ITransferPlanningItem, IOneTimeBudgetPlanningItem
    {
        public DateTime TransactionDate { get; set; }

        public Account SourceAccount { get; set; }
    }
}
