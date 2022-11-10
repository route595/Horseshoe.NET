using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using Horseshoe.NET.Objects;

namespace Horseshoe.NET.SqlDb.Meta
{
    public abstract class DbObjectBase : IEquatable<DbObjectBase>
    {
        public DbObjectBase Parent { get; set; }
        public string Name { get; }
        public SqlObjectType ObjectType { get; }
        public Db Database => _GetDatabase();
        public DbSchema Schema => _GetSchema();
        public DbServer Server => _GetServer();

        public DbObjectBase(string name, SqlObjectType objectType)
        {
            Name = _AcceptName(name) ?? throw new UtilityException("Object name cannot be null or blank");
            ObjectType = objectType;
        }

        private Db _GetDatabase()
        {
            DbObjectBase obj = this;
            while (obj != null)
            {
                if (obj is Db database) return database;
                obj = obj.Parent;
            }
            return null;
        }

        private DbSchema _GetSchema()
        {
            DbObjectBase obj = this;
            while (obj != null)
            {
                if (obj is DbSchema schema) return schema;
                obj = obj.Parent;
            }
            return null;
        }

        private DbServer _GetServer()
        {
            DbObjectBase obj = this;
            while (obj != null)
            {
                if (obj is DbServer server) return server;
                obj = obj.Parent;
            }
            return null;
        }
        
        public override string ToString()
        {
            return "[" + Name + "]";
        }

        public string ToFullyQualifiedString(SqlObjectType? startingWith = null)
        {
            if (Parent == null || ObjectType == startingWith) return ToString();
            return Parent.ToFullyQualifiedString(startingWith: startingWith) + "." + ToString();
        }

        static string _AcceptName(string name)
        {
            var _name = name;
            if (_name.StartsWith("["))
            {
                _name = _name.Substring(1);
                if (_name.EndsWith("]"))
                {
                    _name = _name.Substring(0, _name.Length - 1);
                }
                else
                {
                    throw new UtilityException("Malformed object name: " + name);
                }
            }
            else if (_name.EndsWith("]"))
            {
                throw new UtilityException("Malformed object name: " + name);
            }
            else if (_name.StartsWith("\""))
            {
                _name = _name.Substring(1);
                if (_name.EndsWith("\""))
                {
                    _name = _name.Substring(0, _name.Length - 1);
                }
                else
                {
                    throw new UtilityException("Malformed object name: " + name);
                }
            }
            else if (_name.EndsWith("\""))
            {
                throw new UtilityException("Malformed object name: " + name);
            }
            return Zap.String(_name);
        }

        public override bool Equals(object other)
        {
            if (other is DbObject dbObject)
            {
                return Equals(dbObject);
            }
            return false;
        }

        public bool Equals(DbObjectBase other)
        {
            return other != null &&
                   EqualityComparer<DbObjectBase>.Default.Equals(Parent, other.Parent) &&
                   Name == other.Name &&
                   ObjectType == other.ObjectType;
        }

        public override int GetHashCode()
        {
            var hashCode = 2054261886;
            hashCode = hashCode * -1521134295 + EqualityComparer<DbObjectBase>.Default.GetHashCode(Parent);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Name);
            hashCode = hashCode * -1521134295 + ObjectType.GetHashCode();
            return hashCode;
        }

        public static bool operator ==(DbObjectBase left, DbObjectBase right)
        {
            return EqualityComparer<DbObjectBase>.Default.Equals(left, right);
        }

        public static bool operator !=(DbObjectBase left, DbObjectBase right)
        {
            return !(left == right);
        }
    }
}
