namespace Horseshoe.NET.Finance
{
    public abstract class Account
    {
        public string Name { get; set; }
        public string AccountNumber { get; set; }
        public string FinancialInstitution { get; set; }
        public decimal Balance { get; set; }
    }
}
