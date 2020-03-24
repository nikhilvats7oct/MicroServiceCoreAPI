using System.Threading.Tasks;
using FinancialAccountService.Models;
using FinancialAccountService.Models.DataTransferObjects;
using FinancialAccountService.Proxy.Interfaces;
using FinancialAccountService.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace FinancialAccountService.Services
{
    public class CallbackService : ICallbackService
    {
        private readonly ILogger<CallbackService> _logger;
        private readonly ICaseflowApiProxy _caseflowApiProxy;

        public CallbackService(ILogger<CallbackService> logger, 
                               ICaseflowApiProxy caseflowApiProxy)
        {
            _logger = logger;
            _caseflowApiProxy = caseflowApiProxy;
        }

        public async Task SendCallbackRequest(CallbackDto dto)
        {
            _logger.LogInformation("Callback request action sending request to caseflow.");

            await _caseflowApiProxy.SendCallbackRequest(dto);

            _logger.LogInformation("Callback request action has been sent to caseflow.");
        }
    }
}
