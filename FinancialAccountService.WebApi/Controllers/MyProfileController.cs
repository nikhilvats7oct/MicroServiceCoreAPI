using FinancialAccountService.Models.DataTransferObjects;
using FinancialAccountService.Proxy.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace FinancialAccountService.WebApi.Controllers
{
    [Authorize]
    [ExcludeFromCodeCoverage]
    [Produces("application/json")]
    [Route("api/MyProfile")]
    public class MyProfileController : Controller
    {
        private readonly ILogger<MyProfileController> _logger;
        private readonly ICaseflowApiProxy _proxy;

        public MyProfileController(ILogger<MyProfileController> logger,
                                   ICaseflowApiProxy proxy)
        {
            _logger = logger;
            _proxy = proxy;
        }

        [HttpPost("UpdateEmail")]
        public async Task UpdateEmail([FromBody] UpdateEmailDto model)
        {
            if (!ModelState.IsValid)
            {
                Forbid();
            }

            await _proxy.UpdateUserEmailAsync(model, model.ReplayId);
        }

        [HttpPost("GetContactPreferences")]
        public async Task<IActionResult> GetContactPreferences([FromBody] RetrieveContactPreferences model)
        {
            var result = await _proxy.GetContactPreferences(model.LowellReference);

            return Ok(result);
        }

        [HttpPost("SaveContactPreferences")]
        public async Task SaveContactPreferences([FromBody] SaveContactPreferences model)
        {
            await _proxy.SaveContactPreferences(model);
        }
    }
}
