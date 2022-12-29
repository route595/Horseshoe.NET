namespace Horseshoe.NET.Text
{
    /// <summary>
    /// Typically used in situations when text is padded to a fixed size.  Where the padding is placed can "align" the visible text.
    /// </summary>
    public enum HorizontalPosition
    {
        /// <summary>
        /// Pad right so that text is left aligned
        /// </summary>
        Left,

        /// <summary>
        /// Pad both sides so that text is centered
        /// </summary>
        Center,

        /// <summary>
        /// Pad left so that text is right aligned
        /// </summary>
        Right
    }
}
