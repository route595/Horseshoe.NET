using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Primitives;

using Horseshoe.NET.Data;

namespace Horseshoe.NET.Text.TextGrid
{
    /// <summary>
    /// A <c>List</c> containing column data in a <c>TextGrid</c>.
    /// </summary>
    public class Column : ColumnBase
    {
        /// <summary>
        /// The list of items contained in this column
        /// </summary>
        public List<object> List { get; }

        private List<string> _renderedList { get; set; }

        /// <summary>
        /// The parent <c>TextGrid</c>
        /// </summary>
        public TextGrid Parent { get; set; }

        /// <summary>
        /// Column titles, if set, are rendered across the top of the grid above each column.
        /// </summary>
        public StringValues Title { get; set; }

        /// <summary>
        /// The alignment of the title.
        /// </summary>
        public virtual HorizontalAlign TitleAlign { get; set; }

        /// <summary>
        /// The alignment of the data items.
        /// </summary>
        public virtual HorizontalAlign ItemAlign { get; set; }

        /// <summary>
        /// The desired width of the column.
        /// </summary>
        public int? TargetWidth { get; set; }

        /// <summary>
        /// The natural of the column (i.e. the max width of the items and title).
        /// </summary>
        public int CalculatedWidth { get; set; }

        /// <summary>
        /// The final width of this column.
        /// </summary>
        public int WidthToRender => TargetWidth ?? CalculatedWidth;

        /// <summary>
        /// The number of items in this column
        /// </summary>
        public int Count => List.Count;

        /// <summary>
        /// Creates a new <c>Column</c>.
        /// </summary>
        public Column() : base()
        {
            List = new List<object>();
        }

        /// <summary>
        /// Creates a new column based on the argument column.
        /// </summary>
        /// <param name="columnBase">A column upon which to base the new column.</param>
        public Column(ColumnBase columnBase) : base(columnBase)
        {
            List = new List<object>();
        }

        /// <summary>
        /// Creates a new column with the supplied capacity.
        /// </summary>
        /// <param name="capacity">Specifies the initial capacity.</param>
        public Column(int capacity)
        {
            List = new List<object>(capacity);
        }

        /// <summary>
        /// Creates a new column with the supplied capacity based on the argument column.
        /// </summary>
        /// <param name="columnBase">A column upon which to base the new column.</param>
        /// <param name="capacity">Specifies the initial capacity.</param>
        public Column(ColumnBase columnBase, int capacity) : base(columnBase)
        {
            List = new List<object>(capacity);
        }

        /// <summary>
        /// Creates a new column with the supplied elements.
        /// </summary>
        /// <param name="collection">Elements to copy into this <c>Column</c>.</param>
        public Column(IEnumerable<object> collection)
        {
            List = new List<object>(collection);
        }

        /// <summary>
        /// Creates a new column with the supplied elements based on the argument column.
        /// </summary>
        /// <param name="columnBase">A column upon which to base the new column.</param>
        /// <param name="collection">Elements to copy into this <c>Column</c>.</param>
        public Column(ColumnBase columnBase, IEnumerable<object> collection) : base(columnBase)
        {
            List = new List<object>(collection);
        }

        /// <summary>
        /// Gets the item at <c>index</c> cast to the supplied type
        /// </summary>
        /// <typeparam name="T">A value or reference type</typeparam>
        /// <param name="index">The 0-based index of the item in the column (list)</param>
        /// <returns>The item at <c>index</c></returns>
        /// <exception cref="System.ArgumentOutOfRangeException"></exception>
        public T CastItemAt<T>(int index)
        {
            return (T)List[index];
        }

        /// <summary>
        /// Makes some pre-render calculations.
        /// </summary>
        public void Prerender()
        {
            _renderedList = Render();
            CalculatedWidth = _renderedList.Any() ? _renderedList.Max(s => s.Length) : 0;
            if (!Title.Any() && Name != null)
                Title = Name;
            if (Title.Any() && Title.Max(s => (s ?? "").Length) > CalculatedWidth)
                CalculatedWidth = Title.Max(s => (s ?? "").Length);
        }

