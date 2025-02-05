namespace Horseshoe.NET.Text
{
    /// <summary>
    /// Alignment constants for fixed-width <c>string</c>s
    /// </summary>
    public enum HorizontalAlign
    {
        /// <summary>
        /// Left aligns fixed-width <c>string</c>s, this is the default. 
        /// Result accomplished via right-padding text e.g. 'abcdef....'.
        /// </summary>
        Left,

        /// <summary>
        /// Right aligns fixed-width <c>string</c>s. 
        /// Result accomplished via left-padding text e.g. '....uvwxyz'.
        /// </summary>
        Right,

        /// <summary>
        /// Center aligns fixed-width <c>string</c>s. 
        /// Result accomplished via left- and right-padding text e.g. '..klmnop..'.
        /// </summary>
        Center
    }
}
