using System.Collections.Generic;
using System.Linq;

namespace Horseshoe.NET.Finance
{
    public static class Extensions
    {
        public static string[] GetTitleElements(this CreditAccountPayoffInfo sAccount)
        {
            var list = new List<string>();
            if (sAccount.Account.Name != null)
                list.Add(sAccount.Account.Name);
            if (sAccount.Account.AccountNumber != null)
                list.Add("Acct #: " + sAccount.Account.AccountNumber);
            if (sAccount.Account.APR > 0m)
                list.Add($"APR: {sAccount.Account.APR:P2}");
            if (sAccount.Account.Balance > 0m)
                list.Add($"Balance: {sAccount.Account.Balance:C}");
            if (!list.Any())
                list.Add("[account]");
            return list.ToArray();
        }
    }
}
