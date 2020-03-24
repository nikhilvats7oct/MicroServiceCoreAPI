using FinancialAccountService.Models.DataTransferObjects;
using FinancialAccountService.Processes.Interfaces;
using FinancialAccountService.Proxy.Interfaces;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace FinancialAccountService.Processes
{
    public class LogWebActivityProcess : ILogWebActivityProcess
    {
        private readonly ICaseflowApiProxy _caseFlowApiProxy;
        public LogWebActivityProcess(ICaseflowApiProxy caseFlowApiProxy)
        {
            _caseFlowApiProxy = caseFlowApiProxy;
        }

        public async Task LogWebActivity(WebActivityDto webActivity)
        {
            await _caseFlowApiProxy.LogWebActivity(webActivity);
        }
    }
}
