using System.Collections.Generic;

namespace Horseshoe.NET.Text.TextGrid
{
    public class NCurrencyColumn : Column<decimal?>
    {
        public override string Format { get; set; } = "C";
        public override string DisplayNullAs { get; set; } = "$ -  ";
        public override HorizontalAlign TitleAlign { get; set; }
        public override HorizontalAlign ItemAlign { get; set; } = HorizontalAlign.Right;

        public NCurrencyColumn() : base()
        {
        }

        public NCurrencyColumn(int capacity) : base (capacity)
        {
        }

        public NCurrencyColumn(IEnumerable<decimal?> collection) : base(collection)
        {
        }
    }
}
