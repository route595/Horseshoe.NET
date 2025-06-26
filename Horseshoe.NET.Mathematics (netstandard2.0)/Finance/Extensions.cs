using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Primitives;

using Horseshoe.NET.Text.TextGrid;
using System.Text.RegularExpressions;

namespace Horseshoe.NET.Mathematics.Finance
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

        public static void FormatDecimalColumnsAsCurrency(this TextGrid textGrid, int? decimalDigits = null)
        {
            int _decimalDigits = decimalDigits ?? System.Globalization.CultureInfo.CurrentCulture.NumberFormat.CurrencyDecimalDigits;
            textGrid.FormatColumnsByType(typeof(decimal), "C" + _decimalDigits);
        }

        public static void FormatDecimalColumnsAsCurrency_Custom(this TextGrid textGrid, int? decimalDigits = null)
        {
            //int _decimalDigits = decimalDigits ?? System.Globalization.CultureInfo.CurrentCulture.NumberFormat.CurrencyDecimalDigits;
            textGrid.FormatColumnsByType
            (
                typeof(decimal),
                value => 
                {
                    if (value is decimal decimalValue)
                        return Math.Abs(decimalValue).ToString("C") + (decimalValue > 0 ? '+' : ' ');
                    return value.ToString(); 
                }
                //decimalDigits.HasValue ? "C" + decimalDigits : "C",
                //customPostRenderer: (formattedStr, origValue) =>
                //{
                //    if (origValue is decimal dec && dec >= 0m)
                //        formattedStr = "+ " + formattedStr + " ";
                //    return formattedStr;
                //}
            );
            textGrid.FormatColumnsByTitle
            (
                "Running Total",
                value => 
                {
                    if (value is decimal decimalValue)
                        return Math.Abs(decimalValue).ToString("C") + (decimalValue < 0 ? '-' : ' ');
                    return value.ToString(); 
                }
                //decimalDigits.HasValue ? "C" + decimalDigits : "C",
                //customPostRenderer: (formattedStr, origValue) =>
                //{
                //    if (origValue is decimal dec && dec > 0m)
                //        formattedStr += " ";
                //    return formattedStr;
                //}
            );
        }
    }
}
