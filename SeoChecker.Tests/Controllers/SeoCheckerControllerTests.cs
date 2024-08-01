using Microsoft.AspNetCore.Mvc;
using Moq;
using SeoChecker.Api.Controllers;
using SeoChecker.Api.Interfaces;
using SeoChecker.Shared.Models;

namespace SeoChecker.Test.Controllers
{
	public class SeoCheckerControllerTests
	{
        private readonly Mock<IHandler> _handlerMock;
        private readonly SeoCheckerController _seoCheckerController;

        public SeoCheckerControllerTests()
        {
            _handlerMock = new Mock<IHandler>();
            _seoCheckerController = new SeoCheckerController(_handlerMock.Object);
        }

        [Fact]
        public async Task CheckSeo_ValidRequest_ReturnsOkResult()
        {
            // Arrange
            var keyword = "e-settlements";
            var url = "https://www.sympli.com.au";
            var expectedResult = "1, 10, 33";

            _handlerMock.Setup(h => h.Handle(It.IsAny<SearchRequest>(), It.IsAny<CancellationToken>()))
                       .ReturnsAsync(expectedResult);

            // Act
            var result = await _seoCheckerController.CheckSeo(CancellationToken.None, keyword, url);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(expectedResult, okResult.Value);
        }

        [Fact]
        public async Task CheckSeo_HandlerThrowsArgumentException_ReturnsBadRequest()
        {
            // Arrange
            var keyword = "e-settlements";
            var url = "https://www.sympli.com.au";
            var exceptionMessage = "Invalid argument";

            _handlerMock.Setup(h => h.Handle(It.IsAny<SearchRequest>(), It.IsAny<CancellationToken>()))
                        .ThrowsAsync(new ArgumentException(exceptionMessage));

            // Act
            var result = await _seoCheckerController.CheckSeo(CancellationToken.None, keyword, url);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(exceptionMessage, badRequestResult.Value);
        }
    }
}

