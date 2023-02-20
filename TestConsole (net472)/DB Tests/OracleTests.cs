using System;
using System.Collections.Generic;
using System.Data;

using Horseshoe.NET;
using Horseshoe.NET.ConsoleX;
using Horseshoe.NET.OracleDb;
using Horseshoe.NET.OracleDb.Meta;
using Oracle.ManagedDataAccess.Client;

namespace TestConsole.DbTests
{
    class OracleTests : RoutineX
    {
        private static OracleDbConnectionInfo MyConnectInfo { get; } = new OracleDbConnectionInfo
        {
            DataSource = "ORACLESERVER_or_TNSALIAS",
            OracleCredentials = new Credential("userid", "pa$$word")
        };

        private static OracleDbConnectionInfo MyEzConnectInfo { get; } = new OracleDbConnectionInfo
        {
            DataSource = new OraServer("ORACLESERVER", port: 1560, serviceName: "MYSERVICE"),
            OracleCredentials = new Credential("userid", "pa$$word")
        };

        static OracleTests()
        {
            OracleDbSettings.SqlNetAuthenticationServices = "none";
        }

        public override IList<MenuObject> Menu => new[]
        {
            BuildMenuRoutine
            (
                "Oracle Type Test - Statements From Dual",
                () =>
                {
                    var sysdateStatement = "SELECT SYSDATE FROM DUAL";
                    var charStatement = "SELECT 'char statement' FROM DUAL";

                    using (var conn = OracleDbUtil.LaunchConnection(MyConnectInfo))
                    {
                        Console.Write("SYSDATE - DataTable (via adapter): ");
                        using (var cmd = OracleDbUtil.BuildCommand(conn, CommandType.Text, sysdateStatement))
                        {
                            var dataTable = new DataTable();
                            var adapter = new OracleDataAdapter(cmd);
                            adapter.Fill(dataTable);
                            Console.WriteLine(dataTable.Rows[0][0].GetType().Name);
                        }
                        Console.Write("SYSDATE - ExecuteReader(): ");
                        using (var cmd = OracleDbUtil.BuildCommand(conn, CommandType.Text, sysdateStatement))
                        {
                            using (var reader = cmd.ExecuteReader())
                            {
                                reader.Read();
                                Console.WriteLine(reader[0].GetType().Name);
                            }
                        }
                        Console.Write("SYSDATE - ExecuteScalar(): ");
                        using (var cmd = OracleDbUtil.BuildCommand(conn, CommandType.Text, sysdateStatement))
                        {
                            var result = cmd.ExecuteScalar();
                            Console.WriteLine(result.GetType().Name);
                        }
                        Console.Write("Chars - DataTable (via adapter): ");
                        using (var cmd = OracleDbUtil.BuildCommand(conn, CommandType.Text, charStatement))
                        {
                            var dataTable = new DataTable();
                            var adapter = new OracleDataAdapter(cmd);
                            adapter.Fill(dataTable);
                            Console.WriteLine(dataTable.Rows[0][0].GetType().Name);
                        }
                        Console.Write("Chars - ExecuteReader(): ");
                        using (var cmd = OracleDbUtil.BuildCommand(conn, CommandType.Text, charStatement))
                        {
                            using (var reader = cmd.ExecuteReader())
                            {
                                reader.Read();
                                Console.WriteLine(reader[0].GetType().Name);
                            }
                        }
                        Console.Write("Chars - ExecuteScalar(): ");
                        using (var cmd = OracleDbUtil.BuildCommand(conn, CommandType.Text, charStatement))
                        {
                            var result = cmd.ExecuteScalar();
                            Console.WriteLine(result.GetType().Name);
                        }
                    }
                }
            )
        };
    }
}
