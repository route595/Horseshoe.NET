using System;

namespace Horseshoe.NET.Mathematics.Finance.Budget
{
    public class OneTimeExpenseBudgetPlanningItem : BudgetPlanningItem, IExpensePlanningItem, IOneTimeBudgetPlanningItem
    {
        public ExpenseCategory Category { get; set; }

        public DateTime TransactionDate { get; set; }
    }
}
