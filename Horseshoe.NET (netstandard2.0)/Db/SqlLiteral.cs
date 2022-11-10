using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Horseshoe.NET.Db
{
    public class SqlLiteral
    {
        public string Expression { get; }

        public SqlLiteral(string expression)
        {
            Expression = expression ?? throw new UtilityException("Cannot build a SQL literal from null");
        }

        public override string ToString()
        {
            return Expression;
        }

        public static SqlLiteral CurrentDate(DbPlatform platform)
        {
            switch (platform)
            {
                case DbPlatform.SqlServer:
                    return new SqlLiteral("GETDATE()");
                case DbPlatform.Oracle:
                    return new SqlLiteral("SYSDATE");
                default:
                    throw new NotImplementedException("Platform '" + platform + "' is not a valid choice for CurrentDate as of " + Lib.DisplayName);
            }
        }

        public static SqlLiteral NewGuid(DbPlatform platform)
        {
            switch (platform)
            {
                case DbPlatform.SqlServer:
                    return new SqlLiteral("NEWID()");
                case DbPlatform.Oracle:
                    return new SqlLiteral("SYSGUID()");
                default:
                    throw new NotImplementedException("Platform '" + platform + "' is not a valid choice for NewGuid as of " + Lib.DisplayName);
            }
        }

        public static SqlLiteral Identity(DbPlatform platform)
        {
            switch (platform)
            {
                case DbPlatform.SqlServer:
                    return new SqlLiteral("CONVERT(int, SCOPE_IDENTITY())");
                case DbPlatform.Oracle:
                    return new SqlLiteral("LAST_INSERT_ID()");
                case DbPlatform.Neutral:
                    throw new NotImplementedException("Platform 'Neutral' is not a valid choice for Identity, use another e.g. 'SqlServer' or 'Oracle'.");
                default:
                    throw new NotImplementedException("Platform '" + platform + "' is not a valid choice for Identity as of " + Lib.DisplayName);
            }
        }
    }
}
