using System.Text.RegularExpressions;

using Horseshoe.NET.ObjectsTypesAndValues;

namespace Horseshoe.NET.Mathematics.Finance.Budget
{
    /// <summary>
    /// Defines a specific type of planned transaction that relates and reacts to income planning items.
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    /// <item>
    /// Example: <c>{ Name: "Tax", Amount: "25%", Mode: "Embedded" } (final budgeted amount reflects the disbursement)</c>
    /// </item>
    /// <item>
    /// Example: <c>{ Name: "Tithing", Amount: "10%" [, Mode: "Subsequent" ] } (disbursement appears as separate budget item)</c>
    /// </item>
    /// <item>
    /// Example: <c>{ Name: "Direct Deposit to Savings", Amount: "200.00", Account: "Savings" }</c>
    /// </item>
    /// </list>
    /// </remarks>
    public class AutoDisbursement
    {
        public string Name { get; set; }

        public _Amount Amount { get; set; } = new _Amount(0m, false);

        public Account DestinationAccount { get; set; }

        public ExpenseCategory? Category { get; set; }

        public _Mode Mode { get; set; }

        public decimal Calc(decimal income) => Amount.IsPercent
            ? Amount.Amount / 100m * income
            : Amount.Amount;

        public override string ToString() =>
            GetType().Name + ": " + ObjectUtil.DumpToString(this);

        public class _Amount         
        {
            public decimal Amount { get; }

            public bool IsPercent { get; }

            public _Amount(decimal amount, bool isPercent)
            {
                Amount = amount;
                IsPercent = isPercent;
            }

            public _Amount(string notation)
            {
                Parse(notation, out decimal amount, out bool isPercent);
                Amount = amount;
                IsPercent = isPercent;
            }

            override public string ToString() => IsPercent ? $"{Amount}%" : $"{Amount:C}";

            private static Regex MultiPattern { get; } = new Regex("^[0-9]+(\\.[0-9]+)?[%]?$");   // e.g. "100", "15.75", "10%", "5.25%"

            private static void Parse(string notation, out decimal amount, out bool isPercent)
            {
                Match match = MultiPattern.Match(notation);
                if (!match.Success) 
                    throw new ParseException($"Invalid auto disbursement amount: \"{notation}\". Please see documentation / examples.");
                amount = decimal.Parse(match.Value.TrimEnd('%'));
                isPercent = match.Value.EndsWith("%");
            }

            public static implicit operator _Amount(string notation) => new _Amount(notation);
        }

        public enum _Mode
        {
            Subsequent,
            Embedded
        }
    }
}
