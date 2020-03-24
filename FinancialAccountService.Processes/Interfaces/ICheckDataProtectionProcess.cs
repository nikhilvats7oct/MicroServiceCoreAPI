using FinancialAccountService.Models.DataTransferObjects;
using System.Threading.Tasks;

namespace FinancialAccountService.Processes.Interfaces
{
    public interface ICheckDataProtectionProcess
    {
        Task<bool> CheckDataProtection(RegisterValidationDto dto);
    }
}
