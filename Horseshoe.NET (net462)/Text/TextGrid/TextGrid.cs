using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Horseshoe.NET.ObjectsAndTypes;

namespace Horseshoe.NET.Text.TextGrid
{
    /// <summary>
    /// A container, formatter and displayer of tabular data.
    /// </summary>
    public class TextGrid
    {
        /// <summary>
        /// The columns that comprise this grid.
        /// </summary>
        public IList<IColumn> Columns { get; }

        /// <summary>
        /// The longest of the column counts.
        /// </summary>
        public int MaxCount => Columns.Any() ? Columns.Max(c => c.Count) : 0;

        /// <summary>
        /// The desired width of the grid.
        /// </summary>
        public int? TargetWidth { get; set; }

        /// <summary>
        /// Whether to render borders and which borders to render on the <c>TextGrid</c>.
        /// </summary>
        public BorderPolicy BorderPolicy { get; set; }

        private bool HasOuterBorder => (BorderPolicy & BorderPolicy.Outer) == BorderPolicy.Outer;
        private bool HasHorizontalInnerBorders => (BorderPolicy & BorderPolicy.InnerHorizontal) == BorderPolicy.InnerHorizontal;
        private bool HasVerticalInnerBorders => (BorderPolicy & BorderPolicy.InnerVertical) == BorderPolicy.InnerVertical;

        /// <summary>
        /// Gets or sets the left cell padding.
        /// </summary>
        public int CellPaddingLeft { get; set; }

        /// <summary>
        /// Gets or sets the right cell padding.
        /// </summary>
        public int CellPaddingRight { get; set; }

        /// <summary>
        /// Gets or sets the top cell padding.
        /// </summary>
        public int CellPaddingTop { get; set; }

        /// <summary>
        /// Gets or sets the bottom cell padding.
        /// </summary>
        public int CellPaddingBottom { get; set; }

        /// <summary>
        /// Simultaneously sets the left, right, top and bottom cell padding.
        /// </summary>
        public int CellPadding
        {
            set
            {
                CellPaddingLeft =
                CellPaddingRight =
                CellPaddingTop =
                CellPaddingBottom = value;
            }
        }

        /// <summary>
        /// Gets or sets a distinct left cell padding on cells in the left column.
        /// </summary>
        public int? CellPaddingLeftColumnLeft { get; set; }

        /// <summary>
        /// Gets or sets a distinct right cell padding on cells in the right column.
        /// </summary>
        public int? CellPaddingRightColumnRight { get; set; }
        private int? AutomaticCellPaddingLeft { get; set; }
        private int? AutomaticCellPaddingLeftColumnLeft { get; set; }

        /// <summary>
        /// Gets or sets the left outer grid padding.
        /// </summary>
        public int OuterPaddingLeft { get; set; }

        /// <summary>
        /// Gets or sets the right outer grid padding.
        /// </summary>
        public int OuterPaddingRight { get; set; }

        /// <summary>
        /// Gets or sets the top outer grid padding.
        /// </summary>
        public int OuterPaddingTop { get; set; }

        /// <summary>
        /// Gets or sets the bottom outer grid padding.
        /// </summary>
        public int OuterPaddingBottom { get; set; }

        /// <summary>
        /// Simultaneously sets the left, right, top and bottom outer grid padding.
        /// </summary>
        public int OuterPadding
        {
            set
            {
                OuterPaddingLeft =
                OuterPaddingRight =
                OuterPaddingTop =
                OuterPaddingBottom = value;
            }
        }

        /// <summary>
        /// Gets the combined horizontal outer grid padding.
        /// </summary>
        public int CombinedHorizontalOuterPadding =>
            OuterPaddingLeft +
            OuterPaddingRight;

        /// <summary>
        /// Gets the combined vertical border widths.
        /// </summary>
        public int CombinedBorderWidths =>
            (HasOuterBorder ? 2 : 0) +                          // (1 per border) * 2 = 2
            (HasVerticalInnerBorders ? Columns.Count - 1 : 0);  // (1 per border) * (columns - 1)

