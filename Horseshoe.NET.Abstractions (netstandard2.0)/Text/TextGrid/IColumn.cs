namespace Horseshoe.NET.Text.TextGrid
{
    /// <summary>
    /// Template for columns that can be used to render <c>TextGrid</c>s
    /// </summary>
    public interface IColumn
    {
        /// <summary>
        /// Column titles, if set, are rendered across the top of the grid above each column.
        /// </summary>
        string Title { get; set; }

        /// <summary>
        /// An optional format for item rendering.
        /// </summary>
        string Format { get; set; }

        /// <summary>
        /// How to display null items.
        /// </summary>
        string DisplayNullAs { get; set; }

        /// <summary>
        /// The alignment of the title.
        /// </summary>
        HorizontalPosition TitleAlign { get; set; }

        /// <summary>
        /// The alignment of the data items.
        /// </summary>
        HorizontalPosition ItemAlign { get; set; }

        /// <summary>
        /// The desired width of the column.
        /// </summary>
        int? TargetWidth { get; set; }

        /// <summary>
        /// The natural of the column (i.e. the max width of the items and title).
        /// </summary>
        int CalculatedWidth { get; set; }

        /// <summary>
        /// The final width of this column.
        /// </summary>
        int WidthToRender { get; }

        /// <summary>
        /// The number of items in the column.
        /// </summary>
        int Count { get; }

        /// <summary>
        /// Code to execute before rendering a column (e.g. calculate column width)
        /// </summary>
        void Prerender();

        /// <summary>
        /// Renders an individual cell with padding if applicable.
        /// </summary>
        /// <param name="index">The row index.</param>
        /// <param name="paddingLeft">Left padding.</param>
        /// <param name="paddingRight">Right padding.</param>
        /// <param name="paddingTop">Top padding.</param>
        /// <param name="paddingBottom">Bottom padding.</param>
        /// <returns>The cell rendered as an array.</returns>
        string[] RenderCell(int index, int paddingLeft, int paddingRight, int paddingTop, int paddingBottom);


        /// <summary>
        /// Renders the title cell with padding if applicable.
        /// </summary>
        /// <param name="paddingLeft">Left padding.</param>
        /// <param name="paddingRight">Right padding.</param>
        /// <param name="paddingTop">Top padding.</param>
        /// <param name="paddingBottom">Bottom padding.</param>
        /// <returns>The cell rendered as an array.</returns>
        string[] RenderTitleCell(int paddingLeft, int paddingRight, int paddingTop, int paddingBottom);
    }
}
