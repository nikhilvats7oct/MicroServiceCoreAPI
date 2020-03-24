using FinancialAccountService.Models.DataTransferObjects;
using FinancialAccountService.Processes.Interfaces;
using FinancialAccountService.Proxy.Interfaces;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System.Linq;

namespace FinancialAccountService.Processes
{
    public class MyAccountsGetProcess : IMyAccountsGetProcess
    {
        private readonly ILogger<MyAccountsGetProcess> _logger;
        private readonly ICaseflowApiProxy _caseFlowApiProxy;

        public MyAccountsGetProcess(ILogger<MyAccountsGetProcess> logger,
                                    ICaseflowApiProxy caseFlowApiProxy)
        {
            _logger = logger;
            _caseFlowApiProxy = caseFlowApiProxy;
        }

        public async Task<CaseFlowMyAccountsDto> GetMyAccounts(string userId)
        {
            CaseFlowMyAccountsDto result = await _caseFlowApiProxy.GetMyAccountsAsync(userId);
            if (result.Summaries != null)
            {
                result.Summaries = result.Summaries.Where(x => !x.IsClosedHidden).ToList();
            }

            return result;
        }

        public async Task<CaseFlowMyAccountsDto> GetMyAccountsSummary(string accountId)
        {
            CaseFlowMyAccountsDto result = await _caseFlowApiProxy.GetMyAccountsSummaryAsync(accountId);
            if (result.Summaries != null)
            {
                result.Summaries = result.Summaries.Where(x => !x.IsClosedHidden).ToList();
            }

            return result;
        }

    }
}
