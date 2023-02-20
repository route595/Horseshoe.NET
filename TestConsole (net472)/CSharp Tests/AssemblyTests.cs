using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Horseshoe.NET.ConsoleX;

namespace TestConsole.CSharpTests
{
    class AssemblyTests : RoutineX
    {
        public override IList<MenuObject> Menu => new[]
        {
            BuildMenuRoutine
            (
               "Load assembly by name",
               () =>
               {
                   var name = PromptX.Value<string>("assembly name");
                   Assembly assembly = Assembly.Load(name);
                   Console.WriteLine(assembly?.FullName ?? "[null]");
               }
            ),
            BuildMenuRoutine
            (
               "Load Newtonsoft.Json",
               () =>
               {
                   Assembly assembly = Assembly.Load("Newtonsoft.Json");
                   Console.WriteLine(assembly?.FullName ?? "[null]");
               }
            ),
            BuildMenuRoutine
            (
               "Load System.Text.Json",
               () =>
               {
                   Assembly assembly = Assembly.Load("System.Text.Json");
                   Console.WriteLine(assembly?.FullName ?? "[null]");
               }
            ),
            BuildMenuRoutine
            (
               "List assemblies",
               () =>
               {
                   RenderX.List(AppDomain.CurrentDomain.GetAssemblies());
               }
            )
        };
    }
}
