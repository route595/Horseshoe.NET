using System;
using System.Collections.Generic;
using System.Linq;
using Horseshoe.NET;
using Horseshoe.NET.ConsoleX;
using Horseshoe.NET.XmlDoc;

namespace TestConsole
{
    class XMLTests : RoutineX
    {
        public override IList<MenuObject> Menu => new MenuObject[]
        {
            new MenuHeader("USER ROUTINES"),
            BuildMenuRoutine
            (
                "Parse documentation XML",
                () =>
                {
                    var xmlDoc = new XmlDoc().Fill
                    (
                        "Horseshoe.NET.xml", 
                        fillInMissingTypes: true, 
                        journal: new TraceJournal((level, entry) => Console.WriteLine((level >= 0 ? new string(' ', level * 2) : "<") + entry))
                    );
                    Console.WriteLine();
                    Console.WriteLine("Namespaces");
                    Console.WriteLine("-------");
                    Console.WriteLine(string.Join(Environment.NewLine, xmlDoc.GetDocumentedNamespaces()));
                    Console.WriteLine();
                    Console.WriteLine("Verified Types");
                    Console.WriteLine("-------");
                    Console.WriteLine(string.Join(Environment.NewLine, xmlDoc.VerifiedTypes.OrderBy(kvp => kvp.Key).Select(kvp => kvp.Key + " -> " + kvp.Value.FullName)));
                    Console.WriteLine();
                    Console.WriteLine("Verified Namespaces");
                    Console.WriteLine("-------");
                    Console.WriteLine(string.Join(Environment.NewLine, xmlDoc.VerifiedNamespaces));
                }
            )
        };
    }
}