namespace Horseshoe.NET.Finance
{
    public class MortgageAccount : CreditAccount
    {
        public string MortgageProvider { get => FinancialInstitution; set { FinancialInstitution = value; } }
    }
}
