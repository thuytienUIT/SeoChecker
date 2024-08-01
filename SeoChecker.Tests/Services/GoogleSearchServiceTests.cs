using System;
using System.Net;
using Moq;
using Moq.Protected;
using SeoChecker.Api.Services;
using SeoChecker.Shared.Models;

namespace SeoChecker.Test.Services
{
	public class GoogleSearchServiceTests
	{
        private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock;
        private readonly HttpClient _httpClient;
        private readonly GoogleSearchService _googleSearchService;

        public GoogleSearchServiceTests()
		{
            _httpMessageHandlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            _httpClient = new HttpClient(_httpMessageHandlerMock.Object)
            {
                BaseAddress = new Uri("https://www.google.com/")
            };
            _googleSearchService = new GoogleSearchService(_httpClient);
        }

        private void SetupHttpResponse(string url, string responseContent)
        {
            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get &&
                        req.RequestUri != null && req.RequestUri.ToString() == url),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(responseContent)
                });
        }

        [Fact]
        public async Task SearchAsync_ValidResponse_ReturnsPositions()
        {
            // Arrange
            var request = new SearchRequest("e-settlements", "https://www.sympli.com.au");
            string htmlContent = "<a href=\"https://www.sympli.com.au/blog/the-evolution-of-e-settlements-the-best-is-yet-to-come/\">Link</a>";
            SetupHttpResponse("https://www.google.com.au/search?q=e-settlements&num=100", htmlContent);

            // Act
            var result = await _googleSearchService.SearchAsync(request, CancellationToken.None);

            // Assert
            Assert.Equal("1", result);
        }

        [Fact]
        public async Task SearchAsync_NoMatchingUrl_ReturnsZero()
        {
            // Arrange
            var request = new SearchRequest("e-settlements", "https://www.sympli.com.au");
            string htmlContent = "<a href=\"https://someotherurl.com\">Link</a>";
            SetupHttpResponse("https://www.google.com.au/search?q=e-settlements&num=100", htmlContent);

            // Act
            var result = await _googleSearchService.SearchAsync(request, CancellationToken.None);

            // Assert
            Assert.Equal("0", result);
        }
    }
}

