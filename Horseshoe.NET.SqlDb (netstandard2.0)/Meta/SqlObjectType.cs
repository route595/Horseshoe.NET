using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Horseshoe.NET.SqlDb.Meta
{
    public enum SqlObjectType
    {
        Server,
        Database,
        Schema,
        Table,
        TableOrView,
        View,
        Column,
        StoredProcedure,
        Function,
        TableValuedFunction,
        InlineTableValuedFunction
    }
}
