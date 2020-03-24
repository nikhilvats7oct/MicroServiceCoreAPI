using System.Threading.Tasks;
using FinancialAccountService.Models;
using FinancialAccountService.Models.DataTransferObjects;

namespace FinancialAccountService.Services.Interfaces
{
    public interface IForgotPasswordService
    {
        Task SendForgotPasswordAsync(ForgotPasswordDto forgotPasswordDto);
    }
}
