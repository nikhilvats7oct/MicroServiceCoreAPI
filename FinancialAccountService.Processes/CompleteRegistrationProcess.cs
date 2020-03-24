using FinancialAccountService.Models.DataTransferObjects;
using FinancialAccountService.Processes.Interfaces;
using FinancialAccountService.Proxy.Interfaces;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace FinancialAccountService.Processes
{
    public class CompleteRegistrationProcess : ICompleteRegistrationProcess
    {
        private readonly ILogger<CompleteRegistrationProcess> _logger;
        private readonly ICaseflowApiProxy _caseflowApiProxy;

        public CompleteRegistrationProcess(ILogger<CompleteRegistrationProcess> logger,
                                           ICaseflowApiProxy caseflowApiProxy)
        {
            _logger = logger;
            _caseflowApiProxy = caseflowApiProxy;
        }

        public async Task CompleteRegistrationProcessAsync(CompleteRegistrationDto dto)
        {
             await _caseflowApiProxy.CompleteRegistrationAsync(dto, dto.ReplayId);
        }
    }
}
