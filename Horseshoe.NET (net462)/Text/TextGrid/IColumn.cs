namespace Horseshoe.NET.Text.TextGrid
{
    /// <summary>
    /// Template for columns that can be used to render <c>TextGrid</c>s
    /// </summary>
    public interface IColumn
    {
        /// <summary>
        /// Column title
        /// </summary>
        string Title { get; set; }
        /// <summary>
        /// Data format
        /// </summary>
        string Format { get; set; }
        /// <summary>
        /// Null format
        /// </summary>
        string DisplayNullAs { get; set; }
        /// <summary>
        /// Horizontal alignment of the column title
        /// </summary>
        HorizontalAlign TitleAlign { get; set; }
        /// <summary>
        /// Horizontal alignment of datum
        /// </summary>
        HorizontalAlign ItemAlign { get; set; }

        int? TargetWidth { get; set; }
        int CalculatedWidth { get; set; }
        int WidthToRender { get; }

        int Count { get; }

        /// <summary>
        /// Code to execute before rendering a column (e.g. calculate column width)
        /// </summary>
        void Prerender();

        string[] RenderCell(int index, int paddingLeft, int paddingRight, int paddingTop, int paddingBottom);

        string[] RenderTitleCell(int paddingLeft, int paddingRight, int paddingTop, int paddingBottom);
    }
}
