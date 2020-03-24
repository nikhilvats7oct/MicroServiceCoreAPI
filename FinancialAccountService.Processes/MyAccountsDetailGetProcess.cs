using FinancialAccountService.Models.DataTransferObjects;
using FinancialAccountService.Processes.Interfaces;
using FinancialAccountService.Proxy.Interfaces;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace FinancialAccountService.Processes
{
    public class MyAccountsDetailGetProcess : IMyAccountsDetailGetProcess
    {
        private readonly ILogger<MyAccountsDetailGetProcess> _logger;
        private readonly ICaseflowApiProxy _caseFlowApiProxy;

        public MyAccountsDetailGetProcess(ILogger<MyAccountsDetailGetProcess> logger,
                                          ICaseflowApiProxy caseFlowApiProxy)
        {
            _logger = logger;
            _caseFlowApiProxy = caseFlowApiProxy;
        }

        public async Task<CaseFlowMyAccountsDetailDto> GetMyAccountsDetail(string lowellReference)
        {
            return await _caseFlowApiProxy.GetMyAccountsDetailAsync(lowellReference);
        }
    }
}
