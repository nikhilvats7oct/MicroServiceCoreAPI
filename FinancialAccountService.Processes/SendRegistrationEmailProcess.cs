using FinancialAccountService.Models.DataTransferObjects;
using FinancialAccountService.Processes.Interfaces;
using FinancialAccountService.Proxy.Interfaces;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FinancialAccountService.Processes
{
    public class SendRegistrationEmailProcess : ISendRegistrationEmailProcess
    {
        private readonly ILogger<SendRegistrationEmailProcess> _logger;
        private readonly ICaseflowApiProxy _caseflowApiProxy;

        public SendRegistrationEmailProcess(ILogger<SendRegistrationEmailProcess> logger,
                                            ICaseflowApiProxy caseflowApiProxy)
        {
            _logger = logger;
            _caseflowApiProxy = caseflowApiProxy;
        }

        public async Task SendRegistrationEmailAsync(RegistrationEmailDto model)
        {
            var sendEmail = new SendEmailDto()
            {
                EmailAddress = model.EmailAddress,
                Data = new List<SendEmailDto.DataItem>()
                {
                    new SendEmailDto.DataItem {Value = model.CallBackUrl, Key = "ActivationUrl" },
                },
                Name = model.EmailName
            };

            await _caseflowApiProxy.SendRegistrationEmailAsync(sendEmail, model.LowellReference, model.ReplayId);
        }
    }
}
