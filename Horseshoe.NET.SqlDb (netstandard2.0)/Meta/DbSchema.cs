using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Horseshoe.NET.SqlDb.Meta
{
    public class DbSchema : DbObjectBase
    {
        public DbSchema(string name) : base(name, SqlObjectType.Schema) { }
    }
}
