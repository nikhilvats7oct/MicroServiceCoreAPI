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
    [Produces("application/json")]
    [Route("api/Users")]
    public class UsersController : Controller
    {
        private readonly ILogger<UsersController> _logger;
        private readonly IUsersService _usersService;

        public UsersController(ILogger<UsersController> logger,
                               IUsersService userService)
        {
            _logger = logger;
            _usersService = userService;
        }

        [HttpPost("GetUser")]
        public async Task<IActionResult> GetUser([FromBody] string userId)
        {
            CreatedUserDto result = await _usersService.Get(userId);
            return Ok(result);
        }

    }
}
