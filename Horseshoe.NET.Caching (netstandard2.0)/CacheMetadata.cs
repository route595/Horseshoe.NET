using System.Collections.Generic;

namespace Horseshoe.NET.Caching
{
    /// <summary>
    /// Use <c>CacheMetadata</c> to share info between client (request) and assembly (response) throughout a typical interaction.
    /// </summary>
    public class CacheMetadata : ICacheMetadata
    {
        /// <summary>
        /// Reports whether the requested data is from cache (response sourced)
        /// </summary>
        public bool FromCache { get; set; }

        /// <summary>
        /// Passes along a 'force refresh' request from the client (request sourced)
        /// </summary>
        public bool ForceRefresh { get; set; }

        private IDictionary<string, object> info;

        public IDictionary<string, object> GetValues => info;

        /// <summary>
        /// Gets a client-set value, to be client-consumed
        /// </summary>
        /// <typeparam name="E"></typeparam>
        /// <param name="key">A unique key</param>
        /// <param name="value">A previously set value, if applicable</param>
        /// <returns></returns>
        public bool TryGetValue<E>(string key, out E value)
        {
            if (info != null && info.TryGetValue(key, out object _value))
            {
                value = (E)_value;
                return true;
            }
            value = default;
            return false;
        }

        /// <summary>
        /// Sets a value, to be client-consumed, e.g. SetValue("TotalCount", 5990); SetValue("CurrentPageCount", 300);
        /// </summary>
        /// <typeparam name="E"></typeparam>
        /// <param name="key">A unique key</param>
        /// <param name="value">A value to set</param>
        public void SetValue<E>(string key, E value)
        {
            if (info == null)
                info = new Dictionary<string, object>();
            info[key] = value;
        }
    }
}
