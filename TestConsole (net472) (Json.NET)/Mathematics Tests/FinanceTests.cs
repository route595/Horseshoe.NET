using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

using Horseshoe.NET.ConsoleX;
using Horseshoe.NET.DateAndTime;
using Horseshoe.NET.Mathematics.Finance;
using Horseshoe.NET.Mathematics.Finance.Budget;
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
                        new CreditAccount { Name = "Family Loan", APR = 0m, Balance = 1000m, MinimumPaymentAmount = 50m, AltList = new[] { new AltCreditAccountPayoffInfo { StartDate = DateUtil.MonthStart(), EndDate = DateUtil.MonthStart().AddMonths(4), PaymentAmount = 25m } } },
                        new CreditAccount { Name = "Line of Credit", APR = .0624m, Balance = 4000m, MinimumPaymentAmount = 150m },
                        new CreditAccount { Name = "My Store Card", APR = .1999m, Balance = 20000m, MinimumPaymentAmount = 350m, AltList = new[] { new AltCreditAccountPayoffInfo { StartDate = DateUtil.MonthStart(), EndDate = DateUtil.MonthStart().AddMonths(4), APR = 0m } } }
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
                "Recurrences",
                () =>
                {
        // e.g. "y", "1y", "m", "2w", "14d", etc.
        // e.g. "bimonthly", "last", "5", "1,15", "15,last", "15,{month.length-1}", etc.
        // e.g. "m/d" (same day of every month based on TransactionDate), "m/16" or "m/last" or "m/{month.length-1}" (16th or last or second to last day of every month), "8/5" (every year on Aug 5)

                    var inputs = new List<string>
                    {
                        "m",
                        "[name=startDate,value=20250115]m",
                        "y",
                        "2w",
                        "[name=startDate,value=20241201]2w",
                        "[name=endDate,value=20261201]2w",
                        "14d",
                        "d14",
                        "x",
                        "bimonthly",
                        "last",
                        "5",
                        "1,10,20",
                        "20,30,40",
                        "15,last",
                        "15,{month.length}",
                        "15,{monkey.google}",
                        "15,{month.length-1}",
                        "m/d",
                        "mm/dd",
                        "m/dd",
                        "m/15",
                        "m/last",
                        "m/{month.length}",
                        "m/{monkey.google}",
                        "m/{month.length-1}",
                        "8/15",
                        "2/last",
                        "2/{month.length}",
                        "2/{monkey.google}",
                        "2/{month.length-1}",
                    };
                    foreach (var input in inputs)
                    {
                        Console.WriteLine(input);
                        try
                        {
                            Console.WriteLine("  " + Recurrence.Parse(input));
                        }
                        catch (Exception ex) 
                        {
                            Console.WriteLine("  !! " + ex.Message + " !!");
                        }
                    }
                }
            ),
            BuildMenuRoutine
            (
                "Simple Budget",
                () =>
                {
                    var tithing = new AutoDisbursement { Name = "Tithing", Category = ExpenseCategory.Tithing, Amount = "10%" };

                    var planningItems = new List<IBudgetPlanningItem>
                    {
                        Budget.CreateIncomePlanningItem("Paycheck", 3000m, "[name=startDate,value=20250224]2w", autoDisbursements: new[] { tithing }),  // payday every 2 weeks starting 2/24/2025
                        Budget.CreateIncomePlanningItem("Retirement Pension", 900m, "[name=startDate,value=20250101]last", autoDisbursements: new[] { tithing }),
                        Budget.CreateIncomePlanningItem("2nd to Last Day of Month Income", 1m, "{month.length-1}"),

                        Budget.CreateExpensePlanningItem("Bill Payment #1", 30m, "1"),  // $30 on the 1st of every month
                        Budget.CreateExpensePlanningItem("Bill Payment #2", 60m, "2"),  // $60 on the 2nd of every month
                        Budget.CreateExpensePlanningItem("Bill Payment #3", 90m, "3"),  // $90 on the 3rd of every month
                        Budget.CreateExpensePlanningItem("Bill Payment #4", 120m, "4"),  // $120 on the 4th of every month
                        Budget.CreateExpensePlanningItem("Bill Payment #15", 30m, "[name=startDate,value=20250115]m"),  // $30 on the 15th of every month
                        Budget.CreateExpensePlanningItem("Bill Payment #16", 60m, "[name=startDate,value=20250116]m"),  // $60 on the 16th of every month
                        Budget.CreateExpensePlanningItem("Bill Payment #17", 90m, "[name=startDate,value=20250117]m"),  // $90 on the 17th of every month
                        Budget.CreateExpensePlanningItem("Bill Payment #18", 120m, "[name=startDate,value=20250118]m"),  // $120 on the 18th of every month

                        Budget.CreateExpensePlanningItem("Mortgage", 1000m, "1"),
                        Budget.CreateExpensePlanningItem("Insurance", 200m, "4"),

                        Budget.CreateExpensePlanningItem("Allowance - Groceries ($500/mo)", 250m, "1,15"),  // $500 monthly grocery allowance split between the 1st and 15th
                        Budget.CreateExpensePlanningItem("Allowance - Gas ($400/mo)", 200m, "1,15"),        // $400 monthly gas allowance split between the 1st and 15th
                        Budget.CreateExpensePlanningItem("Allowance - Eating out ($150/mo)", 75m, "1,15"),  // $150 monthly eating out allowance split between the 1st and 15th
                        Budget.CreateExpensePlanningItem("Allowance - Amazon ($60/mo)", 30m, "1,15"),       // $60 monthly Amazon allowance split between the 1st and 15th
                    };
                    //var config = new BudgetConfig
                    //{
                    //    From = new DateTime(2025, 4, 1),
                    //    To = new DateTime(2025, 6, 1).AddDays(-1),
                    //    StartingAmount = 2000m,
                    //    PreSort = _budget => _budget.ItemSort = BudgetItemSort.DefaultIncomeFirst,
                    //    PreCalc = _budget => BudgetSpecialProcessing.CalculateTithingOnIncomeByBudgetItemName(_budget, new[] { "Paycheck", "VA Pension" })
                    //};

                    // generate the budget
                    var budgetList = Budget.GenerateFromPlanningItems
                    (
                        planningItems,
                        from: null, // defaults to current month
                        to: null,   // defaults to end of current month
                        itemOrdering: BudgetItemOrdering.IncomeFirst
                    );

                    // prepare to configure TextGrid
                    void configureGrid (TextGrid grid)
                    {
                        var dateCol = grid.GetColumnByTitle("Transaction Date");
                        if (dateCol != null)
                        {
                            dateCol.Title = "Date";
                            dateCol.Formatter = "MMM d";
                            grid.Columns.Remove(dateCol);
                            grid.Columns.Insert(0, dateCol);
                        }
                        grid.FormatDecimalColumnsAsCustomAccounting();
                    }

                    foreach (var budget in budgetList)
                    {
                        // output the budget in a configured TextGrid
                        Console.WriteLine();
                        Console.WriteLine($"{budget.Account.Name} budget from {budget.StartDate:MMM d, yyyy} to {budget.EndDate:MMM d, yyyy} starting with {budget.StartingAmount:C2}.");
                        Console.WriteLine();
                        Console.Write
                        (
                            TextGrid.FromCollection(budget, 
                                                    overrideType: typeof(BudgetItemBase), 
                                                    propertiesToExclude: new[] { "BudgetPlanningItem" },
                                                    excludeAdditionalPropertiesIf: p => p.PropertyType.FullName.StartsWith("System.Func`")
                            )       .Render(configureGrid: configureGrid)
                        );
                    }
                }
            )
        };

        //private static string GetShowingAdditionalOutputPhrase()
        //{
        //    if (Settings.Snowball.DisplayInterestAndPrincipalColumns.In(OptionalColumnDisplayPref.Always, OptionalColumnDisplayPref.IfGreaterThanZero))
        //        return ", additional output: (Pri, Int)";
        //    return "";
        //}

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
