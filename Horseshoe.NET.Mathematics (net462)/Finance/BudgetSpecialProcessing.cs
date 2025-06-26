using System;
using System.Linq;
using Microsoft.Extensions.Primitives;

using Horseshoe.NET.Text;

namespace Horseshoe.NET.Mathematics.Finance
{
    public static class BudgetSpecialProcessing
    {
        public static void CalculateTithingOnAllIncome(Budget budget, decimal tithingPercent = 0.1m)
        {
            for (int i = 0; i < budget.Count; i++)
            {
                if (budget[i].Amount > 0)
                {
                    budget.Insert(i + 1, new BudgetItem
                    {
                        Name = "Tithing",
                        Amount = budget[i].Amount * tithingPercent * -1,
                        TransactionDate = budget[i].TransactionDate
                    });
                }
            }
        }

        public static void CalculateTithingOnIncomeByBudgetItemName(Budget budget, StringValues budgetItemNames, bool ignoreCase = false, decimal tithingPercent = 0.1m)
        {
            for (int i = 0; i < budget.Count; i++)
            {
                if (budget[i].Amount > 0 && budgetItemNames.Contains(budget[i].Name, ignoreCase ? CaseInsensitiveStringEqualityComparer.Default : StringEqualityComparer.Default))
                {
                    budget.Insert(i + 1, new BudgetItem
                    {
                        Name = "Tithing",
                        Amount = budget[i].Amount * tithingPercent * -1,
                        TransactionDate = budget[i].TransactionDate
                    });
                }
            }
        }
    }
}
