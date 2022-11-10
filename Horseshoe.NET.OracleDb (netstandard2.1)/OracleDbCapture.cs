using System;
using System.Linq;

using Horseshoe.NET.Db;

namespace Horseshoe.NET.OracleDb
{
    public class OracleDbCapture : DbCapture
    {
        public override object? Get(string name, bool suppressErrors = false)
        {
            try
            {
                return OracleDbUtil.NormalizeDbValue
                (
                    OutputParameters
                        .Single(p => string.Equals(name, p.ParameterName, StringComparison.OrdinalIgnoreCase))?
                        .Value
                );
            }
            catch
            {
                if (suppressErrors)
                    return null;
                throw;
            }
        }
    }
}
