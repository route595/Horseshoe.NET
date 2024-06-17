using System.Collections.Generic;
using System.Linq;
using System.Text;

using Horseshoe.NET.Text;

namespace Horseshoe.NET.Finance
{
    public class CreditAccountPayoffInfo : List<MonthlyPaymentInfo>
    {
        public CreditAccount Account { get; set; }
        public decimal RunningBalance => this.Any() ? this.Last().RunningBalance : Account.Balance;
        public decimal MonthlyInterestAmount => FinanceEngine.CalculateSimpleInterest(RunningBalance, Account.APR, 1, compoundingPeriod: CompoundingPeriod.Monthly);
        public decimal MinimumPaymentAmount => CalculateMinimumPaymentAmount();
        internal int MaxPaymentNumericOutputWidth { get; set; }        //    "$600.00"       "$ 60.00"    =>   1 + (6)
        internal int MaxInterestNumericOutputWidth { get; set; }       //    " $60.00"       " $ 6.00"    =>   2 + (5)
        internal int MaxPrincipalNumericOutputWidth { get; set; }      //    " $60.00"       " $ 6.00"    =>   2 + (5)
        internal int MaxRunningBalanceNumericOutputWidth { get; set; } // " $6,000.00"    " $  600.00"    =>   2 + (8)
        internal int MaxOutputWidth => 1 + MaxPaymentNumericOutputWidth + (Settings.IncludeInterestPaymentsInSnowballOutput ? 2 + MaxInterestNumericOutputWidth : 0) + 2 + MaxRunningBalanceNumericOutputWidth;

        public CreditAccountPayoffInfo(CreditAccount account)
        {
            Account = account;
        }

        private decimal CalculateMinimumPaymentAmount()
        {
            decimal p = RunningBalance,
                i = MonthlyInterestAmount;
            if (Account.MinimumPaymentAmount > p + i)
                return p + i;
            return Account.MinimumPaymentAmount;
        }

        internal void CalculateOutputWidths()
        {
            MaxPaymentNumericOutputWidth = (this.Any() ? this.Max(mp => mp.PaymentAmount) : 0m).ToString("N2").Length;
            if (Settings.IncludeInterestPaymentsInSnowballOutput)
                MaxInterestNumericOutputWidth = (this.Any() ? this.Max(mp => mp.InterestAmount) : 0m).ToString("N2").Length;
            if (Settings.IncludePrincipalPaymentsInSnowballOutput)
                MaxPrincipalNumericOutputWidth = (this.Any() ? this.Max(mp => mp.PrincipalAmount) : 0m).ToString("N2").Length;
            MaxRunningBalanceNumericOutputWidth = (this.Any() ? this.Max(mp => mp.RunningBalance) : 0m).ToString("N2").Length;
        }

        public string RenderSubColumnTitles()
        {
            return new StringBuilder()
                .Append("Pmt".PadRight(MaxPaymentNumericOutputWidth + 1))
                .AppendIf(Settings.IncludePrincipalPaymentsInSnowballOutput, ' ')
                .AppendIf(Settings.IncludePrincipalPaymentsInSnowballOutput, "Pri".PadRight(MaxPrincipalNumericOutputWidth + 1))
                .AppendIf(Settings.IncludeInterestPaymentsInSnowballOutput, ' ')
                .AppendIf(Settings.IncludeInterestPaymentsInSnowballOutput, "Int".PadRight(MaxInterestNumericOutputWidth + 1))
                .Append(' ')
                .Append("Bal".PadRight(MaxRunningBalanceNumericOutputWidth + 1))
                .ToString();
        }
    }
}
