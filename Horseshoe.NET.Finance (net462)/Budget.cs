using System;
using System.Collections.Generic;

namespace Horseshoe.NET.Finance
{
    public class Budget : List<BudgetItem>
    {
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        public decimal? StartingAmount { get; set; }
        public BudgetItemSort ItemSort { get; set; } = BudgetItemSort.Default;

        public static Budget GenerateSimpleBudget(IEnumerable<BudgetPlanningItem> pItems, BudgetConfig config = null)
        {
            config = config ?? new BudgetConfig();

            var budget = new Budget { From = config.From, To = config.To, StartingAmount = config.StartingAmount };
            DateTime tDate;
            foreach (var pItem in pItems)
            {
                tDate = pItem.TransactionDate;
                while (tDate <= config.To)
                {
                    if (tDate >= config.From)
                        budget.Add(new BudgetItem { Name = pItem.Name, Amount = pItem.Amount, TransactionDate = tDate });
                    tDate = pItem.Recurrence.Next(tDate);
                }
            }

            config.PeekBudgetPreSort?.Invoke(budget);  // can set ItemSort to IncomeFirst, for example

            budget.Sort(budget.ItemSort);

            config.PeekBudgetPostSortPreCalc?.Invoke(budget);   // can calculate tithings and add next to income budget items, for example

            var startingAmount = config.StartingAmount;

            foreach (var bItem in budget)
            {
                bItem.RunningTotal = startingAmount += bItem.Amount;
            }

            return budget;
        }
    }
}
