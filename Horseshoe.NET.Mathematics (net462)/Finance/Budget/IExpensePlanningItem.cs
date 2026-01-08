namespace Horseshoe.NET.Mathematics.Finance.Budget
{
    public interface IExpensePlanningItem : IBudgetPlanningItem
    {
        ExpenseCategory Category { get; }
    }
}
