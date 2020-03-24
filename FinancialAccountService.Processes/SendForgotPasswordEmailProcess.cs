using FinancialAccountService.Models.DataTransferObjects;
using FinancialAccountService.Processes.Interfaces;
using FinancialAccountService.Proxy.Interfaces;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FinancialAccountService.Processes
{
    public class SendForgotPasswordEmailProcess : ISendForgotPasswordEmailProcess
    {
        private readonly ILogger<SendForgotPasswordEmailProcess> _logger;
        private readonly ICaseflowApiProxy _caseApiProxy;

        public SendForgotPasswordEmailProcess(ILogger<SendForgotPasswordEmailProcess> logger,
                                              ICaseflowApiProxy caseflowApiProxy)
        {
            _logger = logger;
            _caseApiProxy = caseflowApiProxy;
        }

        public async Task SendForgotPasswordAsync(ForgotPasswordDto model)
        {
            var forgottenPassword = new SendForgottenPasswordDto()
            {
                EmailAddress = model.EmailAddress,
                Data = new List<SendForgottenPasswordDto.DataItem>()
                {
                    new SendForgottenPasswordDto.DataItem
                    {
                        Value = model.CallBackUrl
                    },
                },
                Name = model.EmailName
            };

            await _caseApiProxy.SendForgotPasswordAsync(forgottenPassword, model.LowellRef, model.ReplayId);            
        }
    }
}