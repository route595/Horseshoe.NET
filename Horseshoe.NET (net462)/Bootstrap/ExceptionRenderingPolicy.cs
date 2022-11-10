namespace Horseshoe.NET.Bootstrap
{
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
        Hidden,

        /// <summary>
        /// Render exception details only if in Development (see Horseshoe.NET AppMode), otherwise do not (requires configuration i.e. in app|web.config or appsettings.json set Horseshoe.NET:AppMode)
        /// </summary>
        Dynamic
    }
}
