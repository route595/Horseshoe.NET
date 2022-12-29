using System;
using System.Collections.Generic;
using System.Linq;

namespace Horseshoe.NET.Text.TextGrid
{
    /// <summary>
    /// A specialized <c>TextGrid</c> column for storing and displaying values of a specific type.
    /// </summary>
    public class TypedColumn : List<object>, IColumn
    {
        private List<string> _renderedList;

        /// <summary>
        /// The type of item to store and display in this grid column.
        /// </summary>
        public Type Type { get; }

        /// <summary>
        /// Column titles, if set, are rendered across the top of the grid above each column.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// An optional format for item rendering.
        /// </summary>
        public virtual string Format { get; set; }

        /// <summary>
        /// How to display null items.
        /// </summary>
        public virtual string DisplayNullAs { get; set; } = TextConstants.Null;

        /// <summary>
        /// The alignment of the title.
        /// </summary>
        public virtual HorizontalPosition TitleAlign { get; set; }

        /// <summary>
        /// The alignment of the data items.
        /// </summary>
        public virtual HorizontalPosition ItemAlign { get; set; }

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
        /// Creates a new <c>TypedColumn</c>.
        /// </summary>
        /// <param name="type">The type of item to store and display in this grid column.</param>
        public TypedColumn(Type type) : base()
        {
            Type = type;
        }

        /// <summary>
        /// Creates a new <c>TypedColumn</c>.
        /// </summary>
        /// <param name="type">The type of item to store and display in this grid column.</param>
        /// <param name="capacity">Specifies the initial capacity.</param>
        public TypedColumn(Type type, int capacity) : base(capacity)
        {
            Type = type;
        }

        /// <summary>
        /// Creates a new <c>TypedColumn</c>.
        /// </summary>
        /// <param name="type">The type of item to store and display in this grid column.</param>
        /// <param name="collection">Elements to copy into this <c>Column</c>.</param>
        public TypedColumn(Type type, IEnumerable<object> collection) : base(Check(type, collection))
        {
            Type = type;
        }

        private static IEnumerable<object> Check(Type type, IEnumerable<object> collection)
        {
            foreach (var obj in collection)
            {
                if (obj == null)
                    continue;
                if (!type.IsAssignableFrom(obj.GetType()))
                    throw new ArgumentException("The supplied collection contains values that do match the target type: " + type);
            }
            return collection;
        }

        /// <summary>
        /// Adds an item to this column.
        /// </summary>
        /// <param name="obj">A item.</param>
        /// <exception cref="ArgumentException"></exception>
        public new void Add(object obj)
        {
            if (obj != null && !Type.IsAssignableFrom(obj.GetType()))
                throw new ArgumentException("The supplied item does not match the target type: " + Type);
            base.Add(obj);
        }

        /// <summary>
        /// Makes some pre-render calculations.
        /// </summary>
        public void Prerender()
        {
            _renderedList = new List<string>(this.Select(o => RenderItem(o)));
            CalculatedWidth = _renderedList.Any() ? _renderedList.Max(s => s.Length) : 0;
            if (Title?.Length > CalculatedWidth)
            {
                CalculatedWidth = Title.Length;
            }
        }

        /// <summary>
        /// Renders each individual item.
        /// </summary>
        /// <param name="item">The item to render.</param>
        /// <returns>The rendered item.</returns>
        public string RenderItem(object item)
        {
            string rendered;
            if (item != null)
            {
                rendered = Format != null
                    ? string.Format("{0:" + Format + "}", item)
                    : item.ToString();
            }
            else
            {
                rendered = DisplayNullAs;
            }
            return TargetWidth.HasValue
                ? TextUtil.Crop(rendered, TargetWidth.Value, truncateMarker: TruncateMarker.LongEllipsis)
                : rendered;
        }

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
            list.Add("".PadLeft(paddingLeft) + Pad(Title ?? "", TitleAlign) + "".PadLeft(paddingRight));
            list.Add("".PadLeft(paddingLeft) + Pad("".PadLeft(Title?.Length ?? 0, '-'), TitleAlign) + "".PadLeft(paddingRight));
            for (int i = 0; i < paddingBottom; i++)
            {
                list.Add("".PadLeft(paddingLeft + WidthToRender + paddingRight));
            }
            return list.ToArray();
        }

        private string Pad(string text, HorizontalPosition align)
        {
            switch (align)
            {
                case HorizontalPosition.Left:
                default:
                    return text.PadRight(WidthToRender);
                case HorizontalPosition.Center:
                    return TextUtil.Pad(text, WidthToRender, HorizontalPosition.Center);
                case HorizontalPosition.Right:
                    return text.PadLeft(WidthToRender);
            }
        }
    }
}
