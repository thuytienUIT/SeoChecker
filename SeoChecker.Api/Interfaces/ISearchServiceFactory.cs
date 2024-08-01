using SeoChecker.Shared.Enums;

namespace SeoChecker.Api.Interfaces
{
	public interface ISearchServiceFactory
	{
        ISearchService GetSearchService(SearchType searchType);
    }
}

