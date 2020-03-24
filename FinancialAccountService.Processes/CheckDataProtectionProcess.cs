using FinancialAccountService.Models.DataTransferObjects;
using FinancialAccountService.Processes.Interfaces;
using FinancialAccountService.Proxy.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace FinancialAccountService.Processes
{
    public class CheckDataProtectionProcess : ICheckDataProtectionProcess
    {
        private readonly ILogger<CheckDataProtectionProcess> _logger;
        private readonly ICaseflowApiProxy _caseflowApiProxy;

        public CheckDataProtectionProcess(ILogger<CheckDataProtectionProcess> logger,
                                          ICaseflowApiProxy caseflowApiProxy)
        {
            _logger = logger;
            _caseflowApiProxy = caseflowApiProxy;
        }

        public async Task<bool> CheckDataProtection(RegisterValidationDto dto)
        {
            var result = await _caseflowApiProxy.CheckDataProtection(dto);
            if (result == null)
            {
                //If account does not exist will return null.
                return false;
            }

            if (String.Equals(result.Postcode.Replace(" ", ""), dto.Postcode.Replace(" ", ""), StringComparison.OrdinalIgnoreCase) &&
                result.DateOfBirth == dto.DateOfBirth.ToString("yyyy-MM-dd") &&
                result.LowellReference == dto.LowellReference)
            {
                return true;
            }

            return false;
        }
    }
}