        /// <summary>
        /// Gets the combined horizontal cell padding
        /// </summary>
        public int CombinedHorizontalCellPadding =>
            (CellPaddingLeftColumnLeft ?? CellPaddingLeft) +
            (CellPaddingRightColumnRight ?? CellPaddingRight) +
            CellPaddingLeft * (Columns.Count - 1) +
            CellPaddingRight * (Columns.Count - 1);

        /// <summary>
        /// Gets the combined final width of the rendered columns.
        /// </summary>
        public int CombinedRenderedTextWidth => 
            Columns.Sum(c => c.WidthToRender);

        /// <summary>
        /// Gets the total width of the grid.
        /// </summary>
        public int TotalWidth =>
            CombinedBorderWidths +
            CombinedHorizontalCellPadding +
            CombinedRenderedTextWidth;

        /// <summary>
        /// Gets the total width of the grid plus padding.
        /// </summary>
        public int TotalWidthIncludingOuterPadding =>
            CombinedHorizontalOuterPadding +
            CombinedBorderWidths +
            CombinedHorizontalCellPadding +
            CombinedRenderedTextWidth;

        /// <summary>
        /// Gets the total extra width from borders and padding.
        /// </summary>
        public int TotalExtra =>
            CombinedHorizontalOuterPadding +
            CombinedBorderWidths +
            CombinedHorizontalCellPadding;

        /// <summary>
        /// Creates a new <c>TextGrid</c>.
        /// </summary>
        public TextGrid()
        {
            Columns = new List<IColumn>();
        }

        /// <summary>
        /// Creates a new <c>TextGrid</c>.
        /// </summary>
        /// <param name="columns">A collection of <c>IColumns</c> from which to build the grid.</param>
        public TextGrid(params IColumn[] columns)
        {
            Columns = columns;
        }

        /// <summary>
        /// Creates a new <c>TextGrid</c>.
        /// </summary>
        /// <param name="columns">A collection of <c>IColumns</c> from which to build the grid.</param>
        public TextGrid(IEnumerable<IColumn> columns)
        {
            Columns = columns.ToList();
        }

        /// <summary>
        /// Creates a new <c>TextGrid</c>.
        /// </summary>
        /// <param name="items">A collection of items.</param>
        /// <param name="columns">The number of desired columns.</param>
        /// <param name="targetWidth">The desired width of the <c>TextGrid</c>.</param>
        /// <exception cref="ValidationException"></exception>
        public TextGrid(IEnumerable<string> items, int columns = 1, int? targetWidth = null) : this()
        {
            TargetWidth = targetWidth; 

            // validation
            if (items == null || !items.Any())
            {
                items = new[] { "[empty grid]" };
            }
            if (columns < 1)
            {
                throw new ValidationException("TextGrid must have at least one column");
            }

            if (columns == 1)
            {
                Columns.Add(new Column<string>(items));
            }
            else
            {
                // divvy out grid items into multiple columns, shortest column last
                // (e.g. 100i @ 3c => 34i, 34i, 32i)
                var runningCount = 0;
                var blockSize = (int)Math.Ceiling(items.Count() / (double)columns);
                for (int i = 0; i < columns; i++)
                {
                    if (i < columns - 1)    // all columns except last
                    {
                        Columns.Add(new Column<string>(items.Skip(runningCount).Take(blockSize)));
                        runningCount += blockSize;
                    }
                    else                    // last column
                    {
                        Columns.Add(new Column<string>(items.Skip(runningCount)));
                    }
                }
            }
        }

