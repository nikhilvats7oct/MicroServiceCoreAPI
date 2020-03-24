using FinancialAccountService.Models.DataTransferObjects;
using FinancialAccountService.Services.Interfaces;
using FinancialAccountService.WebApi.CustomHeaders;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Threading.Tasks;

namespace FinancialAccountService.WebApi.Controllers
{
    [SecurityHeaders]
    [ExcludeFromCodeCoverage]
    [Authorize]
    [Produces("application/json")]
    [Route("api/Register")]
    public class RegisterController : Controller
    {
        private readonly ILogger<RegisterController> _logger;
        private readonly IRegisterService _registerService;

        public RegisterController(ILogger<RegisterController> logger,
                                  IRegisterService registerService)
        {
            _logger = logger;
            _registerService = registerService;
        }

        [HttpPost("CheckDataProtection")]
        public async Task<IActionResult> CheckDataProtection([FromBody] RegisterValidationDto model)
        {
            var result = await _registerService.CheckDataProtectionAsync(model);
            return Ok(result);
        }

        [HttpPost("CheckIsWebRegistered")]
        public async Task<IActionResult> CheckIsWebRegistered([FromBody] WebRegisteredDto model)
        {
            var result = await _registerService.CheckIsWebRegisteredAsync(model);
            return Ok(result);
        }

        [HttpPost("SendRegistrationEmail")]
        public async Task<IActionResult> SendRegistrationEmail([FromBody] RegistrationEmailDto model)
        {
            if (!ModelState.IsValid)
            {
                Forbid();
            }

            await _registerService.SendRegistrationEmailAsync(model);
            return Ok();
        }

        [HttpPost("CompleteRegistration")]
        public async Task<IActionResult> CompleteRegistration([FromBody] CompleteRegistrationDto model)
        {
            if (!ModelState.IsValid)
            {
                Forbid();
            }

            await _registerService.CompleteRegistrationAsync(model);

            return Ok();
        }

        [HttpPost("CompleteDeregistration")]
        public async Task<IActionResult> CompleteDeregistration([FromBody] CompleteDeregistrationDto completeDeregisterDto)
        {
            //Add Validation
            if (!ModelState.IsValid)
            {
                Forbid();
            }

            await _registerService.CompleteDeregistrationAsync(completeDeregisterDto);

            return Ok();
        }
    }
}