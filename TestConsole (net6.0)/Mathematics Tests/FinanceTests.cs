using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

using Horseshoe.NET;
using Horseshoe.NET.ConsoleX;
using Horseshoe.NET.DateAndTime;
using Horseshoe.NET.Mathematics.Finance;
using Horseshoe.NET.Text.TextGrid;

namespace TestConsole.MathematicsTests
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
                    accounts = PromptX.List
                    (
                        accounts,
                        renderer: act => act.Name + " " + act.Balance.ToString("C") + " " + act.APR.ToString("P2"),
                        selectionMode: ListSelectionMode.OneOrMore
                    ).SelectedItems;
                    var projections = new[]
                    {
                        FinanceEngine.GenerateCreditPayoffProjection(accounts),
                        FinanceEngine.GenerateCreditPayoffProjection(accounts, snowballing: true),
                        FinanceEngine.GenerateCreditPayoffProjection(accounts, snowballing: true, snowballOrder : SnowballOrder.APR_Descending),
                        FinanceEngine.GenerateCreditPayoffProjection(accounts, snowballing: true, extraSnowballAmount: 100m),
                        FinanceEngine.GenerateCreditPayoffProjection(accounts, snowballing: true, extraSnowballAmount: 100m, snowballOrder : SnowballOrder.APR),
                    };
                    TextGrid textGrid = null;
                    var tempFilePath = Path.Combine(Path.GetTempPath(), "Horseshoe.NET.TestConsole.FinanceTest.output.txt");
                    using (var writer = new StreamWriter(File.OpenWrite(tempFilePath)))
                    {
                        foreach (var projection in projections)
                        {
                            writer.WriteLine($"{(projection.Snowballing ? "Snowballing" : "Projecting")} payoff of {accounts.Length} accounts, {(projection.SnowballOrder == SnowballOrder.SameAsSourceCreditAccountCollection ? "not sorted": $"sorted by {projection.SnowballOrder}")}, monthly budget = {projection.MinimumMonthlyBudget:C}{(projection.Snowballing && projection.ExtraSnowballAmount > 0m ? $" + {projection.ExtraSnowballAmount:C} = {projection.TotalMonthlyBudget:C}" : "")}");
                            textGrid = projection.RenderToTextGrid();
                            writer.WriteLine($"Paid {projection.Sum(cap => cap.Account.Balance):C} off in {string.Format("{0:" + textGrid.Columns[0].Formatter?.DisplayFormat + "}", textGrid.Columns[0].List.Last())} ({projection.NumberOfMonths} months ({projection.NumberOfMonths / 12m:N2} years)) with a total of {projection.TotalInterest:C} paid in interest.");
                            writer.WriteLine();
                        }
                        writer.WriteLine(textGrid.Render());
                    }
                    Console.WriteLine("Opening temp file...");
                    Process.Start(tempFilePath);
                }
            ),
            BuildMenuRoutine
            (
                "Simple Budget",
                () =>
                {
                    var planningItems = new List<BudgetPlanningItem>
                    {
                        new BudgetPlanningItem { Name = "Bill Payment #1", Amount = -30m, TransactionDate = new DateTime(2025, 1, 1), Recurrence = "m" },
                        new BudgetPlanningItem { Name = "Bill Payment #2", Amount = -60m, TransactionDate = new DateTime(2025, 1, 2), Recurrence = "m" },
                        new BudgetPlanningItem { Name = "Bill Payment #3", Amount = -90m, TransactionDate = new DateTime(2025, 1, 3), Recurrence = "m" },
                        new BudgetPlanningItem { Name = "Bill Payment #4", Amount = -120m, TransactionDate = new DateTime(2025, 1, 4), Recurrence = "m" },

                        new BudgetPlanningItem { Name = "Paycheck", Amount = 3000m, Recurrence = "w-2", TransactionDate = new DateTime(2025, 3, 21) },
                        //new BudgetPlanningItem { Name = "Tithing", Amount = -300m, Recurrence = "w-2", TransactionDate = new DateTime(2025, 3, 21) },
                        new BudgetPlanningItem { Name = "Retirement Pension", Amount = 900m, Recurrence = "last", TransactionDate = new DateTime(2025, 1, 1) },
                        new BudgetPlanningItem { Name = "Mortgage", Amount = -1000m, Recurrence = "m", TransactionDate = new DateTime(2025, 1, 1) },
                        new BudgetPlanningItem { Name = "Insurance", Amount = -200m, Recurrence = "m", TransactionDate = new DateTime(2025, 1, 1) },
                        new BudgetPlanningItem { Name = "Grocery allowance ($400/mo)", Amount = -200m, Recurrence = "1,15", TransactionDate = new DateTime(2025, 1, 1) },
                        new BudgetPlanningItem { Name = "Gas allowance ($300/mo)", Amount = -150m, Recurrence = "1,15", TransactionDate = new DateTime(2025, 1, 1) },
                        new BudgetPlanningItem { Name = "Eating out allowance ($150/mo)", Amount = -75m, Recurrence = "1,15", TransactionDate = new DateTime(2025, 1, 1) },
                        new BudgetPlanningItem { Name = "Amazon allowance ($60/mo)", Amount = -30m, Recurrence = "1,15", TransactionDate = new DateTime(2025, 1, 1) },

                        new BudgetPlanningItem { Name = "Bill Payment #5", Amount = -30m, TransactionDate = new DateTime(2025, 1, 1), Recurrence = "m" },
                        new BudgetPlanningItem { Name = "Bill Payment #6", Amount = -60m, TransactionDate = new DateTime(2025, 1, 2), Recurrence = "m" },
                        new BudgetPlanningItem { Name = "Bill Payment #7", Amount = -90m, TransactionDate = new DateTime(2025, 1, 3), Recurrence = "m" },
                        new BudgetPlanningItem { Name = "Bill Payment #8", Amount = -120m, TransactionDate = new DateTime(2025, 1, 4), Recurrence = "m" },
                    };
                    var config = new BudgetConfig
                    {
                        From = new DateTime(2025, 4, 1),
                        To = new DateTime(2025, 6, 1).AddDays(-1),
                        StartingAmount = 2000m,
                        PeekBudgetPreSort = _budget => _budget.ItemSort = BudgetItemSort.IncomeFirst,
                        PeekBudgetPostSortPreCalc = _budget => BudgetSpecialProcessing.CalculateTithingOnIncomeByBudgetItemName(_budget, new[] { "Paycheck", "VA Pension" })
                    };

                    // generate the budget
                    var budget = Budget.GenerateSimpleBudget
                    (
                        planningItems, 
                        config: config
                    );

                    // prepare to configure TextGrid
                    void configureGrid (TextGrid grid)
                    {
                        var dateCol = grid.GetColumnByTitle("TransactionDate");
                        if (dateCol != null)
                        {
                            dateCol.Formatter = "MMM d";
                            grid.Columns.Remove(dateCol);
                            grid.Columns.Insert(0, dateCol);
                        }
                        grid.FormatDecimalColumnsAsCurrency_Custom();
                    }

                    // output the budget in a configured TextGrid
                    Console.WriteLine($"Budget from {budget.From:MMM d, yyyy} to {budget.To:MMM d, yyyy} starting with {budget.StartingAmount:C2}.");
                    Console.WriteLine();
                    Console.Write(TextGrid.FromCollection(budget).Render(configureGrid: configureGrid));
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
