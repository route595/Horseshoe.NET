using System;

using Horseshoe.NET.ObjectsTypesAndValues;
using Horseshoe.NET.Text;

namespace Horseshoe.NET.Mathematics.Finance.Budget
{
    public class BudgetItem : BudgetItemBase
    {
        private Account _account;
        public string _category;

        public Account Account
        {
            get => _account ?? BudgetPlanningItem?.Account ?? Account.Default;
            set { _account = value; }
        }

        public string Category
        {
            get => PrivateCalculateCategory();
            set { _category = value; }
        }

        /// <summary>
        /// Gets or sets the function used to calculate the category of a budget planning item.
        /// </summary>
        /// <remarks>
        /// Type parameters:
        /// <list type="bullet">
        /// <item><see cref="IBudgetPlanningItem"/> - The associated budget planning item, if any</item>
        /// <item>The user-supplied category, if any</item>
        /// <item>The returned calculated category</item>
        /// </list>
        /// </remarks>
        public Func<IBudgetPlanningItem, string, string> OnCalculateCategory { get; set; }

        public BudgetItem()
        {
        }

        public BudgetItem(IBudgetPlanningItem budgetPlanningItem) : base(budgetPlanningItem)
        {
        }

        private string PrivateCalculateCategory()
        {
            if (OnCalculateCategory != null)
            {
                return OnCalculateCategory(BudgetPlanningItem, _category);
            }
            if (!string.IsNullOrEmpty(_category))
            {
                return _category;
            }
            if (BudgetPlanningItem is IExpensePlanningItem expensePI)
            {
                return TextUtil.SpaceOutTitleCase(expensePI.Category.ToString());
            }
            if (BudgetPlanningItem is IIncomePlanningItem incomePI)
            {
                return TextUtil.SpaceOutTitleCase(incomePI.Category.ToString());
            }
            if (BudgetPlanningItem != null && BudgetPlanningItem.ParentBudgetPlanningItem is ITransferPlanningItem)
            {
                return I18nService.Get(I18nKey.BankTransfer);
            }
            return null;
        }

        public override string ToString() =>
            GetType().Name + ": " + ObjectUtil.DumpToString(this);
    }
}
