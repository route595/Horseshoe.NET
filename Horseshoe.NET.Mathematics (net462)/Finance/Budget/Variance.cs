using System;

namespace Horseshoe.NET.Mathematics.Finance.Budget
{
    /// <summary>
    /// A variance in a recurring budget planning item.
    /// </summary>
    public readonly struct Variance
    {
        public DateTime Date { get; }

        public decimal Amount { get; }

        /// <summary>
        /// Creates a variance in a recurring budget planning item.
        /// </summary>
        /// <param name="daysOfMonth"></param>
        public Variance(DateTime date, decimal amount)
        {
            Date = date;
            Amount = amount;
        }
    }
}
