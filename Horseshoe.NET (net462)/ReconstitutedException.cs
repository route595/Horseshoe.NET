using System;
using System.Text;

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
                .AppendLine("Reconstituted " + exceptionInfo.Render(includeStackTrace: true, recursive: true))
                .AppendLine("============= END ORIGINAL EXCEPTION =============")
                .ToString()
        )
        {
        }
    }
}
