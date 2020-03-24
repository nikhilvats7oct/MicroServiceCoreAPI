using FinancialAccountService.Models.DataTransferObjects;
using FinancialAccountService.Processes.Interfaces;
using FinancialAccountService.Services.Interfaces;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace FinancialAccountService.Services
{
    public class UsersService : IUsersService
    {
        private readonly ILogger<UsersService> _logger;
        private readonly IGetUserProcess _getUserProcess;

        public UsersService(ILogger<UsersService> logger,
                            IGetUserProcess getUserProcess)
        {
            _logger = logger;
            _getUserProcess = getUserProcess;
        }

        public async Task<CreatedUserDto> Get(string userId)
        {
            return await _getUserProcess.GetUser(userId);
        }
    }
}
