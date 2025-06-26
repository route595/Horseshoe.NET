using System;

namespace Horseshoe.NET.Mathematics.Finance
{
    public class BudgetItem
    {
        public string Name { get; set; }
        public decimal Amount { get; set; }
        public DateTime TransactionDate { get; set; }
        public decimal RunningTotal { get; set; }
    }
}
