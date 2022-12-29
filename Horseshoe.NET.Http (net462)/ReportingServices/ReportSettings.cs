using Horseshoe.NET.Configuration;

namespace Horseshoe.NET.Http.ReportingServices
{
    public static class ReportSettings
    {
        static string _defaultReportServer;

        /// <summary>
        /// Gets or sets the default Report Server used by ReportingServices.  Note: Overrides other settings (i.e. app|web.config: key = Horseshoe.NET:ReportingServices:ReportServer and OrganizationalDefaultSettings: key = ReportingServices.ReportServer)
        /// </summary>
        public static string DefaultReportServer
        {
            get
            {
                return _defaultReportServer
                    ?? Config.Get("Horseshoe.NET:ReportingServices:ReportServer")
                    ?? OrganizationalDefaultSettings.Get<string>("ReportingServices.ReportServer");
            }
            set
            {
                _defaultReportServer = value;
            }
        }
    }
}
