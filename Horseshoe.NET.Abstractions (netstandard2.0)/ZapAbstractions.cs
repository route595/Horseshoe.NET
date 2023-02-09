using System;
using System.Linq;
using Microsoft.Extensions.Primitives;

namespace Horseshoe.NET
{
    /// <summary>
    /// Factory methods for converting objects and strings.  'Zap' means converting blank values to <c>null</c>.
    /// </summary>
    public static class ZapAbstractions
    {
        /// <summary>
        /// Converts <c>obj</c> for nullness (includes <c>DBNull</c>). If <c>obj</c> is a <c>string</c>, 
        /// then <c>string</c> conditions apply.
        /// </summary>
        /// <param name="obj">An object to convert.</param>
        /// <returns>The source <c>object</c>, <c>null</c> or (in the case <c>obj</c> is a <c>string</c>) a zapped <c>string</c>.</returns>
        /// <seealso cref="String"/>
        public static object Object(object obj)
        {
            if (obj == null || obj is DBNull) return null;
            if (obj is string stringValue) return _String(stringValue);
            if (obj is StringValues stringValuesValue)
            {
                switch (stringValuesValue.Count)
                {
                    case 0: return null;
                    case 1: return _String(stringValuesValue.Single());
                    default: return string.Join(", ", stringValuesValue.Select(s => _String(s)));
                };
            }
            return obj;
        }

        /// <summary>
        /// Trims the whitespaces off a <c>string</c>'s edges and if the result is zero-length returns <c>null</c>.
        /// </summary>
        /// <param name="obj">A <c>string</c> or <c>object</c> to evaluate.</param>
        /// <returns>The source <c>object</c> to a <c>string</c> or <c>null</c>.</returns>
        public static string String(object obj)
        {
            if ((obj = Object(obj)) == null)
                return null;

            return _String
            (
                obj is string stringValue
                    ? stringValue
                    : obj.ToString()
            );
        }

        private static string _String(string stringValue)
        {
            var trimmed = stringValue.Trim();
            if (trimmed.Length == 0)
                return null;
            return trimmed;
        }
    }
}
