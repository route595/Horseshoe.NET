namespace Horseshoe.NET.Mathematics.Finance
{
    public class MortgageAccount : CreditAccount
    {
        public string MortgageProvider { get => FinancialInstitution; set { FinancialInstitution = value; } }
    }
}
