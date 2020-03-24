using FinancialAccountService.Models;
using FinancialAccountService.Models.DataTransferObjects;
using FinancialAccountService.Services.Interfaces;
using FinancialAccountService.WebApi.CustomHeaders;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace FinancialAccountService.WebApi.Controllers
{
    [SecurityHeaders]
    [Authorize]
    [Produces("application/json")]
    [Route("api/ForgotPassword")]
    public class ForgotPasswordController : Controller
    {
        private readonly ILogger<ForgotPasswordController> _logger;
        private readonly IForgotPasswordService _forgotPasswordService;

        public ForgotPasswordController(ILogger<ForgotPasswordController> logger,
                                        IForgotPasswordService forgotPasswordService)
        {
            _logger = logger;
            _forgotPasswordService = forgotPasswordService;
        }

        [HttpPost("Send")]
        public async Task SendForgotPassword([FromBody] ForgotPasswordDto forgotPasswordDto)
        {
            if (!ModelState.IsValid)
            {
                Forbid();
            }

            await _forgotPasswordService.SendForgotPasswordAsync(forgotPasswordDto);

        }
    }
}