using System.Collections.Generic;

namespace Horseshoe.NET.Mathematics.Finance.Budget
{
    public interface IRecurringBudgetPlanningItem
    {
        /// <inheritdoc cref="Recurrence"/>
        Recurrence Recurrence { get; }

        /// <summary>
        /// Variations, if any, to the recurrence that should be taken into account by the budget engine.
        /// </summary>
        IEnumerable<Variance> Variances { get; }
    }
}
