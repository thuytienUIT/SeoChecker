using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using SeoChecker.Api.Interfaces;
using SeoChecker.Api.Utils;
using SeoChecker.Shared.Enums;
using SeoChecker.Shared.Models;

namespace SeoChecker.Api.Services
{
    public class CacheBackgroundService : BackgroundService
    {
        private readonly IMemoryCache _memoryCache;
        private readonly ILogger<CacheBackgroundService> _logger;
        private static readonly TimeSpan CacheDuration = TimeSpan.FromHours(1);
        private readonly DefaultSearchRequest _defaultSearchRequest;
        private readonly ISearchServiceFactory _searchServiceFactory;

        public CacheBackgroundService(IMemoryCache memoryCache,
            ILogger<CacheBackgroundService> logger,
            IOptions<DefaultSearchRequest> defaultSearchRequest,
            ISearchServiceFactory searchServiceFactory)
        {
            _memoryCache = memoryCache;
            _logger = logger;
            _defaultSearchRequest = defaultSearchRequest.Value;
            _searchServiceFactory = searchServiceFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    _logger.LogInformation("Loading data into cache.");

                    var searchRequestDefault = new SearchRequest(_defaultSearchRequest.Keyword, _defaultSearchRequest.Url);
                    var cacheKey = MemoryCacheHelper.GenerateCacheKey(searchRequestDefault);

                    var data = await LoadDataAsync(searchRequestDefault, stoppingToken);

                    _memoryCache.Set(cacheKey, data, CacheDuration);

                    _logger.LogInformation("Data loaded into cache successfully.");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while loading data into cache.");
                }

                await Task.Delay(CacheDuration, stoppingToken);
            }
        }

        private async Task<string> LoadDataAsync(SearchRequest searchRequest,CancellationToken cancellationToken)
        {
            var googleService = _searchServiceFactory.GetSearchService(SearchType.Google);
            var bingService = _searchServiceFactory.GetSearchService(SearchType.Bing);

            var googleResult = await googleService.SearchAsync(searchRequest, cancellationToken);
            var bingResult = await bingService.SearchAsync(searchRequest, cancellationToken);

            var cacheValue = $"Google: {googleResult}, Bing: {bingResult}";

            return cacheValue;
        }
    }
}
