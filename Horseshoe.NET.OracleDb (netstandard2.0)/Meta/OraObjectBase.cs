using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Horseshoe.NET.Objects;

namespace Horseshoe.NET.OracleDb.Meta
{
    public abstract class OraObjectBase : IEquatable<OraObjectBase>
    {
        public OraObjectBase Parent { get; set; }
        public string Name { get; }
        public OraObjectType ObjectType { get; }
        public OraSchema Schema => _GetSchema();
        public OraServer Server => _GetServer();

        public OraObjectBase(string name, OraObjectType objectType)
        {
            Name = _AcceptName(name) ?? throw new UtilityException("Object name cannot be null or blank");
            ObjectType = objectType;
        }

        public override string ToString()
        {
            return _PrepareName(Name);
        }

        public string ToFullyQualifiedString(OraObjectType? startingWith = null)
        {
            if (Parent == null || ObjectType == startingWith) return ToString();
            return Parent.ToFullyQualifiedString(startingWith: startingWith) + "." + ToString();
        }

        private OraSchema _GetSchema()
        {
            OraObjectBase obj = this;
            while (obj != null)
            {
                if (obj is OraSchema schema) return schema;
                obj = obj.Parent;
            }
            return null;
        }

        public OraObjectBase SetSchemaByName(string name)
        {
            var obj = this;
            while (obj.Parent != null)
            {
                obj = obj.Parent;
            }
            obj.Parent = new OraSchema(name);
            return this;
        }

        private OraServer _GetServer()
        {
            OraObjectBase obj = this;
            while (obj != null)
            {
                if (obj is OraServer server) return server;
                obj = obj.Parent;
            }
            return null;
        }

        static string _AcceptName(string name)
        {
            var _name = name;
            if (_name.StartsWith("\""))
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

        static Regex NoQuotePattern { get; } = new Regex(@"^[A-Z0-9_]+$", RegexOptions.IgnoreCase);

        static string _PrepareName(string name)
        {
            if (NoQuotePattern.IsMatch(name)) return name;
            return "\"" + name + "\"";
        }

        public override bool Equals(object other)
        {
            if (other is OraObject dbObject)
            {
                return Equals(dbObject);
            }
            return false;
        }

        public bool Equals(OraObjectBase other)
        {
            return other != null &&
                   EqualityComparer<OraObjectBase>.Default.Equals(Parent, other.Parent) &&
                   Name == other.Name &&
                   ObjectType == other.ObjectType;
        }

        public override int GetHashCode()
        {
            var hashCode = 2054261886;
            hashCode = hashCode * -1521134295 + EqualityComparer<OraObjectBase>.Default.GetHashCode(Parent);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Name);
            hashCode = hashCode * -1521134295 + ObjectType.GetHashCode();
            return hashCode;
        }

        public static bool operator ==(OraObjectBase left, OraObjectBase right)
        {
            return EqualityComparer<OraObjectBase>.Default.Equals(left, right);
        }

        public static bool operator !=(OraObjectBase left, OraObjectBase right)
        {
            return !(left == right);
        }
    }
}
