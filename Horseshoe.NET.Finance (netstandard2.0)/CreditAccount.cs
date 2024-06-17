namespace Horseshoe.NET.Finance
{
    public class CreditAccount : Account
    {
        public decimal CreditLimit { get; set; }
        public decimal APR { get; set; }
        public decimal MinimumPaymentAmount { get; set; }
    }
}
