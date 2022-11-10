using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Horseshoe.NET.Caching
{
    /// <summary>
    /// Application in-memory cache (interface)
    /// </summary>
    public interface IRuntimeCache
    {
        /// <summary>
        /// Gets a cached item with the necessary information to obtain and cache the item if not yet cached, cache expired or forced refresh
        /// </summary>
        /// <typeparam name="E">The type of the cached item</typeparam>
        /// <param name="key">A text key uniquely describing a specific cached item</param>
        /// <param name="refreshFunction">A function that can obtain a fresh, current item</param>
        /// <param name="fromCache">Informational, <c>true</c> if item is from cache rather than <c>refreshFunction</c></param>
        /// <param name="cacheDuration"></param>
        /// <param name="forceRefresh"></param>
        /// <returns></returns>
        E GetFromCache<E>(string key, Func<E> refreshFunction, out bool fromCache, int? cacheDuration = null, bool forceRefresh = false);

        /// <summary>
        /// Gets a cached item with the necessary information to obtain and cache the item if not yet cached, cache expired or forced refresh
        /// </summary>
        /// <typeparam name="E">The type of the cached item</typeparam>
        /// <param name="key">A text key uniquely describing a specific cached item</param>
        /// <param name="refreshFunction">A function that can obtain a fresh, current item</param>
        /// <param name="metadata">Informational, <c>metadate.FromCache</c> = <c>true</c> if item is from cache rather than <c>refreshFunction</c></param>
        /// <param name="cacheDuration"></param>
        /// <param name="forceRefresh"></param>
        /// <returns></returns>
        E GetFromCache<E>(string key, Func<E> refreshFunction, ICacheMetadata metadata = null, int? cacheDuration = null, bool forceRefresh = false);

        /// <summary>
        /// Gets a cached item with the necessary information to obtain and cache the item if not yet cached, cache expired or forced refresh
        /// </summary>
        /// <typeparam name="E">The type of the cached item</typeparam>
        /// <param name="key">A text key uniquely describing a specific cached item</param>
        /// <param name="refreshFunction">A function that can obtain a fresh, current item</param>
        /// <param name="metadata">Informational, <c>metadate.FromCache</c> = <c>true</c> if item is from cache rather than <c>refreshFunction</c></param>
        /// <param name="cacheDuration"></param>
        /// <param name="forceRefresh"></param>
        /// <returns></returns>
        Task<E> GetFromCacheAsync<E>(string key, Func<Task<E>> refreshFunction, ICacheMetadata metadata = null, int? cacheDuration = null, bool forceRefresh = false);

        /// <summary>
        /// Gets a cached list with the necessary information to obtain and cache the list if not yet cached, cache expired or forced refresh
        /// </summary>
        /// <typeparam name="E">The type of the items in the cached list</typeparam>
        /// <param name="key">A text key uniquely describing a specific cached list</param>
        /// <param name="refreshFunction">A function that can obtain a fresh, current list of items</param>
        /// <param name="fromCache">Informational, <c>true</c> if list is from cache rather than <c>refreshFunction</c></param>
        /// <param name="cacheDuration"></param>
        /// <param name="forceRefresh"></param>
        /// <returns></returns>
        IList<E> GetListFromCache<E>(string key, Func<IList<E>> refreshFunction, out bool fromCache, int? cacheDuration = null, bool forceRefresh = false);

        /// <summary>
        /// Gets a cached list with the necessary information to obtain and cache the list if not yet cached, cache expired or forced refresh
        /// </summary>
        /// <typeparam name="E">The type of the items in the cached list</typeparam>
        /// <param name="key">A text key uniquely describing a specific cached list</param>
        /// <param name="refreshFunction">A function that can obtain a fresh, current list of items</param>
        /// <param name="metadata">Informational, <c>metadate.FromCache</c> = <c>true</c> if list is from cache rather than <c>refreshFunction</c></param>
        /// <param name="cacheDuration"></param>
        /// <param name="forceRefresh"></param>
        /// <returns></returns>
        IList<E> GetListFromCache<E>(string key, Func<IList<E>> refreshFunction, ICacheMetadata metadata = null, int? cacheDuration = null, bool forceRefresh = false);

        /// <summary>
        /// Gets a cached list with the necessary information to obtain and cache the list if not yet cached, cache expired or forced refresh
        /// </summary>
        /// <typeparam name="E">The type of the items in the cached list</typeparam>
        /// <param name="key">A text key uniquely describing a specific cached list</param>
        /// <param name="refreshFunction">A function that can obtain a fresh, current list of items</param>
        /// <param name="metadata">Informational, <c>metadate.FromCache</c> = <c>true</c> if list is from cache rather than <c>refreshFunction</c></param>
        /// <param name="cacheDuration">Cache duration in seconds</param>
        /// <param name="forceRefresh"></param>
        /// <returns></returns>
        Task<IList<E>> GetListFromCacheAsync<E>(string key, Func<Task<IList<E>>> refreshFunction, ICacheMetadata metadata = null, int? cacheDuration = null, bool forceRefresh = false);
    }
}
