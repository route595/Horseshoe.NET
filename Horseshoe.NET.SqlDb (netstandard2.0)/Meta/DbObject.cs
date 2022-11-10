using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using Horseshoe.NET.Text;

namespace Horseshoe.NET.SqlDb.Meta
{
    public class DbObject : DbObjectBase, IEquatable<DbObject>
    {
        public DbObject(string name, SqlObjectType objectType) : base(name, objectType) { }

        internal static DbObject Parse(string name, string type, string parentType = null)
        {
            if (type == null) throw new UtilityException("type cannot be null");
            var objectType = SqlServerTypes.LookupObjectType(type.ToUpper());
            if (parentType != null)
            {
                var parentObjectType = SqlServerTypes.LookupObjectType(parentType.ToUpper());
                return Parse(name, objectType, parentObjectType: parentObjectType);
            }
            else
            {
                return Parse(name, objectType);
            }
        }

        internal static DbObject Parse(string name, SqlObjectType objectType, SqlObjectType parentObjectType = SqlObjectType.TableOrView)
        {
            var matches = MultiLevelNamePattern.Matches(name)
                .Cast<Match>()
                .Select(m => m.Value.Trim())
                .ToList();

            if (!matches.Any() || matches.Contains(""))
            {
                throw new UtilityException("Invalid " + TextUtil.SpaceOutTitleCase(objectType.ToString()).ToLower() + " name: " + name);
            }

            var oraObj = new DbObject(matches.Last(), objectType);

            if (matches.Count == 1)
            {
                return oraObj;
            }

            switch (objectType)
            {
                case SqlObjectType.Database:
                case SqlObjectType.Schema:
                    return oraObj;
                case SqlObjectType.Table:
                case SqlObjectType.View:
                case SqlObjectType.TableOrView:
                case SqlObjectType.StoredProcedure:
                case SqlObjectType.Function:
                case SqlObjectType.TableValuedFunction:
                case SqlObjectType.InlineTableValuedFunction:
                    if (matches.Count == 2)
                    {
                        oraObj.Parent = new DbSchema(matches[0]);
                        return oraObj;
                    }
                    if (matches.Count == 3)
                    {
                        oraObj.Parent = new DbSchema(matches[1])
                        {
                            Parent = new Db(matches[0])
                        };
                        return oraObj;
                    }
                    break;
                case SqlObjectType.Column:
                    if (matches.Count == 2)
                    {
                        oraObj.Parent = new DbObject(matches[0], parentObjectType);
                        return oraObj;
                    }
                    if (matches.Count == 3)
                    {
                        oraObj.Parent = new DbObject(matches[1], parentObjectType)
                        {
                            Parent = new DbSchema(matches[0])
                        };
                        return oraObj;
                    }
                    if (matches.Count == 4)
                    {
                        oraObj.Parent = new DbObject(matches[2], parentObjectType)
                        {
                            Parent = new DbSchema(matches[1])
                            {
                                Parent = new Db(matches[0])
                            }
                        };
                        return oraObj;
                    }
                    break;
            }
            throw new UtilityException("Invalid " + TextUtil.SpaceOutTitleCase(objectType.ToString()).ToLower() + " name: " + name);
        }

        public bool Equals(DbObject other)
        {
            return this == other;  // see DbObjectBase
        }

        private static Regex MultiLevelNamePattern { get; } = new Regex("(\\[[^\\[\\]]+\\])|(\"[^\"]+\")|([^.]+)");
    }
}
