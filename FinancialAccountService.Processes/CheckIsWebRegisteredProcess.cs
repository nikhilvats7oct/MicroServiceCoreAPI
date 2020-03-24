using FinancialAccountService.Models.DataTransferObjects;
using FinancialAccountService.Processes.Interfaces;
using FinancialAccountService.Proxy.Interfaces;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace FinancialAccountService.Processes
{
    public class CheckIsWebRegisteredProcess : ICheckIsWebRegisteredProcess
    {
        private readonly ILogger<CheckIsWebRegisteredProcess> _logger;
        private readonly ICaseflowApiProxy _caseflowApiProxy;

        public CheckIsWebRegisteredProcess(ILogger<CheckIsWebRegisteredProcess> logger,
                                           ICaseflowApiProxy caseflowApiProxy)
        {
            _logger = logger;
            _caseflowApiProxy = caseflowApiProxy;
        }

        public async Task<bool> CheckHasWebAccountAsync(WebRegisteredDto dto)
        {
            var result = await _caseflowApiProxy.CheckHasWebAccountAsync(dto);

            foreach (var summary in result.Summaries)
            {
                if (summary.HasWebAccount == true)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
