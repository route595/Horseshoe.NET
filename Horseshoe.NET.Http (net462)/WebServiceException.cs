using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Horseshoe.NET.Http
{
    public class WebServiceException : Exception
    {
        public WebServiceException() : base() 
        {
        }

        public WebServiceException(string message) : base(message) 
        {
        }

        public WebServiceException(string message, Exception innerException) : base(message, innerException) 
        { 
        }

        public static WebServiceException From(ExceptionInfo exceptionInfo, ExceptionTypeRenderingPolicy typeRendering = default, bool includeStackTrace = false, int indent = 2, bool recursive = false)
        {
            return new WebServiceException
            (
                message:
                    Environment.NewLine + "========  BEGIN ORIGINAL EXCEPTION  ========" + Environment.NewLine +
                    exceptionInfo.Render(typeRendering: typeRendering, includeStackTrace: includeStackTrace, indent: indent, recursive: recursive) +
                    Environment.NewLine + "========  END ORIGINAL EXCEPTION  ========" + Environment.NewLine
            );
        } 
    }
}
