using System;
using System.Collections.Generic;

using Horseshoe.NET.ConsoleX;
using Horseshoe.NET.Compare;
using Horseshoe.NET.Db;
using Horseshoe.NET.Primitives;

namespace TestConsole.DbTests
{
    class DBTests : RoutineX
    {
        public override IList<MenuObject> Menu => new[]
        {
            BuildMenuRoutine
            (
                "Filter Test",
                () =>
                {
                    DoFilterTest("\"age\" between 5 and 8", () => Filter.Between("age", 5, 8));
                    DoFilterTest("\"age\" between 8 and 5", () => Filter.Between("age", 8, 5));
                    DoFilterTest("\"age\" between 5x and 8x", () => Filter.BetweenExclusive("age", 5, 8));
                    DoFilterTest("\"age\" between 5x and 8", () => Filter.BetweenExclusiveLo("age", 5, 8));
                    DoFilterTest("\"age\" between 5 and 8x", () => Filter.BetweenExclusiveHi("age", 5, 8));
                    DoFilterTest("\"age\" between null and 8", () => new Filter{ ColumnName = "age", Mode = CompareMode.Between, Criteria = ObjectValues.From(null, 8) });
                    DoFilterTest("\"age\" not between 5 and 8", () => Filter.Not().Between("age", 5, 8));
                    DoFilterTest("\"name\" is \'Steve\' or \'Allan\'", () => Filter.In("name", new[] { "Steve", "Allan" }));
                    DoFilterTest("\"name\" is neither \'Steve\' nor \'Allan\'", () => Filter.Not().In("name", new[] { "Steve", "Allan" }));
                    DoFilterTest("\"name\" is null", () => Filter.IsNull("name"));
                    DoFilterTest("\"name\" is not null", () => Filter.Not().IsNull("name"));
                    DoFilterTest("\"object_id\" equals OBJECT_ID('dbo.table12345')", () => Filter.Literal("object_id", "OBJECT_ID('dbo.table12345')"));
                    DoFilterTest("\"due_date\" after today", () => Filter.GreaterThan("due_date", SqlLiteral.CurrentDate(DbPlatform.SqlServer)));
                    DoFilterTest("\"due_date\" after today", () => Filter.Literal("due_date > GETDATE()"));
                    DoFilterTest("\"due_date\" after today", () => Filter.Literal("due_date", "> GETDATE()"));
                    DoFilterTest("first letter in \"name\" is 'A', 'B' or 'C')", () => Filter.In(new ColumnExpression("name", "LEFT({0}, 1)"), new[] { 'A', 'B', 'C' }));

                    void DoFilterTest(string command, Func<IFilter> func)
                    {
                        Console.Write(command + "  >>  ");
                        try
                        {
                            Console.WriteLine("\"" + func.Invoke() + "\"");
                        }
                        catch(Exception ex)
                        {
                            Console.WriteLine(ex.Message + " (" + ex.GetType().Name + ")");
                        }
                    }
                }
            )
        };
    }
}
