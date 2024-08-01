using Microsoft.AspNetCore.Mvc;
using SeoChecker.Api.Interfaces;
using SeoChecker.Shared.Models;

namespace SeoChecker.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SeoCheckerController : ControllerBase
    {
        private readonly IHandler _handler;

        public SeoCheckerController(IHandler handler)
        {
            _handler = handler;
        }

        [HttpGet("check")]
        public async Task<IActionResult> CheckSeo(CancellationToken cancellationToken, [FromQuery] string keyword = "e-settlements", [FromQuery] string url = "https://www.sympli.com.au")
        {
            try
            {
                var request = new SearchRequest(keyword, url);
                var result = await _handler.Handle(request, cancellationToken);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}

