using System.Threading.Tasks;
using FinancialAccountService.Models.DataTransferObjects;

namespace FinancialAccountService.Services.Interfaces
{
    public interface ICaseflowHeartbeatService
    {
        Task<HeartBeatDto> CallHeartbeatAsync();
    }
}