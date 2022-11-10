using System;
using System.Collections.Generic;

using Horseshoe.NET.ConsoleX;
using Horseshoe.NET.Http.ReportingServices;

namespace TestConsole.HTTPTests
{
    class SSRSTests : RoutineX
    {
        public override IList<MenuObject> Menu => new[]
        {
            BuildMenuRoutine
            (
                "Build SSRS URLs",
                () =>
                {
                    Console.WriteLine("File: " + ReportUtil.BuildFileUrl("/Accounting/Annual Report", reportServer: "http://localhost", parameters: new Dictionary<string, object> { { "parm1", "ezstr" }, { "parm2", "cr&zy str" } }));
                    Console.WriteLine("Link: " + ReportUtil.BuildHyperlinkUrl("/Accounting/Annual Report", reportServer: "http://localhost", parameters: new Dictionary<string, object> { { "parm1", "ezstr" }, { "parm2", "cr&zy str" } }));
                }
            )
        };
    }
}
