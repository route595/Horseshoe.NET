using System.Collections.Generic;

namespace Horseshoe.NET.Text.TextGrid
{
    /// <summary>
    /// A specialized <c>TextGrid</c> column for storing and displaying nullable monetary values.
    /// </summary>
    public class NCurrencyColumn : Column<decimal?>
    {
        /// <summary>
        /// A default, basic currency format.
        /// </summary>
        public override string Format { get; set; } = "C";

        /// <summary>
        /// How to display null items.
        /// </summary>
        public override string DisplayNullAs { get; set; } = "$ -  ";

        /// <summary>
        /// The alignment of the title.
        /// </summary>
        public override HorizontalPosition TitleAlign { get; set; }

        /// <summary>
        /// The alignment of the data items.
        /// </summary>
        public override HorizontalPosition ItemAlign { get; set; } = HorizontalPosition.Right;

        /// <summary>
        /// Creates a new <c>NCurrencyColumn</c>.
        /// </summary>
        public NCurrencyColumn() : base()
        {
        }

        /// <summary>
        /// Creates a new <c>NCurrencyColumn</c>.
        /// </summary>
        /// <param name="capacity">Specifies the initial capacity.</param>
        public NCurrencyColumn(int capacity) : base (capacity)
        {
        }

        /// <summary>
        /// Creates a new <c>NCurrencyColumn</c>.
        /// </summary>
        /// <param name="collection">Elements to copy into this <c>Column</c>.</param>
        public NCurrencyColumn(IEnumerable<decimal?> collection) : base(collection)
        {
        }
    }
}
