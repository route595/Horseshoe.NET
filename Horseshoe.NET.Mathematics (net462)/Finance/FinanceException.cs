using System;

namespace Horseshoe.NET.Mathematics.Finance
{
    public class FinanceException : Exception
    {
        public FinanceException(string message) : base(message) { }
        public FinanceException(string message, Exception innerException) : base(message, innerException) { }
    }
}
