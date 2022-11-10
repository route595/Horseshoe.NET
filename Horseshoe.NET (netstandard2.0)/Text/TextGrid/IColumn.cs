namespace Horseshoe.NET.Text.TextGrid
{
    public interface IColumn
    {
        string Title { get; set; }
        string Format { get; set; }
        string DisplayNullAs { get; set; }
        HorizontalAlign TitleAlign { get; set; }
        HorizontalAlign ItemAlign { get; set; }

        int? TargetWidth { get; set; }
        int CalculatedWidth { get; set; }
        int WidthToRender { get; }

        int Count { get; }

        void Prerender();

        string[] RenderCell(int index, int paddingLeft, int paddingRight, int paddingTop, int paddingBottom);

        string[] RenderTitleCell(int paddingLeft, int paddingRight, int paddingTop, int paddingBottom);
    }
}
