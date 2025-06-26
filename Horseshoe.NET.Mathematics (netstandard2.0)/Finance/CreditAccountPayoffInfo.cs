using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Horseshoe.NET.Mathematics.Finance
{
    public class CreditAccountPayoffInfo : List<MonthlyPaymentInfo>
    {
        public CreditAccount Account { get; }
        public decimal MinimumPaymentAmount => Account.MinimumPaymentAmount;
        public bool Snowballing { get; }
        internal bool IsTotalColumn { get; }
        public decimal RunningBalance => this.LastOrDefault()?.RunningBalance ?? Account.Balance;
        //public decimal CurrentCycleInterestAmount => CalculateInterestAmount();
        //public decimal CurrentCyclePaymentAmount => CalculatePaymentAmount();
        internal int MaxPaymentOutputWidth { get; set; }        // 1 + 6  =>   "$" +   "600.00" (also  "$" +   " 60.00")  =>     "$600.00" (also    "$ 60.00")
        internal int MaxInterestOutputWidth { get; set; }       // 2 + 5  =>  " $" +    "60.00" (also " $" +    " 6.00")  =>     " $60.00" (also    " $ 6.00")
        internal int MaxPrincipalOutputWidth { get; set; }      // 2 + 6  =>  " $" +   "540.00" (also " $" +   " 54.00")  =>    " $540.00" (also   " $ 54.00")
        internal int MaxRunningBalanceOutputWidth { get; set; } // 2 + 8  =>  " $" + "6,000.00" (also " $" + "  600.00")  =>  " $6,000.00" (also " $  600.00")
        internal int MaxOutputWidth => 1 + MaxPaymentOutputWidth + (DisplayInterestAndPrincipalColumns() ? 2 + MaxInterestOutputWidth + 2 + MaxPrincipalOutputWidth : 0) + 2 + MaxRunningBalanceOutputWidth;

        public CreditAccountPayoffInfo(CreditAccount account, bool snowballing)
        {
            Account = account;
            Snowballing = snowballing;
        }

        internal CreditAccountPayoffInfo(CreditAccount account, bool snowballing, bool isTotalColumn)
        {
            Account = account;
            Snowballing = snowballing;
            IsTotalColumn = isTotalColumn;
        }

        public decimal CalculateCurrentCycleInterest(DateTime date)
        {
            if (Snowballing && Account.AltList != null)
            {
                foreach (var acap in Account.AltList)
                {
                    if (date >= acap.StartDate && date <= acap.EndDate && acap.APR is decimal apr)
                    {
                        return FinanceEngine.CalculateSimpleInterest(RunningBalance, apr, 1, compoundingPeriod: CompoundingPeriod.Monthly);
                    }
                }
            }
            return FinanceEngine.CalculateSimpleInterest(RunningBalance, Account.APR, 1, compoundingPeriod: CompoundingPeriod.Monthly);
        }

        public decimal CalculateCurrentCyclePayment(DateTime date)
        {
            decimal p = RunningBalance, i = CalculateCurrentCycleInterest(date);
            if (Snowballing && Account.AltList != null)
            {
                foreach (var acap in Account.AltList)
                {
                    if (date >= acap.StartDate && date <= acap.EndDate && acap.PaymentAmount is decimal paymentAmount)
                    {
                        if (paymentAmount > p + i)
                            return p + i;
                        return paymentAmount;
                    }
                }
            }
            if (Account.MinimumPaymentAmount > p + i)
                return p + i;
            return Account.MinimumPaymentAmount;
        }

        internal bool DisplayInterestAndPrincipalColumns()
        {
            if (Settings.Snowball.DisplayInterestAndPrincipalColumns == OptionalColumnDisplayPref.Always || IsTotalColumn)
                return true;
            if (Settings.Snowball.DisplayInterestAndPrincipalColumns == OptionalColumnDisplayPref.IfGreaterThanZero && this.Any() && this.Max(mp => mp.InterestAmount) > 0m)
                return true;
            return false;
        }

        internal bool DisplayBalanceColumn()
        {
            return !IsTotalColumn;
        }

        internal void CalculateOutputWidths()
        {
            MaxPaymentOutputWidth = (this.Any() ? this.Max(mp => mp.PaymentAmount) : 0m).ToString("N2").Length;
            if (DisplayInterestAndPrincipalColumns())
                MaxInterestOutputWidth = this.Max(mp => mp.InterestAmount).ToString("N2").Length;
            if (DisplayInterestAndPrincipalColumns())
                MaxPrincipalOutputWidth = this.Max(mp => mp.PrincipalAmount).ToString("N2").Length;
            if (DisplayBalanceColumn())
                MaxRunningBalanceOutputWidth = (this.Any() ? this.Max(mp => mp.RunningBalance) : 0m).ToString("N2").Length;
        }

        public string RenderSubColumnTitles()
        {
            var strb = new StringBuilder()
                .Append("Pmt".PadRight(MaxPaymentOutputWidth + 1));
            if (DisplayInterestAndPrincipalColumns())
            {
                strb
                    .Append(' ')
                    .Append("Pri".PadRight(MaxPrincipalOutputWidth + 1))
                    .Append(' ')
                    .Append("Int".PadRight(MaxInterestOutputWidth + 1));
            }
            if (DisplayBalanceColumn())
            {
                strb
                    .Append(' ')
                    .Append("Bal".PadRight(MaxRunningBalanceOutputWidth + 1));
            }
            return strb.ToString();
        }
    }
}
