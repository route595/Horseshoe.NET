using System;
using System.Collections.Generic;
using System.Runtime.Caching;
using System.Threading.Tasks;

namespace Horseshoe.NET.Caching
{
    /// <summary>
    /// Application in-memory cache boilerplate code based on legacy System.Runtime.Caching with 120 second default cache duration (configurable)
    /// </summary>
    public class RuntimeCache : IRuntimeCache
    {
        public const int DEFAULT_CACHE_DURATION_SECONDS = 120;
        private readonly MemoryCache memoryCache;

        public RuntimeCache()
        {
            memoryCache = MemoryCache.Default;
        }

        public E GetFromCache<E>(string key, Func<E> refreshFunction, out bool fromCache, int? cacheDuration = null, bool forceRefresh = false)
        {
            object cachedObj = forceRefresh
                ? null
                : memoryCache.Get(key);

            if (cachedObj != null)
            {
                fromCache = true;
                return (E)cachedObj;
            }

            E freshObj = refreshFunction.Invoke();
            var expires = DateTimeOffset.UtcNow.AddSeconds(cacheDuration ?? CacheSettings.DefaultCacheDurationInSeconds);
            memoryCache.Set(key, freshObj, expires);
            fromCache = false;
            return freshObj;
        }

        public E GetFromCache<E>(string key, Func<E> refreshFunction, ICacheMetadata metadata = null, int? cacheDuration = null, bool forceRefresh = false)
        {
            metadata = metadata ?? new CacheMetadata();

            object cachedObj = forceRefresh || metadata.ForceRefresh
                ? null
                : memoryCache.Get(key);

            if (cachedObj != null)
            {
                metadata.FromCache = true;
                return (E)cachedObj;
            }

            E freshObj = refreshFunction.Invoke();
            var expires = DateTimeOffset.UtcNow.AddSeconds(cacheDuration ?? CacheSettings.DefaultCacheDurationInSeconds);
            memoryCache.Set(key, freshObj, expires);
            if (metadata != null) 
                metadata.FromCache = false;
            return freshObj;
        }

        public async Task<E> GetFromCacheAsync<E>(string key, Func<Task<E>> refreshFunction, ICacheMetadata metadata = null, int? cacheDuration = null, bool forceRefresh = false)
        {
            metadata = metadata ?? new CacheMetadata();

            object cachedObj = forceRefresh || metadata.ForceRefresh
                ? null
                : memoryCache.Get(key);

            if (cachedObj != null)
            {
                metadata.FromCache = true;
                return (E)cachedObj;
            }

            E freshObj = await refreshFunction.Invoke();
            var expires = DateTimeOffset.UtcNow.AddSeconds(cacheDuration ?? CacheSettings.DefaultCacheDurationInSeconds);
            memoryCache.Set(key, freshObj, expires);
            if (metadata != null)
                metadata.FromCache = false;
            return freshObj;
        }

        public IList<E> GetListFromCache<E>(string key, Func<IList<E>> refreshFunction, out bool fromCache, int? cacheDuration = null, bool forceRefresh = false)
        {
            object cachedObj = forceRefresh
                ? null
                : memoryCache.Get(key);

            if (cachedObj != null)
            {
                fromCache = true;
                return (IList<E>)cachedObj;
            }

            IList<E> freshObj = refreshFunction.Invoke();
            var expires = DateTimeOffset.UtcNow.AddSeconds(cacheDuration ?? CacheSettings.DefaultCacheDurationInSeconds);
            memoryCache.Set(key, freshObj, expires);
            fromCache = false;
            return freshObj;
        }

        public IList<E> GetListFromCache<E>(string key, Func<IList<E>> refreshFunction, ICacheMetadata metadata = null, int? cacheDuration = null, bool forceRefresh = false)
        {
            metadata = metadata ?? new CacheMetadata();

            object cachedObj = forceRefresh || metadata.ForceRefresh
                ? null
                : memoryCache.Get(key);

            if (cachedObj != null)
            {
                metadata.FromCache = true;
                return (IList<E>)cachedObj;
            }

            IList<E> freshObj = refreshFunction.Invoke();
            var expires = DateTimeOffset.UtcNow.AddSeconds(cacheDuration ?? CacheSettings.DefaultCacheDurationInSeconds);
            memoryCache.Set(key, freshObj, expires);
            if (metadata != null) 
                metadata.FromCache = false;
            return freshObj;
        }

        public async Task<IList<E>> GetListFromCacheAsync<E>(string key, Func<Task<IList<E>>> refreshFunction, ICacheMetadata metadata = null, int? cacheDuration = null, bool forceRefresh = false)
        {
            metadata = metadata ?? new CacheMetadata();

            object cachedObj = forceRefresh || metadata.ForceRefresh
                ? null
                : memoryCache.Get(key);

            if (cachedObj != null)
            {
                metadata.FromCache = true;
                return (IList<E>)cachedObj;
            }

            IList<E> freshObj = await refreshFunction.Invoke();
            var expires = DateTimeOffset.UtcNow.AddSeconds(cacheDuration ?? CacheSettings.DefaultCacheDurationInSeconds);
            memoryCache.Set(key, freshObj, expires);
            if (metadata != null)
                metadata.FromCache = false;
            return freshObj;
        }
    }
}
