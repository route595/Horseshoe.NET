using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

using Horseshoe.NET;
using Horseshoe.NET.ConsoleX;
using Horseshoe.NET.Db;
using Horseshoe.NET.OracleDb;
using Horseshoe.NET.OracleDb.Meta;
using Oracle.ManagedDataAccess.Client;

namespace TestConsole.DbTests
{
    class OracleTests : RoutineX
    {
        private static OracleDbConnectionInfo MyConnectionInfo { get; } = new OracleDbConnectionInfo
        {
            Server = new OraServer("SERVER", port: 1761, serviceName: "SERVICE"),
            OracleCredentials = new Credential("USER", "PASSWORD")
        };

        private static OracleDbConnectionInfo MyOmsConnectionInfo { get; } = new OracleDbConnectionInfo
        {
            Server = new OraServer("DBSRV729", port: 1761, serviceName: "OMSDAQA"),
            OracleCredentials = new Credential("DAU00056", "Cq4SPh0G0hqZbp")
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
                    using (var conn = OracleDbUtil.LaunchConnection(MyConnectionInfo))
                    {
                        Console.Write("SYSDATE - DataTable (via adapter): ");
                        using (var cmd = OracleDbUtil.BuildCommand(conn, CommandType.Text, sysdateStatement))
                        {
                            var dataTable = new DataTable();
                            var adapter = new OracleDataAdapter(cmd);
                            adapter.Fill(dataTable);
                            Console.WriteLine(dataTable.Rows[0][0] + " - " + dataTable.Rows[0][0].GetType().Name);
                        }
                        Console.Write("SYSDATE - ExecuteReader(): ");
                        using (var cmd = OracleDbUtil.BuildCommand(conn, CommandType.Text, sysdateStatement))
                        {
                            using (var reader = cmd.ExecuteReader())
                            {
                                reader.Read();
                                Console.WriteLine(reader[0] + " - " + reader[0].GetType().Name);
                            }
                        }
                        Console.Write("SYSDATE - ExecuteScalar(): ");
                        using (var cmd = OracleDbUtil.BuildCommand(conn, CommandType.Text, sysdateStatement))
                        {
                            var result = cmd.ExecuteScalar();
                            Console.WriteLine(result + " - " + result.GetType().Name);
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
                                Console.WriteLine(reader[0] + " - " + reader[0].GetType().Name);
                            }
                        }
                        Console.Write("Chars - ExecuteScalar(): ");
                        using (var cmd = OracleDbUtil.BuildCommand(conn, CommandType.Text, charStatement))
                        {
                            var result = cmd.ExecuteScalar();
                            Console.WriteLine(result + " - " + result.GetType().Name);
                        }
                    }
                }
            ),
            BuildMenuRoutine
            (
                "Oracle Type Test - Stored Procs",
                () =>
                {
                    var inParamProc = "CARL_FETCHES.FETCH_LKUP_CODES";
                    var outParamProc = "CARL_FETCHES.FETCH_LKUP_TYPES2";
                    using (var conn = OracleDbUtil.LaunchConnection(MyConnectionInfo))
                    {
                        Console.WriteLine(inParamProc + ":");
                        Console.Write("  DataTable (via adapter): ");
                        using (var cmd = OracleDbUtil.BuildCommand(conn, CommandType.StoredProcedure, inParamProc))
                        {
                            cmd.Parameters.Add(new OracleParameter(null, "MAINTYPE"));
                            var dataTable = new DataTable();
                            var adapter = new OracleDataAdapter(cmd);
                            adapter.Fill(dataTable);
                            Console.WriteLine(dataTable.Rows[0][2] + " is " + dataTable.Rows[0][2].GetType().Name);
                        }
                        Console.Write("  ExecuteReader(): ");
                        using (var cmd = OracleDbUtil.BuildCommand(conn, CommandType.StoredProcedure, inParamProc))
                        {
                            cmd.Parameters.Add(new OracleParameter(null, "MAINTYPE"));
                            using (var reader = cmd.ExecuteReader())
                            {
                                reader.Read();
                                Console.WriteLine(reader[2] + " is " + reader[2].GetType().Name);
                            }
                        }
                        Console.Write("  ExecuteScalar(): ");
                        using (var cmd = OracleDbUtil.BuildCommand(conn, CommandType.StoredProcedure, inParamProc))
                        {
                            cmd.Parameters.Add(new OracleParameter(null, "MAINTYPE"));
                            var result = cmd.ExecuteScalar();
                            Console.WriteLine(result + " is " + result.GetType().Name);
                        }
                        Console.WriteLine("  ExecuteScalar() - Out Param: ");
                        using (var cmd = OracleDbUtil.BuildCommand(conn, CommandType.StoredProcedure, outParamProc))
                        {
                            cmd.Parameters.Add(new OracleParameter("message", OracleDbType.Varchar2, 100, null, ParameterDirection.Output));
                            var result = cmd.ExecuteScalar();
                            Console.WriteLine("    " + result + " is " + result.GetType().Name);
                            result = cmd.Parameters[0].Value;
                            Console.WriteLine("    out message: \"" + result + "\" is " + result.GetType().Name);
                            var dbCapture = new DbCapture { OutputParameters = new[] { cmd.Parameters[0] } };
                            var oraCapture = new OracleDbCapture { OutputParameters = new[] { cmd.Parameters[0] } };
                            result = oraCapture.GetString("message");
                            Console.WriteLine("    out message [collector.get()]: \"" + result + "\" is " + result.GetType().Name);
                        }
                    }
                }
            ),
            BuildMenuRoutine
            (
                "OracleDb Tests #1",
                () =>
                {
                    var inParamProc = "CARL_FETCHES.FETCH_LKUP_CODES";
                    var outParamProc = "CARL_FETCHES.FETCH_LKUP_TYPES2";
                    var dbCapture = new OracleDbCapture();
                    var objs = Query.Procedure.AsObjects
                    (
                        inParamProc,
                        parameters: new[]
                        {
                            OracleDbUtil.BuildInParam(value: "MAINTYPE")
                        },
                        connectionInfo: MyConnectionInfo,
                        dbCapture: dbCapture
                    );
                    Console.WriteLine("Test #1 Results:");
                    Console.WriteLine("  " + dbCapture.DataColumns[2].ColumnName);
                    Console.WriteLine("  " + new string('-', dbCapture.DataColumns[2].ColumnName.Length));
                    Console.WriteLine("  " + objs.First()[2]);
                    Console.WriteLine("  " + "... +" + (objs.Count() - 1) + " more");
                    Console.WriteLine();
                    var list = Query.Procedure.AsCollection<string>
                    (
                        outParamProc,
                        parameters: new[]
                        {
                            OracleDbUtil.BuildVarchar2OutParam(name: "message")
                        },
                        connectionInfo: MyConnectionInfo,
                        dbCapture: dbCapture,
                        rowParser: RowParser.ScalarString
                    );
                    Console.WriteLine("Test #2 Results:");
                    Console.WriteLine("  Scalar");
                    Console.WriteLine("  ------");
                    Console.WriteLine("  " + list.First());
                    Console.WriteLine("  " + "... +" + (list.Count() - 1) + " more");
                    Console.WriteLine("Output message: " + dbCapture.GetString("message"));
                    Console.WriteLine();
                }
            ),
            BuildMenuRoutine
            (
                "OMS OracleDb Tests",
                () =>
                {
                    var dbCapture = new OracleDbCapture();
                    var objsArrays = Query.SQL.AsObjects
                    (
                        @"SELECT user_name, full_name 
                        FROM lgenms.CES_USER
                        WHERE FULL_NAME LIKE '%DCC%'
                        AND ACTIVE = 'Y'",
                        connectionInfo: MyOmsConnectionInfo,
                        dbCapture: dbCapture
                    );
                    Console.WriteLine("Columns:");
                    Console.WriteLine(string.Join(Environment.NewLine, dbCapture.DataColumns.Select(s => "  " + s)));
                    Console.WriteLine();
                    Console.WriteLine("Data:");
                    Console.WriteLine(string.Join(Environment.NewLine, objsArrays.Select(objs => "  " + string.Join(", ", objs))));
                    Console.WriteLine();
                }
            )
        };
    }
}
