using SeoChecker.Shared.Models;

namespace SeoChecker.Api.Interfaces
{
	public interface ISearchService
	{
        Task<string> SearchAsync(SearchRequest request, CancellationToken cancellationToken);
    }
}