        /// <summary>
        /// Renders this text grid.
        /// </summary>
        /// <returns>A <c>string</c> representation of this text grid for displaying to a console or other text based output</returns>
        /// <exception cref="ValidationException"></exception>
        public string Render()
        {
            if (TargetWidth.HasValue)
            {
                if (TargetWidth < 1)
                {
                    throw new ValidationException("TextGrid must be at least one character wide");
                }

                // resize columns, e.g. 100w @ 3c => 33w, 33w, 34w
                var targetCombinedCellWidth = TargetWidth.Value - TotalExtra;
                if (targetCombinedCellWidth < 1)
                {
                    throw new ValidationException("TextGrid must be at least one character wide before borders and padding are added");
                }
                var cellWidth = (int)Math.Floor(targetCombinedCellWidth / (double)Columns.Count);
                var runningCombinedCellWidth = 0;

                for (int i = 0; i < Columns.Count; i++)
                {
                    if (i < Columns.Count - 1)  // all columns except last
                    {
                        Columns[i].TargetWidth = cellWidth;
                        runningCombinedCellWidth += cellWidth;
                    }
                    else                        // last column
                    {
                        Columns[i].TargetWidth = targetCombinedCellWidth - runningCombinedCellWidth;
                    }
                }
            }
            else
            {
                foreach (var col in Columns)
                {
                    col.TargetWidth = null;
                }
            }

            // prepare some automatic formatting, if applicable
            AutomaticCellPaddingLeft = null;
            AutomaticCellPaddingLeftColumnLeft = null;
            if ((BorderPolicy == BorderPolicy.None) && CellPaddingLeft == 0)
            {
                AutomaticCellPaddingLeft = 1;                // automatically add 1 padding between columns of unformatted grids
                if (!CellPaddingLeftColumnLeft.HasValue)
                {
                    AutomaticCellPaddingLeftColumnLeft = 0;  // prevent automatic padding in the first column of unformatted grids
                }
            }

            // prerender the columns
            foreach (var col in Columns)
            {
                col.Prerender();
            }

            // render the grid to a string
            var sbld = new StringBuilder();
            for(int i = 0; i < OuterPaddingTop; i++)
            {
                RenderOuterPadding(sbld);
            }
            if (HasOuterBorder)
            {
                RenderTopHorizontalOuterBorder(sbld);
            }
            if (Columns.Any(c => c.Title != null))
            {
                RenderColumnTitleCells(sbld);
                if (HasHorizontalInnerBorders)
                {
                    RenderHorizontalInnerBorder(sbld);
                }
            }
            if (MaxCount > 0)
            {
                for (int i = 0, max = MaxCount; i < max; i++)
                {
                    if (i > 0 && HasHorizontalInnerBorders)
                    {
                        RenderHorizontalInnerBorder(sbld);
                    }
                    RenderDataCells(sbld, i);
                }
            }
            else
            {
                sbld.AppendLine("[empty grid]");
            }
            if (HasOuterBorder)
            {
                RenderBottomHorizontalOuterBorder(sbld);
            }
            for (int i = 0; i < OuterPaddingBottom; i++)
            {
                RenderOuterPadding(sbld);
            }
            return sbld.ToString();
        }

        private void RenderOuterPadding(StringBuilder sbld)
        {
            sbld.AppendLine("".PadLeft(OuterPaddingLeft + TotalWidth + OuterPaddingRight));
        }

        private void RenderTopHorizontalOuterBorder(StringBuilder sbld)
        {
            sbld.Append("".PadLeft(OuterPaddingLeft))
                .Append("╔")
                .Append("".PadLeft(TotalWidth - 2, '═'))
                .Append("╗")
                .AppendLine("".PadLeft(OuterPaddingRight));
        }

        private void RenderBottomHorizontalOuterBorder(StringBuilder sbld)
        {
            sbld.Append("".PadLeft(OuterPaddingLeft))
                .Append("╚")
                .Append("".PadLeft(TotalWidth - 2, '═'))
                .Append("╝")
                .AppendLine("".PadLeft(OuterPaddingRight));
        }

        private void RenderHorizontalInnerBorder(StringBuilder sbld)
        {
            sbld.Append("".PadLeft(OuterPaddingLeft))
                .AppendIf(HasOuterBorder, "╟")
                .Append("".PadLeft(TotalWidth - (HasOuterBorder ? 2 : 0), '─'))
                .AppendIf(HasOuterBorder, "╢")
                .AppendLine("".PadLeft(OuterPaddingRight));
        }

