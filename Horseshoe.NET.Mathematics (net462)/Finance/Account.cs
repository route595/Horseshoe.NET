using System;

using Horseshoe.NET.ObjectsTypesAndValues;

namespace Horseshoe.NET.Mathematics.Finance
{
    public abstract class Account : IEquatable<Account>
    {
        public string Name { get; set; }

        public string AccountNumber { get; set; }

        public string FinancialInstitution { get; set; }

        /// <summary>
        /// The balance of the account.
        /// Use this property to record the starting balance of an account when using it in a budget calculation.
        /// </summary>
        public decimal Balance { get; set; }

        public override string ToString() =>
            GetType().Name + ": " + ObjectUtil.DumpToString(this);

        public override bool Equals(object other)
        {
            if (ReferenceEquals(this, other))
                return true;
            if (other is Account account)
                return _Equals(account);
            return false;
        }

        public bool Equals(Account other)
        {
            if (ReferenceEquals(this, other))
                return true;
            return _Equals(other);
        }

        private bool _Equals(Account other)
        {
            if (!Equals(GetType(), other.GetType()))
                return false;
            if (!string.Equals(Name, other.Name))
                return false;
            if (!string.Equals(AccountNumber, other.AccountNumber))
                return false;
            if (!string.Equals(FinancialInstitution, other.FinancialInstitution))
                return false;
            //if (Balance != other.Balance)
            //    return false;
            return true;
        }

        public static bool operator ==(Account left, Account right)
        {
            if (ReferenceEquals (left, right)) 
                return true;

            if (ReferenceEquals(left, null) || ReferenceEquals(right, null))
                return false;

            return left._Equals(right);
        }

        public static bool operator !=(Account left, Account right) => !(left == right);

        public static Account Default { get; } = new BankAccount { Name = "Default Account" };
    }
}
