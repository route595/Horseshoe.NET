//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//using Horseshoe.NET.ConsoleX;
//using Horseshoe.NET.Db;
//using Horseshoe.NET.SqlDb;

//namespace TestConsole.DbTests
//{
//    class SqlTests : Routine
//    {
//        public override string BannerText => "Sql Tests";

//        public override IEnumerable<MenuObject> Menu => new []
//        {
//            Build
//            (
//                "Not Equals Test",
//                () =>
//                {
//                    Update.Table
//                    (
//                        null,
//                        "Table",
//                        new[]
//                        {
//                            new Column("Column", "column")
//                        },
//                        where: Filter.Column("OtherColumn").NotEquals(15, columnIsNullable: true),
//                        peekStatement: (stmt) => Console.WriteLine(stmt)
//                    );
//                }
//            )
//        };
//    }
//}
