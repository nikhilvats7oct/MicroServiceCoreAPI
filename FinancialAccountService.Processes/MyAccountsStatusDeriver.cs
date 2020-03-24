using FinancialAccountService.Models.DataTransferObjects;
using FinancialAccountService.Models.Interfaces;
using FinancialAccountService.Processes.Interfaces;
using Microsoft.Extensions.Logging;

namespace FinancialAccountService.Processes
{
    public class MyAccountsStatusDeriver : IMyAccountsStatusDeriver
    {
        private const long SolicitorsCompanyCode = 2;

        private readonly ILogger<MyAccountsStatusDeriver> _logger;

        public MyAccountsStatusDeriver(ILogger<MyAccountsStatusDeriver> logger)
        {
            _logger = logger;
        }

        // Updates the result Dto (passed up to portal), using CaseFlow information
        public void DeriveStatusAndStatusSort(ICaseFlowAccountCommon caseFlowDto, MyAccountsSummaryResultDto resultDtoToBeUpdated)
        {
            // Closed
            if ((caseFlowDto.OutstandingBalance ?? 0M) <= 0M
                && caseFlowDto.CanMakePayment == false
                && caseFlowDto.CanSetupIndividualPlan == false)
            {
                resultDtoToBeUpdated.AccountStatus = "Closed";
                resultDtoToBeUpdated.AccountStatusSort = 5;
                resultDtoToBeUpdated.AccountStatusIsClosed = true;
                return;
            }

            // With Lowell Solicitors
            if (caseFlowDto.Company == SolicitorsCompanyCode)
            {
                resultDtoToBeUpdated.AccountStatus = "With Lowell Solicitors";
                resultDtoToBeUpdated.AccountStatusSort = 3;
                resultDtoToBeUpdated.AccountStatusIsWithSolicitors = true;
                return;
            }

            // View Account Details Only
            if ((caseFlowDto.OutstandingBalance ?? 0M) > 0M
                && caseFlowDto.CanMakePayment == false
                && caseFlowDto.CanSetupIndividualPlan == false)
            {
                resultDtoToBeUpdated.AccountStatus = "View Account Details Only";
                resultDtoToBeUpdated.AccountStatusSort = 4;
                resultDtoToBeUpdated.AccountStatusIsViewOnly = true;
                return;
            }

            // No Payment Arrangement
            if ((caseFlowDto.OutstandingBalance ?? 0M) > 0M
                && caseFlowDto.HasArrangement == true)
            {
                resultDtoToBeUpdated.AccountStatus = "Plan in Place";
                resultDtoToBeUpdated.AccountStatusSort = 1;
                resultDtoToBeUpdated.CanManageAccount = true;
                return;
            }

            // Otherwise... Must Plan in Place
            resultDtoToBeUpdated.AccountStatus = "There are no payments set up on this account.";
            resultDtoToBeUpdated.AccountStatusSort = 2;
            resultDtoToBeUpdated.CanManageAccount = true;
        }

        public bool IsWithSolicitors(ICaseFlowAccountCommon caseFlowDto)
        {
            return (caseFlowDto.Company == SolicitorsCompanyCode);
        }
    }
}
