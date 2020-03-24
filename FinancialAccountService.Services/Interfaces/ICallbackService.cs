using System.Threading.Tasks;
using FinancialAccountService.Models;
using FinancialAccountService.Models.DataTransferObjects;

namespace FinancialAccountService.Services.Interfaces
{
    public interface ICallbackService
    {
        Task SendCallbackRequest(CallbackDto dto);
    }
}