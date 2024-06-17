using System;

namespace Horseshoe.NET.Finance
{
    public class CreditAccountSorter
    {
        public CreditAccountSortMode Mode { get; set; }
        public bool Descending { get; set; }
        public Func<CreditAccount, decimal> ByDecimal { get; set; }

        public override string ToString()
        {
            if (ByDecimal != null)
                return "Customer Supplied (decimal)";
            if (Descending)
                return "descending " + Mode;
            return Mode.ToString();
        }
    }
}
