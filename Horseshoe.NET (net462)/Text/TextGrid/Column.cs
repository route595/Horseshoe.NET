using System.Collections.Generic;
using System.Linq;

namespace Horseshoe.NET.Text.TextGrid
{
    public class Column<T> : List<T>, IColumn
    {
        private List<string> _renderedList;

        public string Title { get; set; }

        public virtual string Format { get; set; }
        public virtual string DisplayNullAs { get; set; } = "[null]";
        public virtual HorizontalAlign TitleAlign { get; set; }
        public virtual HorizontalAlign ItemAlign { get; set; }

        public int? TargetWidth { get; set; }
        public int CalculatedWidth { get; set; }
        public int WidthToRender => TargetWidth ?? CalculatedWidth;

        public Column() : base()
        {
        }

        public Column(int capacity) : base (capacity)
        {
        }

        public Column(IEnumerable<T> collection) : base(collection)
        {
        }

        public void Prerender()
        {
            _renderedList = new List<string>(this.Select(o => RenderItem(o)));
            CalculatedWidth = _renderedList.Any() ? _renderedList.Max(s => s.Length) : 0;
            if (Title?.Length > CalculatedWidth)
            {
                CalculatedWidth = Title.Length;
            }
        }

        public string RenderItem(T item)
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

        private string Pad(string text, HorizontalAlign align)
        {
            switch(align)
            {
                case HorizontalAlign.Left:
                default:
                    return text.PadRight(WidthToRender);
                case HorizontalAlign.Center:
                    return TextUtil.Pad(text, WidthToRender, Direction.Center);
                case HorizontalAlign.Right:
                    return text.PadLeft(WidthToRender);
            }
        }
    }
}
