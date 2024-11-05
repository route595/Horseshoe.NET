using System;
using System.Collections.Generic;
using System.Linq;

using Horseshoe.NET.DateAndTime;
using Horseshoe.NET.Text.TextGrid;

namespace Horseshoe.NET.Finance
{
    public class DebtPayoffProjection : List<CreditAccountPayoffInfo>
    {
        public int NumberOfMonths => this.Max(sa => sa.Count);
        public decimal TotalInterest => this.SelectMany(sa => sa).Sum(mp => mp.InterestAmount);
        public decimal TotalRunningBalance => this.Sum(sa => sa.RunningBalance);
        public DateTime StartDate { get; }
        public bool Snowballing { get; }
        public decimal MinimumMonthlyBudget { get; }
        public decimal ExtraSnowballAmount { get; }
        public decimal TotalMonthlyBudget => MinimumMonthlyBudget + (Snowballing ? ExtraSnowballAmount : 0m);
        public CreditAccountSortOrder SortOrder { get; }
        internal IList<DateTime> Dates { get; }

        /// <summary>
        /// This constructor is meant to be called only from <c>FinanceEngine.GenerateCreditPayoffProjection()</c>.
        /// </summary>
        /// <param name="creditAccounts">A list or array of <c>CreditAccount</c>s.</param>
        /// <param name="startDate">An optional <c>DateTime</c> at which to start the debt payoff calculation.</param>
        /// <param name="snowballing">If <c>true</c>, the calculated payments will snowball as account payoffs are projected.  Default is <c>false</c>.</param>
        /// <param name="extraSnowballAmount">An optional extra monthly amount to add to the payoff calculation.</param>
        /// <param name="sortOrder">How the account list was sorted (see <c>FinanceEngine.GenerateCreditPayoffProjection()</c>)</param>
        /// <exception cref="ValidationException"></exception>
        internal DebtPayoffProjection(IEnumerable<CreditAccount> creditAccounts, DateTime? startDate = null, bool snowballing = false, decimal extraSnowballAmount = 0m, CreditAccountSortOrder sortOrder = CreditAccountSortOrder.NotSorted)
        {
            // validation
            if (creditAccounts == null || !creditAccounts.Any())
                throw new ValidationException("at least one account must be passed in");

            if (creditAccounts.Any(ca => ca.MinimumPaymentAmount <= 0m))
                throw new ValidationException("one or more accounts have a missing or invalid minimum payment amount");

            if (snowballing && extraSnowballAmount < 0m)
                throw new ValidationException("extra snowball amount must be greater than zero: " + extraSnowballAmount);

            // build projection
            AddRange(creditAccounts.Select(ca => new CreditAccountPayoffInfo(ca)));
            StartDate = DateUtil.GetMonthStart(startDate);
            Snowballing = snowballing;
            MinimumMonthlyBudget = creditAccounts.Sum(a => a.MinimumPaymentAmount);
            ExtraSnowballAmount = extraSnowballAmount;
            SortOrder = sortOrder;
            Dates = new List<DateTime>();
        }

        public TextGrid RenderToTextGrid(DateTime? startDate = null, string dateFormat = "MMM yyyy")
        {
            var dateColumn = new Column { Title = "Date", Format = dateFormat };
            //var date = DateUtil.GetMonthStart(basedOnDateTime: startDate);
            var accountColumns = this
                .Select(sa => new Column(sa.Select(mp => mp.Render(sa))) { Title = sa.GetTitleElements() })
                .ToList();
            var textGrid = new TextGrid(accountColumns)
            {
                BorderPolicy = BorderPolicy.InnerVertical,  // these can be overridden in TextGrid.Render()
                PaddingPolicy = CellPaddingPolicy.Vertical | CellPaddingPolicy.ExceptOutermost
            };

            // insert sub-column titles in calculated columns
            for (int i = 0; i < Count; i++)
            {
                textGrid.Columns[i].Insert(0, this[i].RenderSubColumnTitles());
            }

            // insert and populate date column
            textGrid.InsertColumn(0, dateColumn);
            dateColumn.Add("");  // sub-column title is blank
            foreach (var date in Dates)
            {
                dateColumn.Add(date);
            }

            return textGrid;
        }
    }
}
