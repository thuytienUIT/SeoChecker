using System.Text.RegularExpressions;
using SeoChecker.Api.Interfaces;
using SeoChecker.Shared.Models;

namespace SeoChecker.Api.Services
{
    public class BingSearchService : ISearchService
    {
        private readonly HttpClient _httpClient;
        private const string baseUrl = "https://www.bing.com";
        private const int NumberOfItemPerPage = 10;
        private const int TotalPage = 10;

        public BingSearchService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> SearchAsync(SearchRequest request, CancellationToken cancellationToken)
        {
            var response = string.Empty;
            for (int i = 0; i < TotalPage; i++)
            {
                var htmlContent = await SearchBingAsync(request.Keywords, i);
                response += htmlContent;
            }

            var positions = ParseResults(response, request.Url);

            return positions.Count > 0 ? string.Join(", ", positions) : "0";
        }

        private async Task<string> SearchBingAsync(string keywords, int pageNumber)
        {
            var url = $"{baseUrl}/search?q={Uri.EscapeDataString(keywords)}&first={pageNumber * NumberOfItemPerPage + 1}";
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }

        private List<int> ParseResults(string html, string targetUrl)
        {
            var positions = new List<int>();
            var urlPattern = @"<a\s+href=""(https?://[^""]*)""";
            var matchPattern = new Regex(urlPattern, RegexOptions.IgnoreCase);

            var matches = matchPattern.Matches(html).Take(100);
            int index = 1;

            foreach (Match match in matches)
            {
                var link = match.Groups[1].Value;

                if (link.Contains(targetUrl, StringComparison.OrdinalIgnoreCase))
                {
                    positions.Add(index);
                }

                index++;
            }

            return positions;
        }
    }
}
