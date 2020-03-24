using FinancialAccountService.Services.Interfaces;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using FinancialAccountService.Models.DataTransferObjects;
using FinancialAccountService.Processes.Interfaces;

namespace FinancialAccountService.Services
{
    public class CaseflowHeartbeatService : ICaseflowHeartbeatService
    {
        private readonly ILogger<CaseflowHeartbeatService> _logger;
        private readonly ICaseflowHeartbeatProcess _caseflowHeartbeatProcess;

        public CaseflowHeartbeatService(ILogger<CaseflowHeartbeatService> logger, ICaseflowHeartbeatProcess caseflowHeartbeatProcess)
        {
            _logger = logger;
            _caseflowHeartbeatProcess = caseflowHeartbeatProcess;
        }
        
        public async Task<HeartBeatDto> CallHeartbeatAsync()
        {
            _logger.LogInformation("CaseflowHeartbeatService CallHeartbeat action account service");
            return await _caseflowHeartbeatProcess.CallHeartbeatAsync();
        }
    }
}