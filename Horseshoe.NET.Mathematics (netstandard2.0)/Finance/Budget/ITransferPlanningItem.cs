namespace Horseshoe.NET.Mathematics.Finance.Budget
{
    public interface ITransferPlanningItem : IBudgetPlanningItem
    {
        Account SourceAccount { get; }
    }
}
