using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using FinancialAccountService.Models;
using FinancialAccountService.Models.DataTransferObjects;
using FinancialAccountService.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace FinancialAccountService.WebApi.Controllers
{
    [ExcludeFromCodeCoverage]
    [Produces("application/json")]
    [Route("api/Callback")]
    public class CallbackController : Controller
    {
        private readonly ILogger<CallbackController> _logger;
        private readonly ICallbackService _callbackService;

        public CallbackController(ILogger<CallbackController> logger,
                                  ICallbackService callbackService)
        {
            _logger = logger;
            _callbackService = callbackService;
        }

        [HttpPost]
        public async Task<ResultDto> SendCallbackMessageAsync([FromBody]CallbackDto dto)
        {
            try
            {
                _logger.LogInformation("Request received to send Callback request.");

                await _callbackService.SendCallbackRequest(dto);

                _logger.LogInformation("Callback request has been successfully sent.");
                return new ResultDto { IsSuccessful = true };
            }
            catch (Exception ex)
            {
                return new ResultDto
                {
                    IsSuccessful = false,
                    MessageForUser = $"An exception occured while sending callback request: Exception Details - {ex.Message}"
                };
            }
        }
    }
}