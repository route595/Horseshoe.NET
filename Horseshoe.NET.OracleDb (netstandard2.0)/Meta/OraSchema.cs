using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Horseshoe.NET.OracleDb.Meta
{
    public class OraSchema : OraObjectBase, IEquatable<OraSchema>
    {
        public OraSchema(string name) : base(name, OraObjectType.Schema) { }

        public bool Equals(OraSchema other)
        {
            return this == other;  // see OraObjectBase
        }
    }
}
