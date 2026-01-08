using System.Collections.Generic;
using System.Linq;

namespace Horseshoe.NET.Mathematics.Finance.Budget
{
    public class RecurringIncomeBudgetPlanningItem : BudgetPlanningItem, IIncomePlanningItem, IRecurringBudgetPlanningItem
    {
        /// <inheritdoc cref="Finance.Budget.Recurrence"/>
        public Recurrence Recurrence { get; set; }

        /// <inheritdoc cref="IRecurringBudgetPlanningItem.Variances"></inheritdoc>
        public IEnumerable<Variance> Variances { get; set; }

        public IncomeCategory Category { get; set; }

        public IEnumerable<AutoDisbursement> AutoDisbursements { get; set; }

        public bool HasAutoDisbursements => AutoDisbursements != null && AutoDisbursements.Any();

        public RecurringIncomeBudgetPlanningItem AddAutoDisbursement(AutoDisbursement autoDisbursement)
        {
            if (AutoDisbursements is List<AutoDisbursement> list)
                list.Add(autoDisbursement);
            else
                AutoDisbursements = new List<AutoDisbursement> { autoDisbursement };
            return this;
        }

        public RecurringIncomeBudgetPlanningItem AddAutoDisbursements(IEnumerable<AutoDisbursement> autoDisbursements)
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
