using System.Text;

namespace Horseshoe.NET.Finance
{
    public class MonthlyPaymentInfo
    {
        public decimal PaymentAmount { get; set; }
        public decimal InterestAmount { get; set; }
        public decimal PrincipalAmount => PaymentAmount - InterestAmount;
        public decimal RunningBalance { get; set; }

        public string Render(CreditAccountPayoffInfo cap)
        {
            cap.CalculateOutputWidths();
            return Render(cap.MaxPaymentOutputWidth, cap.DisplayInterestAndPrincipalColumns() ? cap.MaxPrincipalOutputWidth : -1, cap.DisplayInterestAndPrincipalColumns() ? cap.MaxInterestOutputWidth : -1, cap.DisplayBalanceColumn() ? cap.MaxRunningBalanceOutputWidth : -1);
        }

        public string Render(int paymentNumericWidth, int principalNumericWidth, int interestNumericWidth, int runningBalanceNumericWidth)
        {
            var strb = new StringBuilder("$")
                .Append(PaymentAmount.ToString("N2").PadLeft(paymentNumericWidth));
            if (principalNumericWidth > -1)
                strb.Append(" $" + PrincipalAmount.ToString("N2").PadLeft(principalNumericWidth));
            if (interestNumericWidth > -1)
                strb.Append(" $" + InterestAmount.ToString("N2").PadLeft(interestNumericWidth));
            if (runningBalanceNumericWidth > -1)
                strb.Append(" $" + RunningBalance.ToString("N2").PadLeft(runningBalanceNumericWidth));
            return strb.ToString();
        }
    }
}
