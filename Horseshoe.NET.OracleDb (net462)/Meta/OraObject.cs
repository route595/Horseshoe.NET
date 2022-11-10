using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using Horseshoe.NET.Text;

namespace Horseshoe.NET.OracleDb.Meta
{
    public class OraObject : OraObjectBase, IEquatable<OraObject>
    {
        public OraObject(string name, OraObjectType objectType) : base(name, objectType) { }

        internal static OraObject Parse(string name, string type, string parentType = null)
        {
            if (type == null) throw new UtilityException("type cannot be null");
            var objectType = OracleTypes.LookupObjectType(type.ToUpper());
            if (parentType != null)
            {
                var parentObjectType = OracleTypes.LookupObjectType(parentType.ToUpper());
                return Parse(name, objectType, parentObjectType: parentObjectType);
            }
            else
            {
                return Parse(name, objectType);
            }
        }

        internal static OraObject Parse(string name, OraObjectType objectType, OraObjectType parentObjectType = OraObjectType.TableOrView)
        {
            var matches = MultiLevelNamePattern.Matches(name)
                .Cast<Match>()
                .Select(m => m.Value.Trim())
                .ToList();

            if (!matches.Any() || matches.Contains("")) 
            {
                throw new UtilityException("Invalid " + TextUtil.SpaceOutTitleCase(objectType.ToString()).ToLower() + " name: " + name); 
            }

            var oraObj = new OraObject(matches.Last(), objectType);

            if (matches.Count == 1)
            {
                return oraObj;
            }

            switch (objectType)
            {
                case OraObjectType.Schema:
                    return oraObj;
                case OraObjectType.Table:
                case OraObjectType.View:
                case OraObjectType.TableOrView:
                case OraObjectType.MaterializedView:
                case OraObjectType.Package:
                case OraObjectType.PackageBody:
                    if (matches.Count == 2)
                    {
                        oraObj.Parent = new OraSchema(matches[0]);
                        return oraObj;
                    }
                    break;
                case OraObjectType.Function:
                case OraObjectType.Procedure:
                    if (matches.Count == 2)
                    {
                        oraObj.Parent = new OraSchema(matches[0]);
                        return oraObj;
                    }
                    if (matches.Count == 3)
                    {
                        oraObj.Parent = new OraPackage(matches[1])
                        {
                            Parent = new OraSchema(matches[0])
                        };
                        return oraObj;
                    }
                    break;
                case OraObjectType.Column:
                    if (matches.Count == 2)
                    {
                        oraObj.Parent = new OraObject(matches[0], parentObjectType);
                        return oraObj;
                    }
                    if (matches.Count == 3)
                    {
                        oraObj.Parent = new OraObject(matches[1], parentObjectType)
                        {
                            Parent = new OraSchema(matches[0])
                        };
                        return oraObj;
                    }
                    break;
            }
            throw new UtilityException("Invalid " + TextUtil.SpaceOutTitleCase(objectType.ToString()).ToLower() + " name: " + name);
        }

        public bool Equals(OraObject other)
        {
            return this == other;  // see OraObjectBase
        }

        private static Regex MultiLevelNamePattern { get; } = new Regex("(\"[^\"]+\")|([^.]+)");
    }
}
