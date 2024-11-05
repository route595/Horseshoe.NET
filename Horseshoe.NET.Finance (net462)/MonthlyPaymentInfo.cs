using System;
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

        public string Render(CreditAccountPayoffInfo capi)
        {
            capi.CalculateOutputWidths();
            return Render(capi.MaxPaymentOutputWidth, capi.DisplayInterestAndPrincipalColumns() ? capi.MaxPrincipalOutputWidth : -1, capi.DisplayInterestAndPrincipalColumns() ? capi.MaxInterestOutputWidth : -1, capi.MaxRunningBalanceOutputWidth);
        }

        public string Render(int paymentNumericWidth, int principalNumericWidth, int interestNumericWidth, int runningBalanceNumericWidth)
        {
            var strb = new StringBuilder("$")
                .Append(PaymentAmount.ToString("N2").PadLeft(paymentNumericWidth));
            if (principalNumericWidth > -1)
                strb.Append(" $" + PrincipalAmount.ToString("N2").PadLeft(principalNumericWidth));
            if (interestNumericWidth > -1)
                strb.Append(" $" + InterestAmount.ToString("N2").PadLeft(interestNumericWidth));
            strb.Append(" $" + RunningBalance.ToString("N2").PadLeft(runningBalanceNumericWidth));
            return strb.ToString();
        }
    }
}
