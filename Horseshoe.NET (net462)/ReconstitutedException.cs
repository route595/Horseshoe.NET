using System;
using System.Linq;
using System.Text;

using Horseshoe.NET.Text;

namespace Horseshoe.NET
{
    /// <summary>
    /// A specialized exception for rehydrating instances of <c>ExceptionInfo</c> that may come in HTTP responses.
    /// </summary>
    public class ReconstitutedException : Exception
    {
        internal ReconstitutedException(ExceptionInfo exceptionInfo): 
        base
        (
            new StringBuilder()
                .AppendLine("============ BEGIN ORIGINAL EXCEPTION ============")
                .AppendLine("Reconstituted " + Render(exceptionInfo))
                .AppendLine("============= END ORIGINAL EXCEPTION =============")
                .ToString()
        )
        {
        }

        private static string Render(ExceptionInfo exceptionInfo)
        {
            var strb = new StringBuilder();
            _Render(exceptionInfo, strb);
            return strb.ToString();
        }

        private static void _Render(ExceptionInfo exceptionInfo, StringBuilder strb)
        {
            strb.Append(exceptionInfo.FullType)
                .AppendLine(": " + exceptionInfo.Message);
            if (exceptionInfo.DateTime.HasValue)
            {
                strb.AppendLine("Date/Time: " + exceptionInfo.DateTime.Value.ToString("yyyy-MM-dd HH:mm:ss"));
            }
            if (exceptionInfo.SourceLocation != null)
            {
                strb.AppendLine("Source Location: " + exceptionInfo.SourceLocation);
            }
            strb.AppendLine("Stack Trace:")
                .AppendLine(IndentStackTrace(exceptionInfo.StackTrace));
            if (exceptionInfo.InnerException != null)
            {
                strb.AppendLine();
                _Render(exceptionInfo.InnerException, strb);
            }
        }

        private static string IndentStackTrace(string stackTrace)
        {
            if (stackTrace == null)
                return string.Empty;
            var lines = stackTrace.Replace("\r\n", "\n")
                .Split('\n')
                .Select(ln => new string(' ', 2) + ln);
            return string.Join(Environment.NewLine, lines);
        }
    }
}
