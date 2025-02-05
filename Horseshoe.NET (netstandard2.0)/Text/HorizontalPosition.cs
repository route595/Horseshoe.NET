namespace Horseshoe.NET.Text
{
    /// <summary>
    /// Where in a <c>string</c> to pad or crop.
    /// </summary>
    public enum HorizontalPosition
    {
        /// <summary>
        /// Pad or crop the right side of a <c>string</c>, this is the default. 
        /// Results in left-aligned padded text or 'abcdef...' type cropping.
        /// </summary>
        Right,

        /// <summary>
        /// Pad or crop the left side of a <c>string</c>. 
        /// Results in right-aligned padded text or '...uvwxyz' type cropping.
        /// </summary>
        Left,

        /// <summary>
        /// Pad both sides of a <c>string</c> or crop the center. 
        /// Results in center-aligned padded text or 'abcdef...uvwxyz' type cropping.
        /// </summary>
        Center
    }
}
