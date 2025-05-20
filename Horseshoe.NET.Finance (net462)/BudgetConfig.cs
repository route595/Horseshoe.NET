using Horseshoe.NET.DateAndTime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Horseshoe.NET.Finance
{
    public class BudgetConfig
    {
        public DateTime From { get; set; } = DateUtil.GetMonthStart();
        public DateTime To { get; set; } = DateUtil.GetMonthStart(DateTime.Today.AddMonths(1));
        public decimal StartingAmount { get; set; } 
        public BudgetItemSort Sort { get; set; } = BudgetItemSort.Default;

        public Action<Budget> PeekBudgetPreSort { get; set; }

        public Action<Budget> PeekBudgetPostSortPreCalc { get; set; }
    }
}
