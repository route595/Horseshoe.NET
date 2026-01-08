using System;

namespace Horseshoe.NET.Mathematics.Finance.Budget
{
    [Flags]
    public enum BudgetItemOrdering
    {
        None = 0,
        DateDescending = 1,
        IncomeFirst = 2
    }
}
