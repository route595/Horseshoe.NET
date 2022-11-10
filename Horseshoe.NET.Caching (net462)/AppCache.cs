using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;

namespace Horseshoe.NET.Caching
{
    /// <summary>
    /// Application in-memory cache boilerplate code based on Microsoft.Extensions.Caching.Memory with 120 second default cache duration (configurable)
    /// </summary>
    public class AppCache : IAppCache
    {
        public const int DEFAULT_CACHE_DURATION_SECONDS = 120;
        private readonly IMemoryCache memoryCache;

        public static AppCache Launch()
        {
            return new AppCache(new MemoryCache(new MemoryCacheOptions { }));
        }

        public AppCache(IMemoryCache memoryCache)
        {
            this.memoryCache = memoryCache;
        }

        public E GetFromCache<E>(object key, Func<E> refreshFunction, out bool fromCache, int? cacheDuration = null, bool forceRefresh = false)
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
            var expires = TimeSpan.FromSeconds(cacheDuration ?? CacheSettings.DefaultCacheDuration ?? DEFAULT_CACHE_DURATION_SECONDS);
            memoryCache.Set(key, freshObj, expires);
            fromCache = false;
            return freshObj;
        }

        public E GetFromCache<E>(object key, Func<E> refreshFunction, ICacheMetadata metadata = null, int? cacheDuration = null, bool forceRefresh = false)
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
            var expires = TimeSpan.FromSeconds(cacheDuration ?? CacheSettings.DefaultCacheDuration ?? DEFAULT_CACHE_DURATION_SECONDS);
            memoryCache.Set(key, freshObj, expires);
            if (metadata != null)
                metadata.FromCache = false;
            return freshObj;
        }

        public async Task<E> GetFromCacheAsync<E>(object key, Func<Task<E>> refreshFunction, ICacheMetadata metadata = null, int? cacheDuration = null, bool forceRefresh = false)
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
            var expires = TimeSpan.FromSeconds(cacheDuration ?? CacheSettings.DefaultCacheDuration ?? DEFAULT_CACHE_DURATION_SECONDS);
            memoryCache.Set(key, freshObj, expires);
            if (metadata != null)
                metadata.FromCache = false;
            return freshObj;
        }

        public IList<E> GetListFromCache<E>(object key, Func<IList<E>> refreshFunction, out bool fromCache, int? cacheDuration = null, bool forceRefresh = false)
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
            var expires = TimeSpan.FromSeconds(cacheDuration ?? CacheSettings.DefaultCacheDuration ?? DEFAULT_CACHE_DURATION_SECONDS);
            memoryCache.Set(key, freshObj, expires);
            fromCache = false;
            return freshObj;
        }

        public IList<E> GetListFromCache<E>(object key, Func<IList<E>> refreshFunction, ICacheMetadata metadata = null, int? cacheDuration = null, bool forceRefresh = false)
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
            var expires = TimeSpan.FromSeconds(cacheDuration ?? CacheSettings.DefaultCacheDuration ?? DEFAULT_CACHE_DURATION_SECONDS);
            memoryCache.Set(key, freshObj, expires);
            if (metadata != null)
                metadata.FromCache = false;
            return freshObj;
        }

        public async Task<IList<E>> GetListFromCacheAsync<E>(object key, Func<Task<IList<E>>> refreshFunction, ICacheMetadata metadata = null, int? cacheDuration = null, bool forceRefresh = false)
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
            var expires = TimeSpan.FromSeconds(cacheDuration ?? CacheSettings.DefaultCacheDuration ?? DEFAULT_CACHE_DURATION_SECONDS);
            memoryCache.Set(key, freshObj, expires);
            if (metadata != null)
                metadata.FromCache = false;
            return freshObj;
        }
    }
}
