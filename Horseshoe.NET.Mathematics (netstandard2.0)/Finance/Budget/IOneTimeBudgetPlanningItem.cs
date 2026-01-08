using System;

namespace Horseshoe.NET.Mathematics.Finance.Budget
{
    public interface IOneTimeBudgetPlanningItem
    {
        DateTime TransactionDate { get; }
    }
}
