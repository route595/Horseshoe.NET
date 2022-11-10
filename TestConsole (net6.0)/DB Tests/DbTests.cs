using System;
using System.Collections.Generic;

using Horseshoe.NET.ConsoleX;
using Horseshoe.NET.Db;

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
                    DoFilterTest("Filter.Column(\"age\").Between(5, 8)", () => Filter.Column("age").Between(5, 8));
                    DoFilterTest("Filter.Column(\"age\").Between(null, 8)", () => Filter.Column("age").Between(null, 8));
                    DoFilterTest("Filter.Column(\"age\").Between(5, 8, filterBounds: FilterBounds.Exclusive)", () => Filter.Column("age").Between(5, 8, filterBounds: FilterBounds.Exclusive));
                    DoFilterTest("Filter.Column(\"age\").Between(5, 8, filterBounds: FilterBounds.ExclusiveLowerBoundOnly)", () => Filter.Column("age").Between(5, 8, filterBounds: FilterBounds.ExclusiveLowerBoundOnly));
                    DoFilterTest("Filter.Column(\"age\").NotBetween(5, 8)", () => Filter.Column("age").NotBetween(5, 8));
                    DoFilterTest("Filter.Column(\"age\").NotBetween(5, 8, filterBounds: FilterBounds.Inclusive)", () => Filter.Column("age").NotBetween(5, 8, filterBounds: FilterBounds.Inclusive));
                    DoFilterTest("Filter.Column(\"age\").NotBetween(5, 8, filterBounds: FilterBounds.InclusiveLowerBoundOnly)", () => Filter.Column("age").NotBetween(5, 8, filterBounds: FilterBounds.InclusiveLowerBoundOnly));
                    DoFilterTest("Filter.Column(\"birthdate\").GreaterThan(\"10/17/2020\")", () => Filter.Column("birthdate").GreaterThan("10/17/2020"));
                    DoFilterTest("Filter.Column(\"birthdate\").GreaterThan(DateTime.Today)", () => Filter.Column("birthdate").GreaterThan(DateTime.Today));
                    DoFilterTest("Filter.Column(\"birthdate\").GreaterThan(DateTime.Now)", () => Filter.Column("birthdate").GreaterThan(DateTime.Now));
                    DoFilterTest("Filter.Literal(\"birthdate > GETDATE()\")", () => Filter.Literal("birthdate > GETDATE()"));
                    DoFilterTest("Filter.Column(\"birthdate\").Literal(\"> GETDATE()\")", () => Filter.Column("birthdate").Literal("> GETDATE()"));
                    DoFilterTest("Filter.Column(\"birthdate\").GreaterThan(SqlLiteral.CurrentDate(DbPlatform.SqlServer))", () => Filter.Column("birthdate").GreaterThan(SqlLiteral.CurrentDate(DbPlatform.SqlServer)));
                    DoFilterTest("Filter.Column(\"birthdate\").GreaterThanOrEqual(new SqlLiteral(\"GETDATE()\"))", () => Filter.Column("birthdate").GreaterThanOrEqual(new SqlLiteral("GETDATE()")));
                    DoFilterTest("Filter.Column(\"name\").StartsWith(\"Sha\")", () => Filter.Column("name").StartsWith("Sha"));
                    DoFilterTest("Filter.Column(\"name\").NotStartsWith(\"Sha\")", () => Filter.Column("name").NotStartsWith("Sha"));
                    DoFilterTest("Filter.Column(\"name\").EndsWith(\"non\")", () => Filter.Column("name").EndsWith("non"));
                    DoFilterTest("Filter.Column(\"name\").Contains(\"anno\")", () => Filter.Column("name").Contains("anno"));
                    DoFilterTest("Filter.Column(\"name\").In(\"Steve\", \"Allan\")", () => Filter.Column("name").In("Steve", "Allan"));
                    DoFilterTest("Filter.Column(\"name\").NotIn(\"Steve\", \"Allan\")", () => Filter.Column("name").NotIn("Steve", "Allan"));
                    DoFilterTest("Filter.Column(\"name\").IsNull()", () => Filter.Column("name").IsNull());
                    DoFilterTest("Filter.Column(\"name\").IsNotNull()", () => Filter.Column("name").IsNotNull());
                    DoFilterTest("Filter.Column(\"object_id\").Equals(new SqlLiteral(\"OBJECT_ID('dbo.table12345')\"))", () => Filter.Column("object_id").Equals(new SqlLiteral("OBJECT_ID('dbo.table12345')")));
                    DoFilterTest("Filter.Column(\"user_type_id\").In(167, 175, 231, 239)", () => Filter.Column("user_type_id").In(167, 175, 231, 239));
                    DoFilterTest("Filter.Expression(\"LEFT(name, 1)\").In('A', 'B', 'C')", () => Filter.Expression("LEFT(name, 1)").In('A', 'B', 'C'));

                    void DoFilterTest(string command, Func<Filter> func)
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