        private void RenderLine(StringBuilder sbld, string renderedCells)
        {
            sbld.Append("".PadLeft(OuterPaddingLeft))
                .AppendIf(HasOuterBorder, "║")
                .Append(renderedCells)
                .AppendIf(HasOuterBorder, "║")
                .AppendLine("".PadLeft(OuterPaddingRight));
        }

        private void RenderColumnTitleCells(StringBuilder sbld)
        {
            var cells = new List<string[]>();
            for (int i = 0; i < Columns.Count; i++)
            {
                var leftPadding = i == 0
                    ? AutomaticCellPaddingLeftColumnLeft ?? CellPaddingLeftColumnLeft ?? CellPaddingLeft
                    : AutomaticCellPaddingLeft ?? CellPaddingLeft;
                var rightPadding = i == Columns.Count - 1
                    ? CellPaddingRightColumnRight ?? CellPaddingRight
                    : CellPaddingRight;
                cells.Add(Columns[i].RenderTitleCell(leftPadding, rightPadding, CellPaddingTop, CellPaddingBottom));
            }
            string separator = HasVerticalInnerBorders ? "│" : "";
            for (int i = 0; i < cells.First().Length; i++)
            {
                RenderLine(sbld, string.Join(separator, cells.Select(c => c[i])));
            }
        }

        private void RenderDataCells(StringBuilder sbld, int index)
        {
            var cells = new List<string[]>();
            for(int i = 0; i < Columns.Count; i++)
            {
                var leftPadding = i == 0 
                    ? AutomaticCellPaddingLeftColumnLeft ?? CellPaddingLeftColumnLeft ?? CellPaddingLeft 
                    : AutomaticCellPaddingLeft ?? CellPaddingLeft;
                var rightPadding = i == Columns.Count - 1 
                    ? CellPaddingRightColumnRight ?? CellPaddingRight 
                    : CellPaddingRight;
                cells.Add(Columns[i].RenderCell(index, leftPadding, rightPadding, CellPaddingTop, CellPaddingBottom));
            }
            string separator = HasVerticalInnerBorders ? "│" : "";
            for (int i = 0; i < cells.First().Length; i++)
            {
                RenderLine(sbld, string.Join(separator, cells.Select(c => c[i])));
            }
        }

        /// <summary>
        /// Builds a <c>TextGrid</c> from a collection of items.
        /// </summary>
        /// <typeparam name="T">The type of item to store and display in this <c>TextGrid</c>.</typeparam>
        /// <param name="collection">A collection of items.</param>
        /// <param name="formatGrid"></param>
        /// <returns></returns>
        public static TextGrid FromCollection<T>(IEnumerable<T> collection, Action<TextGrid> formatGrid = null) where T : class, new()
        {
            var props = TypeUtil.GetInstanceProperties<T>();
            var cols = props
                .Select(p => new TypedColumn(p.PropertyType) { Title = TextUtil.SpaceOutTitleCase(p.Name) })
                .ToList();
            foreach (var t in collection ?? Enumerable.Empty<T>())
            {
                for (int i = 0; i < cols.Count; i++)
                {
                    cols[i].Add(props[i].GetValue(t));
                }
            }
            //var grid = new TextGrid { BorderPolicy = borderPolicy, CellPaddingLeft = cellPadding, CellPaddingRight = 1 };
            var grid = new TextGrid(cols);
            //cols.ForEach(c => grid.Columns.Add(c));
            formatGrid?.Invoke(grid);
            return grid;
        }

        /// <summary>
        /// Builds a <c>TextGrid</c> from an <c>IDictionary</c>.
        /// </summary>
        /// <typeparam name="TKey">The type of key.</typeparam>
        /// <typeparam name="TValue">The type of value.</typeparam>
        /// <param name="dictionary">An <c>IDictionary</c>.</param>
        /// <returns></returns>
        public static TextGrid FromDictionary<TKey, TValue>(IDictionary<TKey, TValue> dictionary)
        {
            var col1 = new Column<TKey>();
            var col2 = new Column<TValue>();
            foreach (var kvp in dictionary)
            {
                col1.Add(kvp.Key);
                col2.Add(kvp.Value);
            }
            return new TextGrid(col1, col2);
        }
    }
}
