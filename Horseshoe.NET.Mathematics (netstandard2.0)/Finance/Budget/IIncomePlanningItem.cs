using System.Collections.Generic;

namespace Horseshoe.NET.Mathematics.Finance.Budget
{
    public interface IIncomePlanningItem : IBudgetPlanningItem
    {
        IncomeCategory Category { get; }

        IEnumerable<AutoDisbursement> AutoDisbursements { get; }

        bool HasAutoDisbursements { get; }
    }
}
