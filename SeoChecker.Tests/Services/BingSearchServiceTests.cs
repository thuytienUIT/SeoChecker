using Moq;
using SeoChecker.Api.Services;
using SeoChecker.Shared.Models;
using System.Net;
using Moq.Protected;

namespace SeoChecker.Test.Services
{
	public class BingSearchServiceTests
	{
        private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock;
        private readonly HttpClient _httpClient;
        private readonly BingSearchService _bingSearchService;

        public BingSearchServiceTests()
        {
            _httpMessageHandlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            _httpClient = new HttpClient(_httpMessageHandlerMock.Object)
            {
                BaseAddress = new Uri("https://www.bing.com/")
            };
            _bingSearchService = new BingSearchService(_httpClient);
        }

        [Fact]
        public async Task SearchAsync_ValidRequest_ReturnsPositions()
        {
            // Arrange
            var keywords = "e-settlements";
            var url = "https://www.sympli.com.au";
            var htmlContents = new List<string>
            {
                "<html><ol id=\"b_results\"><li><a href=\"https://www.sympli.com.au/page1\">Link</a></li></ol></html>", // Page 0
                "<html><ol id=\"b_results\"><li><a href=\"https://www.sympli.com.au/page2\">Link</a></li></ol></html>", // Page 1
                "<html><ol id=\"b_results\"><li><a href=\"https://www.sympli.com.au/page3\">Link</a></li></ol></html>", // Page 2
                "<html><ol id=\"b_results\"><li><a href=\"https://www.sympli.com.au/page4\">Link</a></li></ol></html>", // Page 3
                "<html><ol id=\"b_results\"><li><a href=\"https://www.sympli.com.au/page5\">Link</a></li></ol></html>", // Page 4
                "<html><ol id=\"b_results\"><li><a href=\"https://www.sympli.com.au/page6\">Link</a></li></ol></html>", // Page 5
                "<html><ol id=\"b_results\"><li><a href=\"https://www.sympli.com.au/page7\">Link</a></li></ol></html>", // Page 6
                "<html><ol id=\"b_results\"><li><a href=\"https://www.sympli.com.au/page8\">Link</a></li></ol></html>", // Page 7
                "<html><ol id=\"b_results\"><li><a href=\"https://www.sympli.com.au/page9\">Link</a></li></ol></html>", // Page 8
                "<html><ol id=\"b_results\"><li><a href=\"https://www.sympli.com.au/page10\">Link</a></li></ol></html>", // Page 9
                "<html><ol id=\"b_results\"><li><a href=\"https://www.sympli.com.au/page11\">Link</a></li></ol></html>"  // Page 10
            };

            _httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync((HttpRequestMessage request, CancellationToken cancellationToken) =>
                {
                    var requestUri = request.RequestUri;
                    if (requestUri == null)
                    {
                        return new HttpResponseMessage
                        {
                            StatusCode = HttpStatusCode.BadRequest,
                            Content = new StringContent("Invalid RequestUri")
                        };
                    }

                    var pageNumberParam = requestUri.Query.Split('&').FirstOrDefault(q => q.StartsWith("first="));
                    if (pageNumberParam == null)
                    {
                        return new HttpResponseMessage
                        {
                            StatusCode = HttpStatusCode.BadRequest,
                            Content = new StringContent("Invalid query parameter")
                        };
                    }

                    var pageIndex = int.Parse(pageNumberParam.Split('=')[1]) / 10;

                    return new HttpResponseMessage
                    {
                        StatusCode = HttpStatusCode.OK,
                        Content = new StringContent(htmlContents[pageIndex])
                    };
                });

            // Act
            var result = await _bingSearchService.SearchAsync(new SearchRequest(keywords, url), CancellationToken.None);

            // Assert
            var expectedPositions = "1, 2, 3, 4, 5, 6, 7, 8, 9, 10";
            Assert.Equal(expectedPositions, result);
        }
    

        [Fact]
        public async Task SearchAsync_NoMatchingUrl_ReturnsZero()
        {
            // Arrange
            var keywords = "e-settlements";
            var url = "https://www.sympli.com.au";
            var htmlContents = new List<string>
            {
                "<html><ol id=\"b_results\"><li><a href=\"https://www.otherdomain.com/page1\">Link</a></li></ol></html>", // Page 0
                "<html><ol id=\"b_results\"><li><a href=\"https://www.otherdomain.com/page2\">Link</a></li></ol></html>", // Page 1
                "<html><ol id=\"b_results\"><li><a href=\"https://www.otherdomain.com/page3\">Link</a></li></ol></html>", // Page 2
                "<html><ol id=\"b_results\"><li><a href=\"https://www.otherdomain.com/page4\">Link</a></li></ol></html>", // Page 3
                "<html><ol id=\"b_results\"><li><a href=\"https://www.otherdomain.com/page5\">Link</a></li></ol></html>", // Page 4
                "<html><ol id=\"b_results\"><li><a href=\"https://www.otherdomain.com/page6\">Link</a></li></ol></html>", // Page 5
                "<html><ol id=\"b_results\"><li><a href=\"https://www.otherdomain.com/page7\">Link</a></li></ol></html>", // Page 6
                "<html><ol id=\"b_results\"><li><a href=\"https://www.otherdomain.com/page8\">Link</a></li></ol></html>", // Page 7
                "<html><ol id=\"b_results\"><li><a href=\"https://www.otherdomain.com/page9\">Link</a></li></ol></html>", // Page 8
                "<html><ol id=\"b_results\"><li><a href=\"https://www.otherdomain.com/page10\">Link</a></li></ol></html>", // Page 9
                "<html><ol id=\"b_results\"><li><a href=\"https://www.otherdomain.com/page11\">Link</a></li></ol></html>"  // Page 10
            };

            _httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync((HttpRequestMessage request, CancellationToken cancellationToken) =>
                {
                    var requestUri = request.RequestUri;
                    if (requestUri == null)
                    {
                        return new HttpResponseMessage
                        {
                            StatusCode = HttpStatusCode.BadRequest,
                            Content = new StringContent("Invalid RequestUri")
                        };
                    }

                    var pageNumberParam = requestUri.Query.Split('&').FirstOrDefault(q => q.StartsWith("first="));
                    if (pageNumberParam == null)
                    {
                        return new HttpResponseMessage
                        {
                            StatusCode = HttpStatusCode.BadRequest,
                            Content = new StringContent("Invalid query parameter")
                        };
                    }

                    var pageIndex = int.Parse(pageNumberParam.Split('=')[1]) / 10;

                    return new HttpResponseMessage
                    {
                        StatusCode = HttpStatusCode.OK,
                        Content = new StringContent(htmlContents[pageIndex])
                    };
                });

            // Act
            var result = await _bingSearchService.SearchAsync(new SearchRequest(keywords, url), CancellationToken.None);

            // Assert
            var expectedPositions = "0";
            Assert.Equal(expectedPositions, result);
        }
    }
}

