using System;

using Horseshoe.NET.ObjectsTypesAndValues;

namespace Horseshoe.NET.Mathematics.Finance.Budget
{
    /// <summary>
    /// Designed strictly as a parent class for budget planning items
    /// </summary>
    public class BudgetPlanningItem : IBudgetPlanningItem
    {
        public IBudgetPlanningItem ParentBudgetPlanningItem { get; set; }

        public string Name { get; set; }

        public Account Account { get; set; } = Account.Default;

        public decimal Amount { get; set; }

        public override string ToString() =>
            GetType().Name + ": " + ObjectUtil.DumpToString(this, propertiesToInclude: new[] { "Name", "Account", "Amount" });

    }
}
