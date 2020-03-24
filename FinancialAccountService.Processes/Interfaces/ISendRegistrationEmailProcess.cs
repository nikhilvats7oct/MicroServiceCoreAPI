using FinancialAccountService.Models.DataTransferObjects;
using System.Threading.Tasks;

namespace FinancialAccountService.Processes.Interfaces
{
    public interface ISendRegistrationEmailProcess
    {
        Task SendRegistrationEmailAsync(RegistrationEmailDto model);
    }
}
