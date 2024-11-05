using System;
using System.Collections.Generic;

namespace Horseshoe.NET.Finance
{
    public class CreditAccount : Account, IEquatable<CreditAccount>
    {
        internal Guid Id { get; }
        public decimal APR { get; set; }
        public decimal MinimumPaymentAmount { get; set; }
        public IEnumerable<AltCreditAccountPayoffInfo> AltList { get; set; }

        public CreditAccount()
        {
            Id = Guid.NewGuid();
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public bool Equals(CreditAccount other)
        {
            return !(other is null) &&
                   Id == other.Id;
        }

        public override int GetHashCode()
        {
            int hashCode = 1404940613;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Name);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(AccountNumber);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(FinancialInstitution);
            hashCode = hashCode * -1521134295 + Balance.GetHashCode();
            hashCode = hashCode * -1521134295 + APR.GetHashCode();
            hashCode = hashCode * -1521134295 + MinimumPaymentAmount.GetHashCode();
            return hashCode;
        }

        public static bool operator ==(CreditAccount left, CreditAccount right)
        {
            return EqualityComparer<Guid>.Default.Equals(left.Id, right.Id);
        }

        public static bool operator !=(CreditAccount left, CreditAccount right)
        {
            return !(left == right);
        }
    }
}
