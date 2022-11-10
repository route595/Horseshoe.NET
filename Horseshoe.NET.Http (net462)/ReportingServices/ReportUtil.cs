using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

using Horseshoe.NET.IO;
using Horseshoe.NET.Text;

namespace Horseshoe.NET.Http.ReportingServices
{
    public static class ReportUtil
    {
        public static event Action<string> ReportUrlGenerated;

        public static string BuildFileUrl(string reportPath, string reportServer = null, IDictionary<string, object> parameters = null, ReportFormat reportFormat = ReportFormat.PDF, bool announce = false)
        {
            if (reportPath == null) throw new ArgumentNullException(nameof(reportPath));
            reportServer = reportServer ?? ReportSettings.DefaultReportServer;
            if (reportServer == null) throw new ArgumentNullException(nameof(reportServer));
            if (reportServer.EndsWith("/")) reportServer = reportServer.Substring(0, reportServer.Length - 1);  // remove trailing slash, if applicable
            var parameterPortionOfQueryString = BuildReportParameterString(parameters);
            var sb = new StringBuilder(reportServer)      // e.g. http://reports.mycompany.com
                .Append("/ReportServer?")
                .AppendIf(!reportPath.StartsWith("/"), "/")
                .Append(reportPath.Replace(" ", "%20"))   // e.g. /Accounting/Expense Report => /Acct/Expense%20Report
                .Append(parameterPortionOfQueryString)    // e.g. &name=Bob%20Cratchit
                .Append("&rs:Command=Render")
                .Append("&rs:Format=")
                .Append(reportFormat);                    // e.g. PDF, EXCEL
            if (announce)
            {
                ReportUrlGenerated?.Invoke(sb.ToString());
            }
            return sb.ToString();                         // http://reports.mycompany.com/ReportServer?/Acct/Expense%20Report&user=Bob%20Cratchit&rs:Command=Render&rs:Format=PDF
        }

        public static string BuildHyperlinkUrl(string reportPath, string reportServer = null, IDictionary<string, object> parameters = null, bool announce = false)
        {
            if (reportPath == null) throw new ArgumentNullException(nameof(reportPath));
            reportServer = reportServer ?? ReportSettings.DefaultReportServer;
            if (reportServer == null) throw new ArgumentNullException(nameof(reportServer));
            if (reportServer.EndsWith("/")) reportServer = reportServer.Substring(0, reportServer.Length - 1);  // remove trailing slash, if applicable
            var parameterPortionOfQueryString = BuildReportParameterString(parameters);
            var sb = new StringBuilder(reportServer)      // e.g. http://reports.mycompany.com
                .Append("/reports/report")
                .AppendIf(!reportPath.StartsWith("/"), "/")
                .Append(reportPath.Replace(" ", "%20"))   // e.g. /Accounting/Expense Report => /Accounting/Expense%20Report
                .Append(parameterPortionOfQueryString);   // e.g. &name=Bob%20Cratchit
            if (announce)
            {
                ReportUrlGenerated?.Invoke(sb.ToString());
            }
            return sb.ToString();                         // http://reports.mycompany.com/reports/report/Acct/Expense%20Report&user=Bob%20Cratchit
        }

        public static string BuildReportParameterString(IDictionary<string, object> parameters)
        {
            if (parameters == null || !parameters.Any())
            {
                return "";
            }
            return "&" + string.Join("&", parameters.Select(pkvp => pkvp.Key + "=" + HttpUtility.UrlEncode(string.Join(",", ParseParamValues(pkvp.Value)))));
        }

        public static string ParseReportName(string reportPath)
        {
            var reportName = reportPath.Any(c => c == '/')
                ? reportPath.Substring(reportPath.LastIndexOf("/") + 1)
                : reportPath;
            return reportName;
        }

        internal static string[] ParseParamValues(object o)
        {
            if (o == null)
            {
                return new string[] { null };
            }
            else if (o is DateTime dateTime)
            {
                return new string[] { dateTime.ToShortDateString() };
            }
            else if (o is IEnumerable<string> strings)
            {
                return strings.ToArray();
            }
            else if (o is IEnumerable<int> ints)
            {
                return ints.Select(n => n.ToString()).ToArray();
            }
            else if (o is IEnumerable<int?> nInts)
            {
                return nInts.Select(n => n?.ToString()).ToArray();
            }
            else if (o is IEnumerable<double> doubles)
            {
                return doubles.Select(n => n.ToString()).ToArray();
            }
            else if (o is IEnumerable<double?> nDoubles)
            {
                return nDoubles.Select(n => n?.ToString()).ToArray();
            }
            else if (o is IEnumerable<long> longs)
            {
                return longs.Select(n => n.ToString()).ToArray();
            }
            else if (o is IEnumerable<long?> nLongs)
            {
                return nLongs.Select(n => n?.ToString()).ToArray();
            }
            else if (o is IEnumerable<decimal> decimals)
            {
                return decimals.Select(n => n.ToString()).ToArray();
            }
            else if (o is IEnumerable<decimal?> nDecimals)
            {
                return nDecimals.Select(n => n?.ToString()).ToArray();
            }
            else if (o is IEnumerable<DateTime> dateTimes)
            {
                return dateTimes.Select(n => n.ToShortDateString()).ToArray();
            }
            else if (o is IEnumerable<DateTime?> nDateTimes)
            {
                return nDateTimes.Select(n => n?.ToShortDateString()).ToArray();
            }
            return new string[] { o.ToString() };
        }

        internal static FileType ConvertOutputTypeToFileType(ReportFormat reportOutputType)
        {
            switch (reportOutputType)
            {
                case ReportFormat.EXCEL:
                    return FileType.XLS;
                case ReportFormat.PDF:
                default:
                    return FileType.PDF;
            }
        }
    }
}
