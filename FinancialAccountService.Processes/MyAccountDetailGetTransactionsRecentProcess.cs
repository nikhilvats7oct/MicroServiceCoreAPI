using FinancialAccountService.Models.DataTransferObjects;
using FinancialAccountService.Processes.Interfaces;
using FinancialAccountService.Proxy.Interfaces;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace FinancialAccountService.Processes
{
    public class MyAccountDetailGetTransactionsRecentProcess : IMyAccountDetailGetTransactionsRecentProcess
    {
        private readonly ILogger<MyAccountDetailGetTransactionsRecentProcess> _logger;
        private readonly ICaseflowApiProxy _caseFlowApiProxy;

        public MyAccountDetailGetTransactionsRecentProcess(ILogger<MyAccountDetailGetTransactionsRecentProcess> logger,
                                                           ICaseflowApiProxy caseFlowApiProxy)
        {
            _logger = logger;
            _caseFlowApiProxy = caseFlowApiProxy;
        }

        public async Task<RecievedTransactionsDto> GetTransactionsRecent(string lowellReference, uint limit)
        {
            return await _caseFlowApiProxy.GetTransactionsAsync(lowellReference, limit);
        }

    }
}
