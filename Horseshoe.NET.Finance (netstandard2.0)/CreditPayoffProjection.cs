using System;
using System.Collections.Generic;
using System.Linq;

using Horseshoe.NET.DateAndTime;
using Horseshoe.NET.Text.TextGrid;

namespace Horseshoe.NET.Finance
{
    public class CreditPayoffProjection : List<CreditAccountPayoffInfo>
    {
        public int NumberOfMonths => this.Max(sa => sa.Count);
        public decimal TotalInterest => this.SelectMany(sa => sa).Sum(mp => mp.InterestAmount);
        public decimal TotalRunningBalance => this.Sum(sa => sa.RunningBalance);
        public bool Snowballing { get; }
        public decimal MonthlyBudget { get; }
        public CreditAccountSorter Sorter { get; }

        /// <summary>
        /// This constructor is meant to be called only from <c>FinanceEngine.GenerateCreditPayoffProjection()</c>.
        /// </summary>
        /// <param name="creditAccounts">A list or array of <c>CreditAccount</c>s.</param>
        /// <param name="snowballing">If <c>true</c>, the calculated payments will snowball as account payoffs are projected.  Default is <c>false</c>.</param>
        /// <param name="extraSnowballAmount">An optional extra monthly amount to add to the payoff calculation.</param>
        /// <param name="sorter"></param>
        /// <exception cref="ValidationException"></exception>
        internal CreditPayoffProjection(IEnumerable<CreditAccount> creditAccounts, bool snowballing = false, decimal extraSnowballAmount = 0m, CreditAccountSorter sorter = null)
        {
            // validation
            if (creditAccounts == null || !creditAccounts.Any())
                throw new ValidationException("at least one account must be passed in");

            if (creditAccounts.Any(ca => ca.MinimumPaymentAmount <= 0m))
                throw new ValidationException("one or more accounts have a missing or invalid minimum payment amount");

            // build projection
            AddRange(creditAccounts.Select(ca => new CreditAccountPayoffInfo(ca)));
            Snowballing = snowballing;
            MonthlyBudget = creditAccounts.Sum(a => a.MinimumPaymentAmount) + (snowballing && extraSnowballAmount > 0m ? extraSnowballAmount : 0m);
            Sorter = sorter;
        }

        public TextGrid RenderToTextGrid(DateTime? startDate = null, string dateFormat = "MMM yyyy")
        {
            var dateColumn = new Column { Title = "Date", Format = dateFormat };
            var date = DateUtil.GetMonthStart(basedOnDateTime: startDate);
            var accountColumns = this
                .Select(sa => new Column(sa.Select(mp => mp.Render(sa))) { Title = sa.GetTitleElements() })
                .ToList();
            var textGrid = new TextGrid(accountColumns)
            {
                BorderPolicy = BorderPolicy.InnerVertical,  // these can be overridden in TextGrid.Render()
                PaddingPolicy = CellPaddingPolicy.Vertical | CellPaddingPolicy.ExceptOutermost
            };
            dateColumn.Insert(0, "");  // to match the subcolumn titles
            for (int i = 0, max = textGrid.MaxCount; i < max; i++)
            {
                dateColumn.Add(date.AddMonths(i));
            }
            for (int i = 0; i < Count; i++)
            {
                textGrid.Columns[i].Insert(0, this[i].RenderSubColumnTitles());
            }
            textGrid.InsertColumn(0, dateColumn);
            return textGrid;
        }
    }
}
