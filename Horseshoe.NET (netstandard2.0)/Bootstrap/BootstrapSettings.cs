using System;

using Horseshoe.NET.Application;

namespace Horseshoe.NET.Bootstrap
{
    public static class BootstrapSettings
    {
        private static ExceptionRenderingPolicy? _defaultExceptionRendering;

        public static ExceptionRenderingPolicy DefaultExceptionRendering  // example "InAlert"
        {
            get
            {
                return _defaultExceptionRendering
                    ?? GetExceptionRenderingPolicy(_Config.GetNEnum<ExceptionRenderingPolicy>("Horseshoe.NET:Bootstrap:ExceptionRendering"))
                    ?? GetExceptionRenderingPolicy(OrganizationalDefaultSettings.GetNullable<ExceptionRenderingPolicy>("Bootstrap.ExceptionRendering"))
                    ?? ExceptionRenderingPolicy.Preclude;
            }
            set
            {
                _defaultExceptionRendering = value;
            }
        }

        static ExceptionRenderingPolicy? GetExceptionRenderingPolicy(ExceptionRenderingPolicy? exceptionRendering)
        {
            if (exceptionRendering == ExceptionRenderingPolicy.Dynamic)
            {
                switch (ClientApp.AppMode)
                {
                    case AppMode.Production:
                    case AppMode.IA:
                    case AppMode.QA:
                    case AppMode.UAT:
                    case AppMode.Training:
                        return ExceptionRenderingPolicy.Preclude;
                    case AppMode.Development:
                        return ExceptionRenderingPolicy.Visible;
                    case AppMode.Test:
                        return ExceptionRenderingPolicy.Hidden;
                }
            }
            return exceptionRendering;
        }

        static bool? _autoCloseable;

        public static bool DefaultAutoCloseable
        {
            get
            {
                return _autoCloseable
                    ?? _Config.GetNBool("Horseshoe.NET:Bootstrap:AutoCloseable")
                    ?? OrganizationalDefaultSettings.GetNBoolean("Bootstrap.AutoCloseable")
                    ?? false;
            }
            set
            {
                _autoCloseable = value;
            }
        }
    }
}
