using FinancialAccountService.Models.DataTransferObjects;
using FinancialAccountService.Processes.Interfaces;
using FinancialAccountService.Services.Interfaces;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace FinancialAccountService.Services
{
    public class WebActivityService : IWebActivityService
    {
        private readonly ILogWebActivityProcess _logWebActivityProcess;
        public WebActivityService(ILogWebActivityProcess logWebActivityProcess)
        {
            _logWebActivityProcess = logWebActivityProcess;
        }

        public async Task LogWebActivity(WebActivityDto webActivity)
        {
            await _logWebActivityProcess.LogWebActivity(webActivity);
        }
    }
}
