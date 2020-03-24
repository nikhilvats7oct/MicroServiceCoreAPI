using FinancialAccountService.Models.DataTransferObjects;
using System.Threading.Tasks;

namespace FinancialAccountService.Processes.Interfaces
{
    public interface ICompleteDeregistrationProcess
    {
        Task CompleteDeregistrationAsync(CompleteDeregistrationDto completeDeregistrationDto);
    }
}
