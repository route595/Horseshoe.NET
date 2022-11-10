using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Horseshoe.NET.OracleDb.Meta
{
    public class OraPackage : OraObjectBase, IEquatable<OraPackage>
    {
        public OraPackage(string name) : base(name, OraObjectType.Package) { }

        public bool Equals(OraPackage other)
        {
            return this == other;  // see OraObjectBase
        }
    }
}
