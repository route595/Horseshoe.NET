using System;

namespace Horseshoe.NET.Finance
{
    public class FinanceException : Exception
    {
        public FinanceException(string message) : base(message) { }
        public FinanceException(string message, Exception innerException) : base(message, innerException) { }
    }
}
