using FinancialAccountService.Models;
using FinancialAccountService.Models.DataTransferObjects;
using System.Threading.Tasks;

namespace FinancialAccountService.Services.Interfaces
{
    public interface IRegisterService
    {
        Task<ResultDto> CheckDataProtectionAsync(RegisterValidationDto model);
        Task<ResultDto> CheckIsWebRegisteredAsync(WebRegisteredDto model);
        Task SendRegistrationEmailAsync(RegistrationEmailDto model);
        Task CompleteRegistrationAsync(CompleteRegistrationDto model);
        Task CompleteDeregistrationAsync(CompleteDeregistrationDto completeDeregisterDto);
    }
}