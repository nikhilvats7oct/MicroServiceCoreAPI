using FinancialAccountService.Models;
using FinancialAccountService.Models.DataTransferObjects;
using FinancialAccountService.Models.Validation;
using FinancialAccountService.Processes.Interfaces;
using FinancialAccountService.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.Threading.Tasks;

namespace FinancialAccountService.Services
{
    public class RegisterService : IRegisterService
    {
        private readonly ILogger<RegisterService> _logger;
        private readonly ICheckDataProtectionProcess _checkDataProtectionProcess;
        private readonly ICheckIsWebRegisteredProcess _checkIsWebRegisteredProcess;
        private readonly ISendRegistrationEmailProcess _sendRegistrationEmailProcess;
        private readonly ICompleteRegistrationProcess _completeRegistrationProcess;
        private readonly ICompleteDeregistrationProcess _completeDeregistrationProcess;
        

        public RegisterService(ILogger<RegisterService> logger,
                               ICheckDataProtectionProcess checkDataProtectionProcess,
                               ICheckIsWebRegisteredProcess checkIsWebRegisteredProcess,
                               ISendRegistrationEmailProcess sendRegistrationEmailProcess,
                               ICompleteRegistrationProcess completeRegistrationProcess, ICompleteDeregistrationProcess completeDeregistrationProcess)
        {
            _logger = logger;
            _checkDataProtectionProcess = checkDataProtectionProcess;
            _checkIsWebRegisteredProcess = checkIsWebRegisteredProcess;
            _sendRegistrationEmailProcess = sendRegistrationEmailProcess;
            _completeRegistrationProcess = completeRegistrationProcess;
            _completeDeregistrationProcess = completeDeregistrationProcess;
        }

        public async Task<ResultDto> CheckDataProtectionAsync(RegisterValidationDto model)
        {
            var result = new ResultDto();

            if (await _checkDataProtectionProcess.CheckDataProtection(model))
            {
                result.IsSuccessful = true;
            }
            else
            {
                result.IsSuccessful = false;
                result.MessageForUser = ValidationMessages.DataProtectionCheckFailed;
                return result;
            }

            return result;
        }

        public async Task<ResultDto> CheckIsWebRegisteredAsync(WebRegisteredDto model)
        {
            var result = new ResultDto();

            try
            {
                if (await _checkIsWebRegisteredProcess.CheckHasWebAccountAsync(model))
                {
                    result.IsSuccessful = false;
                    result.MessageForUser = ValidationMessages.AccountAlreadyWebRegistered;
                }
                else
                {
                    result.IsSuccessful = true;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error in CheckIsWebRegisteredAsync.");
                result.IsSuccessful = false;
            }

            return result;
        }

        public async Task SendRegistrationEmailAsync(RegistrationEmailDto model)
        {
            await _sendRegistrationEmailProcess.SendRegistrationEmailAsync(model);
        }

        public async Task CompleteRegistrationAsync(CompleteRegistrationDto model)
        {            
             await _completeRegistrationProcess.CompleteRegistrationProcessAsync(model);
        }

        public async Task CompleteDeregistrationAsync(CompleteDeregistrationDto completeDeregistrationDto)
        {
            await _completeDeregistrationProcess.CompleteDeregistrationAsync(completeDeregistrationDto);
        }
    }
}