        /// <summary>
        /// Renders each item to <c>string</c> and returns them as a <c>List</c>
        /// </summary>
        /// <returns>A <c>List</c> of <c>string</c>-rendered column data</returns>
        public List<string> Render()
        {
            return List
                .Select(o => RenderItem(o))
                .ToList();
        }

        /// <summary>
        /// Renders an individual item.
        /// </summary>
        /// <param name="item">The item to render</param>
        /// <returns>The rendered item</returns>
        public string RenderItem(object item)
        {
            return TargetWidth.HasValue
                ? TextUtil.Crop(Format(item), TargetWidth.Value, truncateMarker: TruncateMarker.LongEllipsis)
                : Format(item);
        }

        ///// <summary>
        ///// Renders the individual item at position <c>index</c>.
        ///// </summary>
        ///// <param name="index">The 0-based index of the item in the column (list)</param>
        ///// <returns>The rendered item</returns>
        ///// <exception cref="System.ArgumentOutOfRangeException"></exception>
        //public string RenderItemAt(int index)
        //{
        //    return RenderItem(this[index]);
        //}

        /// <summary>
        /// Renders an individual cell with padding if applicable.
        /// </summary>
        /// <param name="index">The row index.</param>
        /// <param name="paddingLeft">Left padding.</param>
        /// <param name="paddingRight">Right padding.</param>
        /// <param name="paddingTop">Top padding.</param>
        /// <param name="paddingBottom">Bottom padding.</param>
        /// <returns>The cell rendered as an array.</returns>
        /// <exception cref="ValidationException"></exception>
        public string[] RenderCell(int index, int paddingLeft, int paddingRight, int paddingTop, int paddingBottom)
        {
            if (index < 0)
            {
                throw new ValidationException("index cannot be less than zero");
            }
            var list = new List<string>();
            string text = "";
            if (index < Count)
            {
                text = _renderedList[index];
            }
            for (int i = 0; i < paddingTop; i++)
            {
                list.Add("".PadLeft(paddingLeft + WidthToRender + paddingRight));
            }
            list.Add("".PadLeft(paddingLeft) + Pad(text, ItemAlign) + "".PadLeft(paddingRight));
            for (int i = 0; i < paddingBottom; i++)
            {
                list.Add("".PadLeft(paddingLeft + WidthToRender + paddingRight));
            }
            return list.ToArray();
        }

        /// <summary>
        /// Renders the title cell with padding if applicable.
        /// </summary>
        /// <param name="paddingLeft">Left padding.</param>
        /// <param name="paddingRight">Right padding.</param>
        /// <param name="paddingTop">Top padding.</param>
        /// <param name="paddingBottom">Bottom padding.</param>
        /// <returns>The cell rendered as an array.</returns>
        public string[] RenderTitleCell(int paddingLeft, int paddingRight, int paddingTop, int paddingBottom)
        {
            var list = new List<string>();
            for (int i = 0; i < paddingTop; i++)
            {
                list.Add("".PadLeft(paddingLeft + WidthToRender + paddingRight));
            }
            for (int i = 0; i < Parent?.TitleHeight; i++)
            {
                if (Title.Count > i)
                    list.Add("".PadLeft(paddingLeft) + Pad(Title[i] ?? "", TitleAlign) + "".PadLeft(paddingRight));
                else
                    list.Add("".PadLeft(paddingLeft) + Pad("", TitleAlign) + "".PadLeft(paddingRight));
            }
            list.Add("".PadLeft(paddingLeft) + Pad("".PadLeft(Title.Max(s => (s ?? "").Length), '-'), TitleAlign) + "".PadLeft(paddingRight));
            for (int i = 0; i < paddingBottom; i++)
            {
                list.Add("".PadLeft(paddingLeft + WidthToRender + paddingRight));
            }
            return list.ToArray();
        }

        private string Pad(string text, HorizontalAlign align)
        {
            switch (align)
            {
                case HorizontalAlign.Left:
                default:
                    return text.PadRight(WidthToRender);
                case HorizontalAlign.Center:
                    return TextUtil.Pad(text, WidthToRender, HorizontalPosition.Center);
                case HorizontalAlign.Right:
                    return text.PadLeft(WidthToRender);
            }
        }
    }
}
