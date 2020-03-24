using FinancialAccountService.Models.DataTransferObjects;
using FinancialAccountService.Processes.Interfaces;
using System;
using System.Threading.Tasks;
using FinancialAccountService.Proxy.Interfaces;
using Microsoft.Extensions.Logging;

namespace FinancialAccountService.Processes
{
    public class CompleteDeregistrationProcess : ICompleteDeregistrationProcess
    {
        private readonly ILogger<CompleteDeregistrationProcess> _logger;
        private readonly ICaseflowApiProxy _caseflowApiProxy;

        public CompleteDeregistrationProcess(ILogger<CompleteDeregistrationProcess> logger, ICaseflowApiProxy caseflowApiProxy)
        {
            _logger = logger;
            _caseflowApiProxy = caseflowApiProxy;
        }

        public async Task CompleteDeregistrationAsync(CompleteDeregistrationDto completeDeregistrationDto)
        {
            await _caseflowApiProxy.CompleteDeregistrationAsync(completeDeregistrationDto, completeDeregistrationDto.ReplayId);
        }
    }
}
