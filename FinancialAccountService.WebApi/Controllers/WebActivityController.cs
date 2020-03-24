using FinancialAccountService.Models.DataTransferObjects;
using FinancialAccountService.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace FinancialAccountService.WebApi.Controllers
{
    [Authorize]
    [ExcludeFromCodeCoverage]
    [Route("api/WebActivity")]
    public class WebActivityController : Controller
    {
        private readonly IWebActivityService _webActivityService;

        public WebActivityController(IWebActivityService webActivityService)
        {
            _webActivityService = webActivityService;
        }

        [HttpPost("LogWebActivity")]
        public async Task LogWebActivity([FromBody] WebActivityDto webActivity)
        {
            await _webActivityService.LogWebActivity(webActivity);
        }

    }
}
