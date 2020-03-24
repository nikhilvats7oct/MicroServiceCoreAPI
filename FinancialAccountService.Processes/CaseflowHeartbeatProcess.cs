using System.Diagnostics;
using System.Threading.Tasks;
using FinancialAccountService.Models.DataTransferObjects;
using FinancialAccountService.Processes.Interfaces;
using FinancialAccountService.Proxy.Interfaces;
using Microsoft.Extensions.Logging;

namespace FinancialAccountService.Processes
{
    public class CaseflowHeartbeatProcess : ICaseflowHeartbeatProcess
    {
        private readonly ILogger<CaseflowHeartbeatProcess> _logger;
        private readonly ICaseflowApiProxy _caseflowApiProxy;

        public CaseflowHeartbeatProcess(ILogger<CaseflowHeartbeatProcess> logger, ICaseflowApiProxy caseflowApiProxy)
        {
            _logger = logger;
            _caseflowApiProxy = caseflowApiProxy;
        }

        public async Task<HeartBeatDto> CallHeartbeatAsync()
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();

            await _caseflowApiProxy.HitHeartBeat();
            
            stopWatch.Stop();

            var caseflowHeartbeat = new HeartBeatDto
            {
                ServiceName = "Caseflow Ping From Account Service",
                RunningElapsedTime = stopWatch.Elapsed,
                TotalElapsedTime = stopWatch.Elapsed
            };

            return caseflowHeartbeat;
        }
    }
}