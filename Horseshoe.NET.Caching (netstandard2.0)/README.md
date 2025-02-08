![Horseshoe.NET icon](https://raw.githubusercontent.com/route595/Horseshoe.NET/refs/heads/main/assets/images/horseshoe-icon-128x128.png)

# Horseshoe.NET.Caching

DI-ready boilerplate code for in-memory caching

## Code Examples

```c#
// Caches the value for 5 seconds, increments when accessed only if the cached value 
// has expired.  The out param indicates whether the returned value is from cache.
int num = 0;
int value = runtimeCache.GetFromCache<int>(key, () => ++num, out fromCache, cacheDuration: 5);
```
