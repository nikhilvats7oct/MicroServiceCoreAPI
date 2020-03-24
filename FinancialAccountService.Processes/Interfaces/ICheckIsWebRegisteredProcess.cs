using FinancialAccountService.Models.DataTransferObjects;
using System.Threading.Tasks;

namespace FinancialAccountService.Processes.Interfaces
{
    public interface ICheckIsWebRegisteredProcess
    {
        Task<bool> CheckHasWebAccountAsync(WebRegisteredDto dto);
    }
}
