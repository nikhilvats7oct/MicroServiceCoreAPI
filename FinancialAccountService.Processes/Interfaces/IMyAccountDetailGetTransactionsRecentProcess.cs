using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using FinancialAccountService.Models.DataTransferObjects;

namespace FinancialAccountService.Processes.Interfaces
{
    public interface IMyAccountDetailGetTransactionsRecentProcess
    {
        Task<RecievedTransactionsDto> GetTransactionsRecent(string lowellReference, uint limit);
    }
}
