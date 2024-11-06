using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Primitives;

namespace Horseshoe.NET.Finance
{
    public static class Extensions
    {
        public static StringValues GetTitleElements(this CreditAccountPayoffInfo cap)
        {
            if (cap.IsTotalColumn)
                return "Totals";
            
            var list = new List<string>();
            if (cap.Account.Name != null)
                list.Add(cap.Account.Name);
            if (cap.Account.AccountNumber != null)
                list.Add("Acct #: " + cap.Account.AccountNumber);
            if (cap.Account.APR > 0m)
                list.Add($"APR: {cap.Account.APR:P2}{(cap.Account.Balance > 0m && cap.Account.APR > 0m ? $" ({FinanceEngine.CalculateSimpleInterest(cap.Account.Balance, cap.Account.APR, 1):C})" : "")}");
            if (cap.Account.Balance > 0m)
                list.Add($"Balance: {cap.Account.Balance:C}");
            if (cap.Account.MinimumPaymentAmount > 0m)
                list.Add($"Min Payment: {cap.Account.MinimumPaymentAmount:C}");
            if (list.Any())
                return list.ToArray();
            return "[account]";
        }
    }
}
