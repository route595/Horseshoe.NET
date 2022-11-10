using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Horseshoe.NET.Objects;

namespace Horseshoe.NET.Text.TextGrid
{
    public class TextGrid
    {
        public IList<IColumn> Columns { get; }
        public int MaxCount => Columns.Any() ? Columns.Max(c => c.Count) : 0;
        public int? TargetWidth { get; set; }

        public BorderPolicy BorderPolicy { get; set; }
        private bool HasOuterBorder => (BorderPolicy & BorderPolicy.Outer) == BorderPolicy.Outer;
        private bool HasHorizontalInnerBorders => (BorderPolicy & BorderPolicy.InnerHorizontal) == BorderPolicy.InnerHorizontal;
        private bool HasVerticalInnerBorders => (BorderPolicy & BorderPolicy.InnerVertical) == BorderPolicy.InnerVertical;

        public int CellPaddingLeft { get; set; }
        public int CellPaddingRight { get; set; }
        public int CellPaddingTop { get; set; }
        public int CellPaddingBottom { get; set; }
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
        public int? CellPaddingLeftColumnLeft { get; set; }
        public int? CellPaddingRightColumnRight { get; set; }
        private int? AutomaticCellPaddingLeft { get; set; }
        private int? AutomaticCellPaddingLeftColumnLeft { get; set; }

        public int OuterPaddingLeft { get; set; }
        public int OuterPaddingRight { get; set; }
        public int OuterPaddingTop { get; set; }
        public int OuterPaddingBottom { get; set; }
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

        public int CombinedHorizontalOuterPadding =>
            OuterPaddingLeft +
            OuterPaddingRight;

        public int CombinedBorderWidths =>
            (HasOuterBorder ? 2 : 0) +                          // (1 per border) * 2 = 2
            (HasVerticalInnerBorders ? Columns.Count - 1 : 0);  // (1 per border) * (columns - 1)

        public int CombinedHorizontalCellPadding =>
            (CellPaddingLeftColumnLeft ?? CellPaddingLeft) +
            (CellPaddingRightColumnRight ?? CellPaddingRight) +
            CellPaddingLeft * (Columns.Count - 1) +
            CellPaddingRight * (Columns.Count - 1);

        public int CombinedRenderedTextWidth => 
            Columns.Sum(c => c.WidthToRender);

        public int TotalWidth =>
            CombinedBorderWidths +
            CombinedHorizontalCellPadding +
            CombinedRenderedTextWidth;

        public int TotalWidthIncludingOuterPadding =>
            CombinedHorizontalOuterPadding +
            CombinedBorderWidths +
            CombinedHorizontalCellPadding +
            CombinedRenderedTextWidth;

        public int TotalExtra =>
            CombinedHorizontalOuterPadding +
            CombinedBorderWidths +
            CombinedHorizontalCellPadding;

        public TextGrid()
        {
            Columns = new List<IColumn>();
        }

        public TextGrid(params IColumn[] columns)
        {
            Columns = columns;
        }

        public TextGrid(IEnumerable<IColumn> columns)
        {
            Columns = columns.ToList();
        }

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

        public static TextGrid FromCollection<T>(IEnumerable<T> collection, Action<TextGrid> formatGrid = null) where T : class, new()
        {
            var props = ObjectUtil.GetInstanceProperties<T>();
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
