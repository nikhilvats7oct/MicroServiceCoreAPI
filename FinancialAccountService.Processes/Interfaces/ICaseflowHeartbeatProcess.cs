using System.Threading.Tasks;
using FinancialAccountService.Models.DataTransferObjects;

namespace FinancialAccountService.Processes.Interfaces
{
    public interface ICaseflowHeartbeatProcess
    {
        Task<HeartBeatDto> CallHeartbeatAsync();
    }
}