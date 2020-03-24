using System.Net.Http;
using FinancialAccountService.Models.DataTransferObjects;
using System.Threading.Tasks;

namespace FinancialAccountService.Processes.Interfaces
{
    public interface ICompleteRegistrationProcess
    {
        Task CompleteRegistrationProcessAsync(CompleteRegistrationDto dto);
    }
}
