using FinancialAccountService.Models.DataTransferObjects;
using FinancialAccountService.Services.Interfaces;
using FinancialAccountService.WebApi.CustomHeaders;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace FinancialAccountService.WebApi.Controllers
{
    [Authorize]
    [SecurityHeaders]
    [ExcludeFromCodeCoverage]
    [Produces("application/json")]
    [Route("api/MyAccounts")]
    public class MyAccountsController : Controller
    {
        private readonly ILogger<MyAccountsController> _logger;
        private readonly IMyAccountsService _myAccountsService;

        public MyAccountsController(ILogger<MyAccountsController> logger,
                                    IMyAccountsService myAccountsService)
        {
            _logger = logger;
            _myAccountsService = myAccountsService;
        }

        [HttpPost("GetMyAccounts")]
        public async Task<IActionResult> GetMyAccounts([FromBody] MyAccountsDto model)
        {
            // TODO: review versus proxy behavaviour and other controllers
            if (!ModelState.IsValid)
                return new BadRequestResult();

            var result = await _myAccountsService.GetMyAccounts(model.UserId);
            return Ok(result);
        }

        [HttpPost("GetMyAccountsSummary")]
        public async Task<IActionResult> GetMyAccountsSummary([FromBody] MyAccountsSummaryDto model)
        {
            // TODO: review versus proxy behavaviour and other controllers
            if (!ModelState.IsValid)
                return new BadRequestResult();

            var result = await _myAccountsService.GetMyAccountsSummary(model.AccountId);
            return Ok(result);
        }

        [HttpPost("GetMyAccountsDetail")]
        public async Task<IActionResult> GetMyAccountsDetail([FromBody] MyAccountsDetailDto model)
        {
            // TODO: review versus proxy behavaviour and other controllers
            if (!ModelState.IsValid)
                return new BadRequestResult();

            var result = await _myAccountsService.GetMyAccountsDetail(model.UserId, model.LowellReference);
            return Ok(result);
        }

    }
}
