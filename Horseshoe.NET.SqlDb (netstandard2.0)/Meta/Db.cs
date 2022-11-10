using System;
using System.Collections.Generic;

namespace Horseshoe.NET.SqlDb.Meta
{
    public class Db : DbObjectBase, IEquatable<Db>
    {
        public Db(string name) : base(name, SqlObjectType.Database) { }

        public bool Equals(Db other)
        {
            return this == other;  // see DbObjectBase
        }
    }
}
