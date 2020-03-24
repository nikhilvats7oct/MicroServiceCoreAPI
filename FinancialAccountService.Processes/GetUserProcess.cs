using FinancialAccountService.Models.DataTransferObjects;
using FinancialAccountService.Processes.Interfaces;
using FinancialAccountService.Proxy.Interfaces;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace FinancialAccountService.Processes
{
    public class GetUserProcess : IGetUserProcess
    {
        private readonly ILogger<GetUserProcess> _logger;
        private readonly ICaseflowApiProxy _caseFlowApiProxy;

        public GetUserProcess(ILogger<GetUserProcess> logger,
                              ICaseflowApiProxy caseFlowApiProxy)
        {
            _logger = logger;
            _caseFlowApiProxy = caseFlowApiProxy;
        }

        public async Task<CreatedUserDto> GetUser(string userId)
        {
            return await _caseFlowApiProxy.GetUserAsync(userId);
        }
    }
}
