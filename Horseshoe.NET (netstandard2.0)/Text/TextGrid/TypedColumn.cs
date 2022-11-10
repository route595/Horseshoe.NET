using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Horseshoe.NET.Text.TextGrid
{
    public class TypedColumn : List<object>, IColumn
    {
        private List<string> _renderedList;

        public Type Type { get; }
        public string Title { get; set; }

        public virtual string Format { get; set; }
        public virtual string DisplayNullAs { get; set; } = "[null]";
        public virtual HorizontalAlign TitleAlign { get; set; }
        public virtual HorizontalAlign ItemAlign { get; set; }

        public int? TargetWidth { get; set; }
        public int CalculatedWidth { get; set; }
        public int WidthToRender => TargetWidth ?? CalculatedWidth;

        public TypedColumn(Type type) : base()
        {
            Type = type;
        }

        public TypedColumn(Type type, int capacity) : base(capacity)
        {
            Type = type;
        }

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

        public new void Add(object obj)
        {
            if (obj != null && !Type.IsAssignableFrom(obj.GetType()))
                throw new ArgumentException("The supplied item does not match the target type: " + Type);
            base.Add(obj);
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
            switch (align)
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
