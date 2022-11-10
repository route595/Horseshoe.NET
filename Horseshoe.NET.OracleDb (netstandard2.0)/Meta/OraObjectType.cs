using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Horseshoe.NET.OracleDb.Meta
{
    public enum OraObjectType
    {
        Server,
        Schema,
        Table,
        View,
        TableOrView,
        MaterializedView,
        Column,
        Procedure,
        Function,
        Package,
        PackageBody
    }
}
