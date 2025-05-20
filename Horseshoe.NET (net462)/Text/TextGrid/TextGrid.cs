using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

using Horseshoe.NET.Comparison;
using Horseshoe.NET.Format;
using Horseshoe.NET.ObjectsTypesAndValues;

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
        public List<Column> Columns { get; } = new List<Column>();

        /// <summary>
        /// The columns that will be rendered.
        /// </summary>
        public List<Column> VisibleColumns => Columns.Where(c => !c.Hidden).ToList();

        /// <summary>
        /// The longest of the column counts.
        /// </summary>
        public int MaxVisibleCount => VisibleColumns.Any() ? VisibleColumns.Max(c => c.Count) : 0;

        /// <summary>
        /// The desired width of the grid.
        /// </summary>
        public int? TargetWidth { get; set; }

        public int TitleHeight => VisibleColumns.Max(c => c.Title.Count);

        /// <summary>
        /// Which borders to render on the <c>TextGrid</c>.
        /// </summary>
        public BorderPolicy BorderPolicy { get; set; }

        public bool HasOuterTopBorder => (BorderPolicy & BorderPolicy.OuterTop) == BorderPolicy.OuterTop;
        public bool HasOuterBottomBorder => (BorderPolicy & BorderPolicy.OuterBottom) == BorderPolicy.OuterBottom;
        public bool HasOuterLeftBorder => (BorderPolicy & BorderPolicy.OuterLeft) == BorderPolicy.OuterLeft;
        public bool HasOuterRightBorder => (BorderPolicy & BorderPolicy.OuterRight) == BorderPolicy.OuterRight;
        public bool HasOuterBorder => HasOuterTopBorder && HasOuterBottomBorder && HasOuterLeftBorder && HasOuterRightBorder;
        public bool HasHorizontalInnerBorders => (BorderPolicy & BorderPolicy.InnerHorizontal) == BorderPolicy.InnerHorizontal;
        public bool HasVerticalInnerBorders => (BorderPolicy & BorderPolicy.InnerVertical) == BorderPolicy.InnerVertical;

        /// <summary>
        /// Which cell borders to pad in the <c>TextGrid</c>.
        /// </summary>
        public CellPaddingPolicy PaddingPolicy { get; set; }

        public bool IsPaddingCellTop => (PaddingPolicy & CellPaddingPolicy.Top) == CellPaddingPolicy.Top;
        public bool IsPaddingCellBottom => (PaddingPolicy & CellPaddingPolicy.Bottom) == CellPaddingPolicy.Bottom;
        public bool IsPaddingCellLeft => (PaddingPolicy & CellPaddingPolicy.Left) == CellPaddingPolicy.Left;
        public bool IsPaddingCellLeftExceptLeftmost => (PaddingPolicy & CellPaddingPolicy.ExceptLeftmost) == CellPaddingPolicy.ExceptLeftmost;
        public bool IsPaddingCellRight => (PaddingPolicy & CellPaddingPolicy.Right) == CellPaddingPolicy.Right;
        public bool IsPaddingCellRightExceptRightmost => (PaddingPolicy & CellPaddingPolicy.ExceptRightmost) == CellPaddingPolicy.ExceptRightmost;

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
            (HasOuterLeftBorder ? 1 : 0) +
            (HasOuterRightBorder ? 1 : 0) +
            (HasVerticalInnerBorders ? VisibleColumns.Count - 1 : 0);  // (1 per border) * (columns - 1)

        /// <summary>
        /// Gets the combined horizontal cell padding
        /// </summary>
        public int CombinedHorizontalCellPadding =>
            // |^cell 1 |^cell 2 |^cell 3 |
            (IsPaddingCellLeft ? VisibleColumns.Count - (IsPaddingCellLeftExceptLeftmost ? 1 : 0) : 0) +
            // | cell 1^| cell 2^| cell 3^|
            (IsPaddingCellRight ? VisibleColumns.Count - (IsPaddingCellRightExceptRightmost ? 1 : 0) : 0);

        /// <summary>
        /// Gets the combined final width of the rendered columns.
        /// </summary>
        public int CombinedRenderedTextWidth =>
            VisibleColumns.Sum(c => c.WidthToRender);

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
        }

        /// <summary>
        /// Creates a new <c>TextGrid</c>.
        /// </summary>
        /// <param name="columns">A collection of <c>Column</c> from which to build the grid.</param>
        public TextGrid(IEnumerable<Column> columns)
        {
            AddColumns(columns);
        }

        /// <summary>
        /// Creates a new <c>TextGrid</c>.
        /// </summary>
        /// <param name="columns">A collection of <c>Column</c> from which to build the grid.</param>
        public TextGrid(params Column[] columns) : this(columns as IEnumerable<Column>)
        {
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
                AddColumn(new Column(items));
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
                        AddColumn(new Column(items.Skip(runningCount).Take(blockSize)));
                        runningCount += blockSize;
                    }
                    else                    // last column
                    {
                        AddColumn(new Column(items.Skip(runningCount)));
                    }
                }
            }
        }

        /// <summary>
        /// Generates a new column and adds it to the grid.
        /// </summary>
        /// <param name="title">Column title, optional.</param>
        public Column AddColumn(string title = null)
        {
            var column = new Column { Title = title };
            AddColumns(column);
            return column;
        }

        /// <summary>
        /// Generates a new column and adds it to the grid.
        /// </summary>
        /// <param name="capacity">Specifies the initial capacity.</param>
        /// <param name="title">Column title, optional.</param>
        public Column AddColumn(int capacity, string title = null)
        {
            var column = new Column(capacity) { Title = title };
            AddColumns(column);
            return column;
        }

        /// <summary>
        /// Generates a new column and adds it to the grid.
        /// </summary>
        /// <param name="collection">Elements to copy into this <c>Column</c>.</param>
        /// <param name="title">Column title, optional.</param>
        public Column AddColumn(IEnumerable<object> collection, string title = null)
        {
            var column = new Column(collection) { Title = title };
            AddColumns(column);
            return column;
        }
         
        /// <summary>
        /// The preferred method for adding a column to the grid (as opposed to adding it to the <c>Columns</c> collection)
        /// </summary>
        /// <param name="column">A column to add to the grid</param>
        public void AddColumn(Column column)
        {
            column.Parent = this;
            Columns.Add(column);
        }

        /// <summary>
        /// The preferred method for adding columns to the grid (as opposed to adding them to the column collection)
        /// </summary>
        /// <param name="columns">Columns to add to the grid</param>
        public void AddColumns(IEnumerable<Column> columns)
        {
            if (columns == null)
                return;
            foreach (var column in columns)
                AddColumn(column);
        }

        /// <summary>
        /// The preferred method for adding columns to the grid (as opposed to adding them to the column collection)
        /// </summary>
        /// <param name="columns">Columns to add to the grid</param>
        public void AddColumns(params Column[] columns)
        {
            if (columns == null)
                return;
            foreach (var column in columns)
                AddColumn(column);
        }

        /// <summary>
        /// Inserts a column at the specified index
        /// </summary>
        /// <param name="index">The position in the column collection at which to insert the column</param>
        /// <param name="column">A column</param>
        public void InsertColumn(int index, Column column)
        {
            column.Parent = this;
            Columns.Insert(index, column);
        }

        /// <summary>
        /// Quickly formats all columns of a specified type with the specified format.
        /// </summary>
        /// <param name="type">A column runtime type</param>
        /// <param name="format">A format</param>
        /// <param name="customPostRenderer">A user-supplied post-renderer for adding custom formatting to the output string prior to padding.</param>
        public void FormatColumnsByType(Type type, string format, Func<string, object, string> customPostRenderer = null)
        {
            foreach (var col in Columns)
            {
                if (col.DataType == type)
                    col.Formatter = new Formatter(format, customPostRenderer: customPostRenderer);
            }
        }

        /// <summary>
        /// Quickly formats all columns of a specified type with the specified format.
        /// </summary>
        /// <param name="type">A column runtime type</param>
        /// <param name="customFormatter">A user-supplied custom formatter</param>
        public void FormatColumnsByType(Type type, Func<object,string> customFormatter)
        {
            foreach (var col in Columns)
            {
                if (col.DataType == type)
                    col.Formatter = customFormatter;
            }
        }

        /// <summary>
        /// Quickly formats all columns having the specified text in the title with the specified format.
        /// </summary>
        /// <param name="titleOrPartial">A column title or partial title ot search</param>
        /// <param name="format">A format</param>
        /// <param name="ignoreCase">If <c>true</c>, performs a case insensitive search</param>
        /// <param name="customPostRenderer">A user-supplied post-renderer for adding custom formatting to the output string prior to padding.</param>
        public void FormatColumnsByTitle(string titleOrPartial, string format, bool ignoreCase = false, Func<string, object, string> customPostRenderer = null)
        {
            var criterinator = ignoreCase
                ? Criterinator.ContainsIgnoreCase(titleOrPartial)
                : Criterinator.Contains(titleOrPartial);
            var colsToFormat = Columns.Where(c => criterinator.IsMatch(c.Title));
            foreach (var col in colsToFormat)
                col.Formatter = new Formatter(format, customPostRenderer: customPostRenderer);
        }

        /// <summary>
        /// <param name="ignoreCase">If <c>true</c>, performs a case insensitive search</param>
        /// </summary>
        /// <param name="titleOrPartial">A column title or partial title ot search</param>
        /// <param name="formatter">A formatter</param>
        public void FormatColumnsByTitle(string titleOrPartial, Func<object, string> formatter, bool ignoreCase = false)
        {
            var criterinator = ignoreCase
                ? Criterinator.ContainsIgnoreCase(titleOrPartial)
                : Criterinator.Contains(titleOrPartial);
            var colsToFormat = Columns.Where(c => criterinator.IsMatch(c.Title));
            foreach (var col in colsToFormat)
                col.Formatter = formatter;
        }

        /// <summary>
        /// Gets a column based on the title which may have been inferred from a class property name.
        /// </summary>
        /// <param name="titleOrPropertyName">The column title or the class property name from which it was inferred</param>
        /// <param name="ignoreCase">Whether to ignore the title case of the column title when comparing to the argument</param>
        /// <returns>The matching column</returns>
        public Column GetColumnByTitle(string titleOrPropertyName, bool ignoreCase = false)
        {
            if (Columns == null)
                return null;
            var col = Columns.SingleOrDefault(c => ignoreCase ? string.Equals(c.Title, titleOrPropertyName, StringComparison.OrdinalIgnoreCase) : c.Title == titleOrPropertyName);
            if (col == null && !titleOrPropertyName.Any(c => c == ' '))
                col = Columns.SingleOrDefault(c => c.Title == TextUtil.SpaceOutTitleCase(titleOrPropertyName));
            return col;
        }

        /// <summary>
        /// Renders this text grid.
        /// </summary>
        /// <param name="targetWidth">The final width to attempt to achieve.</param>
        /// <param name="configureGrid">An optional function to pass in which configures the generated <c>TextGrid</c>.</param>
        /// <returns>A <c>string</c> representation of this text grid for displaying to a console or other text based output</returns>
        /// <exception cref="ValidationException"></exception>
        public string Render(int? targetWidth = null, Action<TextGrid> configureGrid = null)
        {
            if (targetWidth.HasValue)
                TargetWidth = targetWidth.Value;
            configureGrid?.Invoke(this);

            var visibleColumns = VisibleColumns;  // local cache for performance

            if (TargetWidth.HasValue)
            {
                if (TargetWidth < 0)
                    throw new ValidationException("TextGrid width may not be a negative number");

                // resize columns, e.g. 100w @ 3c => 33w, 33w, 34w
                var targetCombinedCellWidth = TargetWidth.Value - TotalExtra;
                if (targetCombinedCellWidth < 1)
                {
                    throw new ValidationException("TextGrid must be at least one character wide before borders and padding are added");
                }
                var cellWidth = (int)Math.Floor(targetCombinedCellWidth / (double)visibleColumns.Count);
                var runningCombinedCellWidth = 0;

                for (int i = 0, visibleCount = visibleColumns.Count; i < visibleCount; i++)
                {
                    if (i < visibleCount - 1)  // all columns except last
                    {
                        visibleColumns[i].TargetWidth = cellWidth;
                        runningCombinedCellWidth += cellWidth;
                    }
                    else                        // last column
                    {
                        visibleColumns[i].TargetWidth = targetCombinedCellWidth - runningCombinedCellWidth;
                    }
                }
            }
            else
            {
                foreach (var col in visibleColumns)
                {
                    col.TargetWidth = null;
                }
            }

            //// prepare some automatic formatting, if applicable
            //AutomaticCellPaddingLeft = null;
            //AutomaticCellPaddingLeftColumnLeft = null;
            //if ((BorderPolicy == BorderPolicy.None) && !IsPaddingAnyBorders)
            //{
            //    AutomaticCellPaddingLeft = 1;                // automatically add 1 padding between columns of unformatted grids
            //    AutomaticCellPaddingLeftColumnLeft = 0;      // prevent automatic padding in the first column of unformatted grids
            //}

            // prerender the columns
            foreach (var col in visibleColumns)
            {
                col.Prerender();
            }

            // render the grid to a string
            var sbld = new StringBuilder();
            for (int i = 0; i < OuterPaddingTop; i++)
            {
                RenderOuterPadding(sbld);
            }
            if (HasOuterTopBorder)
            {
                RenderTopHorizontalOuterBorder(sbld);
            }
            if (visibleColumns.Any(c => c.Title.Any()))
            {
                RenderColumnTitleCells(sbld);
                if (HasHorizontalInnerBorders)
                {
                    RenderHorizontalInnerBorder(sbld);
                }
            }
            if (MaxVisibleCount > 0)
            {
                for (int i = 0, max = MaxVisibleCount; i < max; i++)
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
            if (HasOuterBottomBorder)
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
                .AppendIf(HasOuterLeftBorder, "╟")
                .Append("".PadLeft(TotalWidth - (HasOuterLeftBorder ? 1 : 0) - (HasOuterRightBorder ? 1 : 0), '─'))
                .AppendIf(HasOuterRightBorder, "╢")
                .AppendLine("".PadLeft(OuterPaddingRight));
        }

        private void RenderLine(StringBuilder sbld, string renderedCells)
        {
            sbld.Append("".PadLeft(OuterPaddingLeft))
                .AppendIf(HasOuterLeftBorder, "║")
                .Append(renderedCells)
                .AppendIf(HasOuterRightBorder, "║")
                .AppendLine("".PadLeft(OuterPaddingRight));
        }

        private void RenderColumnTitleCells(StringBuilder sbld)
        {
            var cells = new List<string[]>();
            var visibleColumns = VisibleColumns;  // local cache for performance
            for (int i = 0; i < visibleColumns.Count; i++)
            {
                var leftPadding = i == 0
                    ? (IsPaddingCellLeft && !IsPaddingCellLeftExceptLeftmost ? 1 : 0)
                    : (IsPaddingCellLeft || (BorderPolicy == BorderPolicy.None && PaddingPolicy == CellPaddingPolicy.None) ? 1 : 0);
                var rightPadding = i == visibleColumns.Count - 1
                    ? (IsPaddingCellRight && !IsPaddingCellRightExceptRightmost ? 1 : 0)
                    : (IsPaddingCellRight ? 1 : 0);
                cells.Add(visibleColumns[i].RenderTitleCell(leftPadding, rightPadding, IsPaddingCellTop ? 1 : 0, IsPaddingCellBottom ? 1 : 0));
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
            for (int i = 0; i < Columns.Count; i++)
            {
                var leftPadding = i == 0
                    ? (IsPaddingCellLeft && !IsPaddingCellLeftExceptLeftmost ? 1 : 0)
                    : (IsPaddingCellLeft || (BorderPolicy == BorderPolicy.None && PaddingPolicy == CellPaddingPolicy.None) ? 1 : 0);
                var rightPadding = i == Columns.Count - 1
                    ? (IsPaddingCellRight && !IsPaddingCellRightExceptRightmost ? 1 : 0)
                    : (IsPaddingCellRight ? 1 : 0);
                cells.Add(Columns[i].RenderCell(index, leftPadding, rightPadding, IsPaddingCellTop ? 1 : 0, IsPaddingCellBottom ? 1 : 0));
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
        /// <param name="configureGrid">An optional function to pass in which configures the generated <c>TextGrid</c>.</param>
        /// <returns></returns>
        public static TextGrid FromCollection<T>(IEnumerable<T> collection, Action<TextGrid> configureGrid = null) where T : class, new()
        {
            var props = TypeUtil.GetInstanceProperties<T>();
            var cols = props
                .Select(InferColumn)
                .ToList();
            foreach (var t in collection ?? Enumerable.Empty<T>())
            {
                for (int i = 0; i < cols.Count; i++)
                {
                    cols[i].List.Add(props[i].GetValue(t));
                }
            }
            //var grid = new TextGrid { BorderPolicy = borderPolicy, CellPaddingLeft = cellPadding, CellPaddingRight = 1 };
            var grid = new TextGrid(cols);
            //cols.ForEach(c => grid.Columns.Add(c));
            configureGrid?.Invoke(grid);
            return grid;
        }

        private static Column InferColumn(PropertyInfo prop)
        {
            var col = new Column
            {
                Title = TextUtil.SpaceOutTitleCase(prop.Name),
                DataType = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType
            };
            if (col.DataType.IsNumeric() || col.DataType == typeof(DateTime))
            {
                col.TitleAlign = HorizontalAlign.Right;
                col.ItemAlign = HorizontalAlign.Right;
            }
            else if (col.DataType == typeof(bool))
            {
                col.TitleAlign = HorizontalAlign.Center;
                col.ItemAlign = HorizontalAlign.Center;
            }
            return col;
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
            var col1 = new Column();
            var col2 = new Column();
            foreach (var kvp in dictionary)
            {
                col1.List.Add(kvp.Key);
                col2.List.Add(kvp.Value);
            }
            return new TextGrid(new[] { col1, col2 });
        }
    }
}
