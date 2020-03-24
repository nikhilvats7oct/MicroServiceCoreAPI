using System.Collections.Generic;

namespace FinancialAccountService.Models.DataTransferObjects
{
    public class MyAccountsResultDto
    {
        public List<MyAccountsSummaryResultDto> Accounts { get; set; }
    }
}
