using FinancialAccountService.Models.DataTransferObjects;
using FinancialAccountService.Services.Interfaces;
using FinancialAccountService.WebApi.CustomHeaders;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace FinancialAccountService.WebApi.Controllers
{
    [SecurityHeaders]
    [Route("api/Heartbeat")]
    public class HeartBeatController : Controller
    {
        private readonly ILogger<HeartBeatController> _logger;
        private readonly ICaseflowHeartbeatService _caseflowHeartbeatService;
        private readonly IConfiguration _configuration;

        public HeartBeatController(ILogger<HeartBeatController> logger,
                                   ICaseflowHeartbeatService caseflowHeartbeatService, IConfiguration configuration)
        {
            _logger = logger;
            _caseflowHeartbeatService = caseflowHeartbeatService;
            _configuration = configuration;
        }

        [HttpGet]
        public IActionResult Index()
        {
            _logger.LogInformation("HeartBeatController index action account service");
            return Ok(DateTime.Now);
        }

        [HttpGet("CheckAccountService")]
        public async Task<IActionResult> CheckAccountService()
        {
            _logger.LogInformation("HeartBeatController CheckAccountService action account service");
            var stopWatch = new Stopwatch();
            stopWatch.Start();

            var greenThreshold = _configuration.GetValue<int>("HeartBeat:GreenThreshold");
            var redThreshold = _configuration.GetValue<int>("HeartBeat:RedThreshold");
            var caseflowGreenThreshold = _configuration.GetValue<int>("HeartBeat:CaseflowGreenThreshold");
            var caseflowredThreshold = _configuration.GetValue<int>("HeartBeat:CaseflowRedThreshold");
            
            HeartBeatDto caseflowResult;
            try
            {
                caseflowResult = await _caseflowHeartbeatService.CallHeartbeatAsync();
                caseflowResult.SetStatus(caseflowGreenThreshold, caseflowredThreshold);
            }
            catch
            {
                caseflowResult = new HeartBeatDto
                {
                    ServiceName = "Caseflow Ping From Account Service",
                    Status = "RED",
                    Details = "Account service is down or Caseflow is down"
                };
            }
            
            stopWatch.Stop();
            var result = new HeartBeatDto
            {
                ServiceName = "Financial Account Service",
                TotalElapsedTime = stopWatch.Elapsed,
                RunningElapsedTime = stopWatch.Elapsed - caseflowResult.RunningElapsedTime
            };

            result.SetStatus(greenThreshold, redThreshold);
            result.ChildHeartBeat = caseflowResult;

            return Ok(result);
        }
    }
}