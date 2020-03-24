using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using FinancialAccountService.Models.DataTransferObjects;

namespace FinancialAccountService.Services.Interfaces
{
    public interface IMyAccountsService
    {
        Task<MyAccountsResultDto> GetMyAccounts(string userId);

        Task<MyAccountsResultDto> GetMyAccountsSummary(string accountId);        

        Task<MyAccountsDetailResultDto> GetMyAccountsDetail(string userId, string lowellReference);
    }
}
