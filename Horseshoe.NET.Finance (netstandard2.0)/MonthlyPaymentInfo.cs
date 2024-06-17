using System.Text;

using Horseshoe.NET.Text;

namespace Horseshoe.NET.Finance
{
    public class MonthlyPaymentInfo
    {
        public decimal PaymentAmount { get; set; }
        public decimal InterestAmount { get; set; }
        public decimal PrincipalAmount => PaymentAmount - InterestAmount;
        public decimal RunningBalance { get; set; }

        public string Render(CreditAccountPayoffInfo sAccount)
        {
            sAccount.CalculateOutputWidths();
            return Render(sAccount.MaxPaymentNumericOutputWidth, sAccount.MaxPrincipalNumericOutputWidth, sAccount.MaxInterestNumericOutputWidth, sAccount.MaxRunningBalanceNumericOutputWidth);
        }

        public string Render(int paymentNumericWidth, int principalNumericWidth, int interestNumericWidth, int runningBalanceNumericWidth)
        {
            return new StringBuilder("$")
                .Append(PaymentAmount.ToString("N2").PadLeft(paymentNumericWidth))
                .AppendIf(Settings.IncludePrincipalPaymentsInSnowballOutput, " $")
                .AppendIf(Settings.IncludePrincipalPaymentsInSnowballOutput, PrincipalAmount.ToString("N2").PadLeft(principalNumericWidth))
                .AppendIf(Settings.IncludeInterestPaymentsInSnowballOutput, " $")
                .AppendIf(Settings.IncludeInterestPaymentsInSnowballOutput, InterestAmount.ToString("N2").PadLeft(interestNumericWidth))
                .Append(" $")
                .Append(RunningBalance.ToString("N2").PadLeft(runningBalanceNumericWidth))
                .ToString();
        }
    }
}
