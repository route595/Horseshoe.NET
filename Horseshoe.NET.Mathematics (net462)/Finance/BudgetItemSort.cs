using System.Collections.Generic;

namespace Horseshoe.NET.Mathematics.Finance
{
    public class BudgetItemSort : IComparer<BudgetItem>
    {
        public const int DateOrderAscending = 0;
        //public const int DateOrderDescending = 1;  // not used
        public const int AdditionalSortingNone = 0;
        public const int AdditionalSortingIncomeFirst = 1;

        public int DateOrder { get; }

        public int AdditionalSorting { get; }

        public BudgetItemSort() : this(DateOrderAscending, AdditionalSortingNone) { }

        public BudgetItemSort(int dateOrder, int additionalSorting)
        {
            DateOrder = dateOrder;
            AdditionalSorting = additionalSorting;
        }

        public int Compare(BudgetItem x, BudgetItem y)
        {
            var dateComparison = x.TransactionDate.CompareTo(y.TransactionDate);
            if (dateComparison != 0) 
                return dateComparison;
            if (x.Amount < 0)
            {
                if (y.Amount >= 0)
                    return 1;
            }
            else if (y.Amount < 0)
            {
                return -1;
            }
            return 0;
        }

        public static BudgetItemSort Default { get; } = new BudgetItemSort();

        public static BudgetItemSort IncomeFirst { get; } = new BudgetItemSort(DateOrderAscending, AdditionalSortingIncomeFirst);
    }
}
