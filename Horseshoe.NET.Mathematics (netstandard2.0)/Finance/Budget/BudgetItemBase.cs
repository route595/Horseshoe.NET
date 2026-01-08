using System;

using static Horseshoe.NET.Guardrails;
using Horseshoe.NET.ObjectsTypesAndValues;

namespace Horseshoe.NET.Mathematics.Finance.Budget
{
    public class BudgetItemBase
    {
        private string _name;
        private decimal? _amount;
        private DateTime? _transactionDate;

        public IBudgetPlanningItem BudgetPlanningItem { get; }

        public string Name 
        { 
            get => PrivateCalculateName();
            set { _name = value; }
        }

        public decimal Amount
        {
            get => PrivateCalculateAmount();
            set { _amount = value; }
        }

        public DateTime TransactionDate
        {
            get => _transactionDate ?? Wrangle((BudgetPlanningItem as IOneTimeBudgetPlanningItem)?.TransactionDate);
            set { _transactionDate = Wrangle(value); }
        }

        public decimal RunningTotal { get; set; }


        /// <summary>
        /// Gets or sets the function used to calculate the name of a budget planning item.
        /// </summary>
        /// <remarks>
        /// Type parameters:
        /// <list type="bullet">
        /// <item><see cref="IBudgetPlanningItem"/> - The associated budget planning item, if any</item>
        /// <item>The user-supplied name, if any</item>
        /// <item>The returned calculated name</item>
        /// </list>
        /// </remarks>
        public Func<IBudgetPlanningItem, string, string> OnCalculateName { get; set; }

        /// <summary>
        /// Gets or sets the function used to calculate the amount of a budget planning item.
        /// </summary>
        /// <remarks>
        /// Type parameters:
        /// <list type="bullet">
        /// <item><see cref="IBudgetPlanningItem"/> - The associated budget planning item, if any</item>
        /// <item>The user-supplied amount, if any</item>
        /// <item>The returned calculated amount</item>
        /// </list>
        /// </remarks>
        public Func<IBudgetPlanningItem, decimal, decimal> OnCalculateAmount { get; set; }

        public BudgetItemBase()
        {
        }

        public BudgetItemBase(IBudgetPlanningItem budgetPlanningItem)
        {
            BudgetPlanningItem = budgetPlanningItem;
        }

        private string PrivateCalculateName()
        {
            if (OnCalculateName != null)
            {
                return OnCalculateName(BudgetPlanningItem, _name);
            }
            if (!string.IsNullOrEmpty(_name))
            {
                return _name;
            }
            if (BudgetPlanningItem != null && !string.IsNullOrWhiteSpace(BudgetPlanningItem.Name))
            {
                return BudgetPlanningItem.Name;
            }
            return "No Name";
        }

        private decimal PrivateCalculateAmount()
        {
            if (OnCalculateAmount != null)
                return OnCalculateAmount(BudgetPlanningItem, _amount ?? 0m);
            if (_amount.HasValue)
                return _amount.Value;
            if (BudgetPlanningItem != null)
                return BudgetPlanningItem is IExpensePlanningItem
                    ? BudgetPlanningItem.Amount * -1m
                    : BudgetPlanningItem.Amount;
            return _amount ?? 0m;
        }

        public override string ToString() =>
            GetType().Name + ": " + ObjectUtil.DumpToString(this);
    }
}
