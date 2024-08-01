using Microsoft.Extensions.Caching.Memory;
using SeoChecker.Api.Interfaces;
using SeoChecker.Api.Utils;
using SeoChecker.Shared.Enums;
using SeoChecker.Shared.Models;

namespace SeoChecker.Api
{
    public class SearchHandler : IHandler
    {
        private readonly IMemoryCache _memoryCache;
        private readonly ISearchServiceFactory _searchServiceFactory;

        public SearchHandler(IMemoryCache memoryCache, ISearchServiceFactory searchServiceFactory)
        {
            _memoryCache = memoryCache;
            _searchServiceFactory = searchServiceFactory;
        }

        public async Task<string> Handle(SearchRequest request, CancellationToken cancellationToken)
        {
            var cacheKey = MemoryCacheHelper.GenerateCacheKey(request);

            if (!_memoryCache.TryGetValue(cacheKey, out string? cachedValue))
            {
                var googleService = _searchServiceFactory.GetSearchService(SearchType.Google);
                var bingService = _searchServiceFactory.GetSearchService(SearchType.Bing);

                var googleResult = await googleService.SearchAsync(request, cancellationToken);
                var bingResult = await bingService.SearchAsync(request, cancellationToken);

                cachedValue = $"Google: {googleResult}, Bing: {bingResult}";

                var cacheEntryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1)
                };

                _memoryCache.Set(cacheKey, cachedValue, cacheEntryOptions);
            }
            
            return cachedValue ?? "Google: 0, Bing: 0";
        }
    }
}

