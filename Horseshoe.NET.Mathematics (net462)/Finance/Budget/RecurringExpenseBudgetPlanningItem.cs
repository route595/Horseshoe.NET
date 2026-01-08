using System.Collections.Generic;

namespace Horseshoe.NET.Mathematics.Finance.Budget
{
    public class RecurringExpenseBudgetPlanningItem : BudgetPlanningItem, IExpensePlanningItem, IRecurringBudgetPlanningItem
    {

        /// <inheritdoc cref="Finance.Budget.Recurrence"/>
        public Recurrence Recurrence { get; set; }

        /// <inheritdoc cref="IRecurringBudgetPlanningItem.Variances"></inheritdoc>
        public IEnumerable<Variance> Variances { get; set; }

        public ExpenseCategory Category { get; set; }
    }
}
