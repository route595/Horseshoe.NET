namespace Horseshoe.NET.Caching
{
    public interface ICacheMetadata
    {
        /// <summary>
        /// Reports whether the requested data is from cache (response sourced)
        /// </summary>
        bool FromCache { get; set; }

        /// <summary>
        /// Passes along a 'force refresh' request from the client (request sourced)
        /// </summary>
        bool ForceRefresh { get; }
    }
}
