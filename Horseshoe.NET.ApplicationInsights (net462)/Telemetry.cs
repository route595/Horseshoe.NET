using System;
using System.Collections.Generic;
using System.Configuration;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;

using Horseshoe.NET.Collections;
using Horseshoe.NET.Configuration;

namespace Horseshoe.NET.ApplicationInsights
{
    public static class Telemetry
    {
        /* * * * * * * * * * * * * * * * * * 
         *      SETUP / CONFIGURATION
         * * * * * * * * * * * * * * * * * */

        private static string appSettingsName_ConnectionString;
        private static string appSettingsName_CloudRoleName;

        public static void RegisterAppSettingsNames(string appSettingsName_ConnectionString = TelemetryConstants.AppSettingsName_ConnectionString, string appSettingsName_CloudRoleName = TelemetryConstants.AppSettingsName_CloudRoleName)
        {
            Telemetry.appSettingsName_ConnectionString = appSettingsName_ConnectionString;
            Telemetry.appSettingsName_CloudRoleName = appSettingsName_CloudRoleName;
        }

        static Telemetry()
        {
            RegisterAppSettingsNames();
        }

        /* * * * * * * * * * * * * * * * * * 
         *        TELEMETRY  CLIENT
         * * * * * * * * * * * * * * * * * */

        private static TelemetryClient _telemetryClient;

        public static TelemetryClient GetClient()
        {
            if (_telemetryClient == null)
            {
                var connectionString = Config.Get(appSettingsName_ConnectionString, required: true);
                var telemetryConfig = TelemetryConfiguration.CreateDefault();
                telemetryConfig.ConnectionString = connectionString;
                _telemetryClient = new TelemetryClient(telemetryConfig);
                _telemetryClient.Context.Cloud.RoleName = Config.Get(appSettingsName_CloudRoleName);
            }
            return _telemetryClient;
        }

        /// <inheritdoc cref="TelemetryClient.TrackException(Exception, IDictionary{string, string}, IDictionary{string, double})"/>
        public static void TrackException(Exception ex, IDictionary<string, string> properties = null, IDictionary<string, double> metrics = null)
        {
            var client = GetClient();
            if (ex == null) 
            {
                client.TrackException(new Exception("exception was null"), properties: properties, metrics: metrics);
            }
            else
            {
                properties = DictionaryUtil.CombineLTR
                (
                    properties, 
                    new Dictionary<string, string> 
                    { 
                        { "methodWhereOccurred", ex.TargetSite.Name }, 
                        { "classWhereOccurred", ex.TargetSite.DeclaringType.FullName }, 
                        { "exceptionClassName", ex.GetType().FullName }, 
                        { "exceptionMessage", ex.Message } 
                    }
                );
                client.TrackException(ex, properties: properties, metrics: metrics);
            }
            client.Flush();
        }

        /// <inheritdoc cref="TelemetryClient.TrackEvent(string, IDictionary{string, string}, IDictionary{string, double})"/>
        public static void TrackEvent(string eventName, IDictionary<string, string> properties = null, IDictionary<string, double> metrics = null)
        {
            var client = GetClient();
            client.TrackEvent(eventName, properties: properties, metrics: metrics);
            client.Flush();
        }
    }
}
