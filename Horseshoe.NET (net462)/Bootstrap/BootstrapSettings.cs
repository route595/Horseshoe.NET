using System;

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
                    ?? _Config.GetNEnum<ExceptionRenderingPolicy>("Horseshoe.NET:Bootstrap:ExceptionRendering")
                    ?? OrganizationalDefaultSettings.GetNullable<ExceptionRenderingPolicy>("Bootstrap.ExceptionRendering")
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
                    ?? _Config.GetNBool("Horseshoe.NET:Bootstrap:AutoCloseableAlerts")
                    ?? OrganizationalDefaultSettings.GetNBoolean("Bootstrap.AutoCloseableAlerts")
                    ?? false;
            }
            set
            {
                _defaultAutoCloseableAlerts = value;
            }
        }
    }
}
