using System;
using System.Collections.Generic;

using Horseshoe.NET.ConsoleX;
using Horseshoe.NET.Email;

namespace TestConsole
{
    class EmailTests : RoutineX
    {
        public override IList<MenuObject> Menu => new[]
        {
            BuildMenuRoutine
            (
                "Plain Email",
                () =>
                {
                    PlainEmail.Send("Horseshoe.NET email test", "This is a plain email test.", to: "recipient@email.com", from: "sender@email.net", connectionInfo: new SmtpConnectionInfo { Server = "smtp-relay@email.biz" });
                    Console.WriteLine("Email sent!");
                }
            )
        };
    }
}
