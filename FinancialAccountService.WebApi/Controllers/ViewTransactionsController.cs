using FinancialAccountService.Models.DataTransferObjects;
using FinancialAccountService.Proxy.Interfaces;
using FinancialAccountService.WebApi.CustomHeaders;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace FinancialAccountService.WebApi.Controllers
{
    [SecurityHeaders]
    [Authorize]
    [Produces("application/json")]
    [Route("api/ViewTransactions")]
    public class ViewTransactionsController : Controller
    {
        private readonly ILogger<ViewTransactionsController> _logger;
        private readonly ICaseflowApiProxy _proxy;

        public ViewTransactionsController(ILogger<ViewTransactionsController> logger,
                                          ICaseflowApiProxy proxy)
        {
            _logger = logger;
            _proxy = proxy;
        }

        [HttpPost("GetTransactions")]
        public async Task<IActionResult> GetTransactions([FromBody] GetTransactionsDto dto)
        {
            _logger.LogInformation("ViewTransactionsController GetTransactions action account service");

            var transactions = await _proxy.GetTransactionsAsync(dto);

            return Ok(transactions);
        }
    }
}
