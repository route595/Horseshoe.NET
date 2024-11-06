using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

using Horseshoe.NET;
using Horseshoe.NET.ConsoleX;
using Horseshoe.NET.DateAndTime;
using Horseshoe.NET.Finance;
using Horseshoe.NET.Text;
using Horseshoe.NET.Text.TextGrid;

namespace TestConsole.Finance
{
    class FinanceTests : RoutineX
    {
        public override IList<MenuObject> Menu => new[]
        {
            BuildMenuRoutine
            (
                "Test simple interest calculations",
                () =>
                {
                    Console.WriteLine("$30,000 - 5% - 1 month: " + FinanceEngine.CalculateSimpleInterest(30000, .05m, 1, onAnnualRateConverted: DisplayFormattedConvertedRate));
                    Console.WriteLine("$30,000 - 5% - 1 month - no rounding: " + FinanceEngine.CalculateSimpleInterest(30000, .05m, 1, suppressRounding: true));
                    Console.WriteLine("$30,000 - 5% - 1 month - month rate 5 dec: " + FinanceEngine.CalculateSimpleInterest(30000, .05m, 1, convertedAnnualRateDecimalPoints: 5, onAnnualRateConverted: DisplayFormattedConvertedRate));
                    Console.WriteLine("$30,000 - 5% - 1 month - no rounding - month rate 5 dec: " + FinanceEngine.CalculateSimpleInterest(30000, .05m, 1, suppressRounding: true, convertedAnnualRateDecimalPoints: 5));
                    Console.WriteLine("$30,000 - 5% - 3 months: " + FinanceEngine.CalculateSimpleInterest(30000, .05m, 3));
                    Console.WriteLine("$30,000 - 5% - 1 quarter: " + FinanceEngine.CalculateSimpleInterest(30000, .05m, 1, compoundingPeriod: CompoundingPeriod.Quarterly));
                    Console.WriteLine("$30,000 - 5% - 12 months: " + FinanceEngine.CalculateSimpleInterest(30000, .05m, 12));
                    Console.WriteLine("$30,000 - 5% - 1 year: " + FinanceEngine.CalculateSimpleInterest(30000, .05m, 1, compoundingPeriod: CompoundingPeriod.Yearly));
                    Console.WriteLine("$30,000 - 5% - 4 years: " + FinanceEngine.CalculateSimpleInterest(30000, .05m, 4, compoundingPeriod: CompoundingPeriod.Yearly));
                }
            ),
            BuildMenuRoutine
            (
                "Test Snowball",
                () =>
                {
                    Settings.Snowball.DisplayInterestAndPrincipalColumns = OptionalColumnDisplayPref.IfGreaterThanZero;
                    var accounts = new CreditAccount[]
                    {
                        new CreditAccount { Name = "Credit Card", APR = .1299m, AccountNumber = "*7890", Balance = 10000m, MinimumPaymentAmount = 200m },
                        new CreditAccount { Name = "Family Loan", APR = 0m, Balance = 1000m, MinimumPaymentAmount = 50m, AltList = new[] { new AltCreditAccountPayoffInfo { StartDate = DateUtil.GetMonthStart(), EndDate = DateUtil.GetMonthStart().AddMonths(4), PaymentAmount = 25m } } },
                        new CreditAccount { Name = "Line of Credit", APR = .0624m, Balance = 4000m, MinimumPaymentAmount = 150m },
                        new CreditAccount { Name = "My Store Card", APR = .1999m, Balance = 20000m, MinimumPaymentAmount = 350m, AltList = new[] { new AltCreditAccountPayoffInfo { StartDate = DateUtil.GetMonthStart(), EndDate = DateUtil.GetMonthStart().AddMonths(4), APR = 0m } } }
                    };
                    var projections = new[]
                    {
                        FinanceEngine.GenerateCreditPayoffProjection(accounts),
                        FinanceEngine.GenerateCreditPayoffProjection(accounts, snowballing: true),
                        FinanceEngine.GenerateCreditPayoffProjection(accounts, snowballing: true, snowballOrder : SnowballOrder.APR_Descending),
                        FinanceEngine.GenerateCreditPayoffProjection(accounts, snowballing: true, extraSnowballAmount: 500m),
                        FinanceEngine.GenerateCreditPayoffProjection(accounts, snowballing: true, extraSnowballAmount: 500m, snowballOrder : SnowballOrder.APR_Descending),
                    };
                    TextGrid textGrid = null;
                    var tempFilePath = Path.Combine(Path.GetTempPath(), "Horseshoe.NET.TestConsole.FinanceTest.output.txt");
                    using (var writer = new StreamWriter(File.OpenWrite(tempFilePath)))
                    {
                        foreach (var projection in projections)
                        {
                            writer.WriteLine($"{(projection.Snowballing ? "Snowballing" : "Projecting")} payoff of {accounts.Length} accounts, {(projection.SnowballOrder == SnowballOrder.SameAsSourceCreditAccountCollection ? "not sorted": $"sorted by {projection.SnowballOrder}")}, monthly budget = {projection.MinimumMonthlyBudget:C}{(projection.Snowballing && projection.ExtraSnowballAmount > 0m ? $" + {projection.ExtraSnowballAmount:C} = {projection.TotalMonthlyBudget:C}" : "")}");
                            textGrid = projection.RenderToTextGrid();
                            writer.WriteLine($"Paid {projection.Sum(cap => cap.Account.Balance):C} off in {string.Format("{0:" + textGrid.Columns[0].Format + "}", textGrid.Columns[0].Last())} ({projection.NumberOfMonths} months ({projection.NumberOfMonths / 12m:N2} years)) with a total of {projection.TotalInterest:C} paid in interest.");
                            writer.WriteLine();
                        }
                        writer.WriteLine(textGrid.Render());
                    }
                    Console.WriteLine("Opening temp file...");
                    Process.Start(tempFilePath);
                }
            )
        };

        private static string GetShowingAdditionalOutputPhrase()
        {
            if (Settings.Snowball.DisplayInterestAndPrincipalColumns.In(OptionalColumnDisplayPref.Always, OptionalColumnDisplayPref.IfGreaterThanZero))
                return ", additional output: (Pri, Int)";
            return "";
        }

        private static void DisplayFormattedConvertedRate(decimal rate)
        {
            var strb = new StringBuilder(rate.ToString());
            var pos = strb.ToString().IndexOf('.');
            strb.Remove(pos, 1)
                .Insert(pos + 2, '.')
                .Replace("00.", "0.")
                .Replace("00.", "0.")
                .Append("%");
            RenderX.Alert("Converted Rate: " + strb);
        }
    }
}
