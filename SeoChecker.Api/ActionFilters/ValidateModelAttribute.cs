using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace SeoChecker.Api.ActionFilters
{
    public class ValidateModelAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.ActionArguments.TryGetValue("keyword", out var keywordsObj) &&
            context.ActionArguments.TryGetValue("Url", out var targetUrlObj))
            {
                if (keywordsObj is not string keywords || string.IsNullOrWhiteSpace(keywords))
                {
                    context.Result = new BadRequestObjectResult("Keyword must not be empty.");
                    return;
                }

                if (targetUrlObj is not string targetUrl || string.IsNullOrWhiteSpace(targetUrl))
                {
                    context.Result = new BadRequestObjectResult("Target URL must not be empty.");
                    return;
                }

                if (!IsValidUrl(targetUrl))
                {
                    context.Result = new BadRequestObjectResult("Invalid URL format.");
                    return;
                }
            }
            else
            {
                context.Result = new BadRequestObjectResult("Missing required parameters.");
                return;
            }

            base.OnActionExecuting(context);
        }

        private bool IsValidUrl(string url)
        {
            return Uri.TryCreate(url, UriKind.Absolute, out Uri? uriResult)
                   && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.Exception != null)
            {
                context.Result = new ObjectResult("An error occurred while processing your request.")
                {
                    StatusCode = 500
                };
                context.ExceptionHandled = true;
            }

            base.OnActionExecuted(context);
        }
    }
}

