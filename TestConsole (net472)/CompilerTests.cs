using System;
using System.Collections.Generic;
using System.CodeDom.Compiler;

using Horseshoe.NET;
using Horseshoe.NET.ConsoleX;
using Horseshoe.NET.ObjectsTypesAndValues;
using Horseshoe.NET.Text;

namespace TestConsole
{
    class CompilerTests : RoutineX
    {
        public override IList<MenuObject> Menu => new[]
        {
            BuildMenuRoutine
            (
                "Compile",
                () =>
                {
                 
                }
            )
        };
    }
}
