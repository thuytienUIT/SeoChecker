using SeoChecker.Shared.Models;
using SeoChecker.Api.Interfaces;
using System.Text.RegularExpressions;

namespace SeoChecker.Api.Services
{
    public class GoogleSearchService : ISearchService
    {
        private readonly HttpClient _httpClient;
        private const int NumberOfItemPerPage = 100;
        private const string baseUrl = "https://www.google.com.au";
        private const string DefaultUserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36";

        public GoogleSearchService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.DefaultRequestHeaders.Add("User-Agent", DefaultUserAgent);
        }

        public async Task<string> SearchAsync(SearchRequest request, CancellationToken cancellationToken)
        {
            var response = await SearchGoogleAsync(request.Keywords);

            var positions = ParseResults(response, request.Url);

            return positions.Count > 0 ? string.Join(", ", positions) : "0";
        }

        private async Task<string> SearchGoogleAsync(string keywords)
        {
            var url = $"{baseUrl}/search?q={Uri.EscapeDataString(keywords)}&num={NumberOfItemPerPage}";
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        private List<int> ParseResults(string html, string targetUrl)
        {
            var positions = new List<int>();
            string linkPattern = $@"href=['""](https?://[^'""]*)['""]";
            var regex = new Regex(linkPattern, RegexOptions.IgnoreCase);

            var matches = regex.Matches(html).Take(100);
            int index = 1;

            foreach (Match match in matches)
            {
                if (match.ToString().Contains(targetUrl, StringComparison.OrdinalIgnoreCase))
                {
                    positions.Add(index);
                }

                index++;
            }

            return positions;
        }
    }
}

