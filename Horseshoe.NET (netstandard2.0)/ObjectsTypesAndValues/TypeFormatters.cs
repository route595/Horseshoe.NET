using System;
using System.Collections.Generic;

using Horseshoe.NET.Collections;

namespace Horseshoe.NET.ObjectsTypesAndValues
{
    /// <summary>
    /// A cache of custom, user-supplied formatters for displaying values
    /// </summary>
    public static class TypeFormatters
    {
        private static IDictionary<Type, Func<object, string>> _formatters;

        /// <summary>
        /// Add or replace a custom formatter for a specific runtime type to the cache
        /// </summary>
        /// <param name="type">A type</param>
        /// <param name="formatter">A custome formatter</param>
        public static void AddFormatter(Type type, Func<object, string> formatter)
        {
            if (_formatters == null)
                _formatters = new Dictionary<Type, Func<object, string>>();
            _formatters[type] = formatter;
        }

        /// <summary>
        /// Gets a formatter from the user-supplied cache
        /// </summary>
        /// <param name="type">A type</param>
        /// <returns>A formatter or <c>null</c></returns>
        public static Func<object, string> GetFormatter(Type type)
        {
            if (_formatters == null)
                return null;

            return DictionaryUtil.TryGetValue(_formatters, type, out Func<object, string> formatter)
                ? formatter
                : null;
        }
    }
}
