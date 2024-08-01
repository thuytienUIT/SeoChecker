using SeoChecker.Shared.Models;

namespace SeoChecker.Api.Utils
{
	public static class MemoryCacheHelper
	{
        public static string GenerateCacheKey(SearchRequest searchRequest)
        {
            var cacheKey = $"{searchRequest.Keywords}_{searchRequest.Url}";
            return cacheKey;
        }
    }
}

