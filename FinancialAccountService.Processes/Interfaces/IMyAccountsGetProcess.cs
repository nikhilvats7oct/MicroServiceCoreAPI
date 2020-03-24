using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using FinancialAccountService.Models.DataTransferObjects;

namespace FinancialAccountService.Processes.Interfaces
{
    public interface IMyAccountsGetProcess
    {
        Task<CaseFlowMyAccountsDto> GetMyAccounts(string userId);
        Task<CaseFlowMyAccountsDto> GetMyAccountsSummary(string accountId);
    }
}
