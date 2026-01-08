using System.Collections.Generic;

namespace Horseshoe.NET.Mathematics.Finance.Budget
{
    public readonly struct BudgetItemSort : IComparer<BudgetItem>
    {
        public BudgetItemOrdering ItemOrdering { get; }       // default is no sorting

        public BudgetItemSort(BudgetItemOrdering itemOrdering)
        {
            ItemOrdering = itemOrdering;
        }

        public int Compare(BudgetItem x, BudgetItem y)
        {
            var temp = x.TransactionDate.CompareTo(y.TransactionDate);
            if (temp != 0) 
                return (ItemOrdering & BudgetItemOrdering.DateDescending) == BudgetItemOrdering.DateDescending
                    ? -temp
                    : temp;

            if ((ItemOrdering & BudgetItemOrdering.IncomeFirst) == BudgetItemOrdering.IncomeFirst && (x.BudgetPlanningItem is IIncomePlanningItem ^ y.BudgetPlanningItem is IIncomePlanningItem))
                return x.BudgetPlanningItem is IIncomePlanningItem ? -1 : 1;

            return 0;
        }
    }
}
