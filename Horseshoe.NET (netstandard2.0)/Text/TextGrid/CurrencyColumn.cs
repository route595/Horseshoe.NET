using System.Collections.Generic;

namespace Horseshoe.NET.Text.TextGrid
{
    public class CurrencyColumn : Column<decimal>
    {
        public override string Format { get; set; } = "C";
        public override HorizontalAlign TitleAlign { get; set; }
        public override HorizontalAlign ItemAlign { get; set; } = HorizontalAlign.Right;

        public CurrencyColumn() : base()
        {
        }

        public CurrencyColumn(int capacity) : base (capacity)
        {
        }

        public CurrencyColumn(IEnumerable<decimal> collection) : base(collection)
        {
        }
    }
}
