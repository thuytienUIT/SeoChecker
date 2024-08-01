using SeoChecker.Api.Interfaces;
using SeoChecker.Shared.Enums;

namespace SeoChecker.Api.Services
{
    public class SearchServiceFactory : ISearchServiceFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public SearchServiceFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public ISearchService GetSearchService(SearchType searchType)
        {
            using var scope = _serviceProvider.CreateScope();
            return searchType switch
            {
                SearchType.Google => scope.ServiceProvider.GetRequiredService<GoogleSearchService>(),
                SearchType.Bing => scope.ServiceProvider.GetRequiredService<BingSearchService>(),
                _ => throw new ArgumentException("Invalid search type."),
            };
        }
    }
}

