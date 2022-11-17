namespace Horseshoe.NET.Bootstrap
{
    /// <summary>
    /// Whether and how to render exception details in Bootstrap 'danger' alerts
    /// </summary>
    public enum ExceptionRenderingPolicy
    {
        /// <summary>
        /// Do not render exception details (default)
        /// </summary>
        Preclude,

        /// <summary>
        /// Render exception details (recommendation: hide initially, click to toggle or show)
        /// </summary>
        Visible,

        /// <summary>
        /// Render exception details to a nonvisible element only
        /// </summary>
        Hidden
    }
}
