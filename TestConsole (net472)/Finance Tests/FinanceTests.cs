﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Horseshoe.NET;
using Horseshoe.NET.ConsoleX;
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
                    Settings.IncludePrincipalPaymentsInSnowballOutput = true;
                    Settings.IncludeInterestPaymentsInSnowballOutput = true;
                    var accounts = new CreditAccount[]
                    {
                            new CreditAccount { Name = "Credit Card", APR = .1299m, AccountNumber = "*7890", Balance = 10000m, MinimumPaymentAmount = 200m },
                            new CreditAccount { Name = "Line of Credit", APR = .0624m, Balance = 4000m, MinimumPaymentAmount = 150m },
                            new CreditAccount { Name = "My Store Card", APR = .1999m, Balance = 20000m, MinimumPaymentAmount = 350m }
                    };
                    var descendingAPR = new CreditAccountSorter { Descending = true, Mode = CreditAccountSortMode.APR };
                    var projections = new[]
                    {
                        FinanceEngine.GenerateCreditPayoffProjection(accounts, sorter: descendingAPR),
                        FinanceEngine.GenerateCreditPayoffProjection(accounts, snowballing: true, sorter: descendingAPR),
                        FinanceEngine.GenerateCreditPayoffProjection(accounts, snowballing: true, extraSnowballAmount: 500m, sorter: descendingAPR),
                    };
                    TextGrid textGrid = null;
                    foreach (var projection in projections)
                    {
                        Console.WriteLine($"{(projection.Snowballing ? "Snowballing" : "Projecting")} payoff of {accounts.Length} accounts, sorting by {projection.Sorter}, monthly budget = {projection.MonthlyBudget:C}");
                        textGrid = projection.RenderToTextGrid();
                        Console.WriteLine($"Paid {projection.Sum(cap => cap.Account.Balance):C} off in {string.Format("{0:" + textGrid.Columns[0].Format + "}", textGrid.Columns[0].Last())} ({projection.NumberOfMonths} months ({projection.NumberOfMonths / 12m:N2} years)) with a total of {projection.TotalInterest:C} paid in interest.");
                        Console.WriteLine();
                    }
                    Console.WriteLine(textGrid.Render());
                    Settings.IncludePrincipalPaymentsInSnowballOutput = false;
                    Settings.IncludeInterestPaymentsInSnowballOutput = false;
                }
            )
        };
        private static string GetShowingAdditionalOutputPhrase()
        {
            var list = new List<string>();
            if (Settings.IncludePrincipalPaymentsInSnowballOutput)
                list.Add("Pri");
            if (Settings.IncludeInterestPaymentsInSnowballOutput)
                list.Add("Int");
            if (list.Any())
                return ", additional output: (" + string.Join(", ", list) + ")";
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
