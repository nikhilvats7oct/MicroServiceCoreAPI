using System;
using System.Collections.Generic;
using System.Text;
using FinancialAccountService.Models.DataTransferObjects;
using FinancialAccountService.Models.Interfaces;

namespace FinancialAccountService.Processes.Interfaces
{
    public interface IMyAccountsStatusDeriver
    {
        void DeriveStatusAndStatusSort(
            ICaseFlowAccountCommon caseFlowDto,
            MyAccountsSummaryResultDto resultDtoToBeUpdated);
        bool IsWithSolicitors(ICaseFlowAccountCommon caseFlowDto);
    }
}
