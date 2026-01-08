using System;
using System.Collections.Generic;
using System.Linq;

namespace Horseshoe.NET.Mathematics.Finance.Budget
{
    public class OneTimeIncomeBudgetPlanningItem : BudgetPlanningItem, IIncomePlanningItem, IOneTimeBudgetPlanningItem
    {
        public IncomeCategory Category { get; set; }

        public DateTime TransactionDate { get; set; }

        public IEnumerable<AutoDisbursement> AutoDisbursements { get; set; }

        public bool HasAutoDisbursements => AutoDisbursements != null && AutoDisbursements.Any();

        public OneTimeIncomeBudgetPlanningItem AddAutoDisbursement(AutoDisbursement autoDisbursement)
        {
            if (AutoDisbursements is List<AutoDisbursement> list)
                list.Add(autoDisbursement);
            else
                AutoDisbursements = new List<AutoDisbursement> { autoDisbursement };
            return this;
        }

        public OneTimeIncomeBudgetPlanningItem AddAutoDisbursements(IEnumerable<AutoDisbursement> autoDisbursements)
        {
            if (autoDisbursements != null)
            {
                foreach (var disbursement in autoDisbursements)
                {
                    AddAutoDisbursement(disbursement);
                }
            }
            return this;
        }
    }
}
