using System;
using System.Collections.Generic;
using Microsoft.ApplicationInsights.Channel;

using Serilog.Events;
using Serilog.Sinks.ApplicationInsights.TelemetryConverters;

namespace Horseshoe.NET.ApplicationInsights
{
    public class SerilogAppInsightsTraceTelemetryConverter : TraceTelemetryConverter
    {
        private readonly string roleName;

        public SerilogAppInsightsTraceTelemetryConverter(string roleName)
        {
            this.roleName = roleName;
        }

        public override IEnumerable<ITelemetry> Convert(LogEvent logEvent, IFormatProvider formatProvider)
        {
            foreach (var telemetry in base.Convert(logEvent, formatProvider))
            {
                telemetry.Context.Cloud.RoleName = roleName;
                yield return telemetry;
            }
        }
    }
}
