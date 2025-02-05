using Horseshoe.NET.Configuration;

namespace Horseshoe.NET.Bootstrap
{
    /// <summary>
    /// Configuration elements for <c>Horseshoe.NET.Bootstrap</c> classes and methods
    /// </summary>
    public static class BootstrapSettings
    {
        private static ExceptionRenderingPolicy? _defaultExceptionRendering;

        /// <summary>
        /// Exception rendering policy configured by the client, if not configured then <c>default</c>
        /// </summary>
        public static ExceptionRenderingPolicy DefaultExceptionRendering  // example "InAlert"
        {
            get
            {
                return _defaultExceptionRendering
                    ?? Config.Get<ExceptionRenderingPolicy?>("Horseshoe.NET:Bootstrap:ExceptionRendering")
                    ?? default;
            }
            set
            {
                _defaultExceptionRendering = value;
            }
        }

        static bool? _defaultAutoCloseableAlerts;

        /// <summary>
        /// Client configured closeability of alerts, if not configured then <c>false</c>
        /// </summary>
        public static bool DefaultAutoCloseableAlerts
        {
            get
            {
                return _defaultAutoCloseableAlerts
                    ?? Config.Get<bool?>("Horseshoe.NET:Bootstrap:AutoCloseableAlerts")
                    ?? false;
            }
            set
            {
                _defaultAutoCloseableAlerts = value;
            }
        }
    }
}
