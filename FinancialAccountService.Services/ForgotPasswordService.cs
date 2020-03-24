using FinancialAccountService.Models;
using FinancialAccountService.Models.DataTransferObjects;
using FinancialAccountService.Models.Validation;
using FinancialAccountService.Processes.Interfaces;
using FinancialAccountService.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.Net;
using System.Threading.Tasks;

namespace FinancialAccountService.Services
{
    public class ForgotPasswordService : IForgotPasswordService
    {
        private readonly ILogger<ForgotPasswordService> _logger;
        private readonly ISendForgotPasswordEmailProcess _sendForgotPasswordEmailProcess;

        public ForgotPasswordService(ILogger<ForgotPasswordService> logger,
                                     ISendForgotPasswordEmailProcess sendForgotPasswordEmailProcess)
        {
            _logger = logger;
            _sendForgotPasswordEmailProcess = sendForgotPasswordEmailProcess;
        }

        public async Task SendForgotPasswordAsync(ForgotPasswordDto forgotPasswordDto)
        {
               await _sendForgotPasswordEmailProcess.SendForgotPasswordAsync(forgotPasswordDto);
        }
    }
}