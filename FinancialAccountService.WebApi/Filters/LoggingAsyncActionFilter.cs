using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Threading.Tasks;

namespace FinancialAccountService.WebApi.Filters
{
    public class LoggingAsyncActionFilter : IAsyncActionFilter
    {
        protected ILogger<LoggingAsyncActionFilter> _logger { get; }
        public LoggingAsyncActionFilter(ILogger<LoggingAsyncActionFilter> logger)
        {
            _logger = logger;
        }
        public async Task OnActionExecutionAsync(
            ActionExecutingContext context,
            ActionExecutionDelegate next)
        {
            var sw = Stopwatch.StartNew();

            var requestURL = context.HttpContext.Request.GetDisplayUrl();

            _logger.LogInformation($"Request starting {requestURL}.");

            await next();
            sw.Stop();

            _logger.LogInformation($"Request finished for {requestURL} in {sw.Elapsed.TotalMilliseconds}ms.");


        }
    }
}
