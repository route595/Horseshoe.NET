using System;

namespace Horseshoe.NET.Mathematics.Finance.Budget
{
    public interface IBudgetPlanningItem
    {
        IBudgetPlanningItem ParentBudgetPlanningItem { get; }

        string Name { get; }

        Account Account { get; }

        decimal Amount { get; }
    }
}
