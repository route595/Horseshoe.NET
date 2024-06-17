using System;
using System.Collections.Generic;
using System.Linq;

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

        public CreditPayoffProjection(bool snowballing = false, decimal? monthlyBudget = null, CreditAccountSorter sorter = null)
        {
            Snowballing = snowballing;
            if (monthlyBudget.HasValue)
                MonthlyBudget = monthlyBudget.Value;
            if (sorter != null)
                Sorter = sorter;
        }

        public void LoadAccounts(IEnumerable<CreditAccount> accounts)
        {
            AddRange(accounts.Select(ca => new CreditAccountPayoffInfo(ca)));
        }

        public TextGrid RenderToTextGrid(DateTime? startDate = null, string dateFormat = "MMM yyyy")
        {
            var dateColumn = new Column { Title = "Date", Format = dateFormat };
            var date = startDate ?? new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
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
