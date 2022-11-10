using System.Collections.Generic;
using System.Net;

using Horseshoe.NET.Http;
using Horseshoe.NET.IO;

namespace Horseshoe.NET.Http.ReportingServices
{
    public static class ReportServer
    {
        public static string Render(string reportPath, string reportServer = null, IDictionary<string, object> parameters = null, ReportFormat reportFormat = ReportFormat.PDF, string targetFileName = null, string targetDirectory = null, Credential? credentials = null)
        {
            var targetExt = ReportUtil.ConvertOutputTypeToFileType(reportFormat);
            var bytes = RenderBytes(reportPath, reportServer: reportServer, parameters: parameters, reportFormat: reportFormat, credentials: credentials);

            return FileUtil.Create
            (
                bytes,
                targetDirectory: targetDirectory,
                targetFileName: FileUtil.AppendExtension(targetFileName ?? ReportUtil.ParseReportName(reportPath), targetExt),
                fileType: targetExt
            );
        }

        public static byte[] RenderBytes(string url, IDictionary<string, object> parameters = null, Credential? credentials = null)
        {
            try
            {
                return WebDocument.AsBytes(url, credentials: credentials);
            }
            catch (WebException wex)
            {
                throw new ReportException("Web Error (" + wex.Status + "): Check if the report server is operational and double check your report parameters (if applicable), dynamic or user-supplied URL (including report path), report options, credentials, etc.", wex) { Parameters = parameters };
            }
        }

        public static byte[] RenderBytes(string reportPath, string reportServer = null, IDictionary<string, object> parameters = null, ReportFormat reportFormat = ReportFormat.PDF, Credential? credentials = null)
        {
            var url = ReportUtil.BuildFileUrl(reportPath, reportServer: reportServer, parameters: parameters, reportFormat: reportFormat, announce: true);

            return RenderBytes(url, credentials: credentials, parameters: parameters);
        }
    }
}
