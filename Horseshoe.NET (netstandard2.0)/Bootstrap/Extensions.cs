namespace Horseshoe.NET.Bootstrap
{
    /// <summary>
    /// Extension methods
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Convert an <c>AlertType</c> to a corresponding Bootstrap css class
        /// </summary>
        /// <param name="alertType"></param>
        /// <returns></returns>
        public static string ToCssClass(this Bootstrap3.AlertType alertType)
        {
            switch (alertType)
            {
                case Bootstrap3.AlertType.Error:
                    return "alert-danger";
                default:
                    return "alert-" + alertType.ToString().ToLower();
            }
        }

        /// <summary>
        /// Convert an <c>AlertType</c> to a corresponding Bootstrap css class
        /// </summary>
        /// <param name="alertType"></param>
        /// <returns></returns>
        public static string ToCssClass(this Bootstrap4.AlertType alertType)
        {
            switch (alertType)
            {
                case Bootstrap4.AlertType.Error:
                    return "alert-danger";
                default:
                    return "alert-" + alertType.ToString().ToLower();
            }
        }

        internal static AlertMessageDetailsRenderingPolicy ToAlertMessageDetailsRendering(this ExceptionRenderingPolicy exceptionRendering)
        {
            switch (exceptionRendering)
            {
                case ExceptionRenderingPolicy.Visible:
                    return AlertMessageDetailsRenderingPolicy.EncodeHtml | AlertMessageDetailsRenderingPolicy.PreFormatted;
                case ExceptionRenderingPolicy.Hidden:
                default:
                    return AlertMessageDetailsRenderingPolicy.Default;
            }
        }
    }
}
