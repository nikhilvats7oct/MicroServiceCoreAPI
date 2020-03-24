using FinancialAccountService.Models.DataTransferObjects;
using FinancialAccountService.Models.Interfaces;
using FinancialAccountService.Processes.Interfaces;
using FinancialAccountService.Services.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace FinancialAccountService.Services
{
    public class MyAccountsService : IMyAccountsService
    {
        private readonly ILogger<MyAccountsService> _logger;
        private readonly IMyAccountsGetProcess _getProcess;
        private readonly IMyAccountsDetailGetProcess _getDetailProcess;
        private readonly IMyAccountDetailGetTransactionsRecentProcess _getTransactionProcess;
        private readonly IMyAccountsStatusDeriver _statusDeriverProcess;

        class AccountComparer : IComparer<MyAccountsSummaryResultDto>
        {
            public int Compare(MyAccountsSummaryResultDto x, MyAccountsSummaryResultDto y)
            {
                Debug.Assert(x != null);
                Debug.Assert(y != null);

                int compareSort = x.AccountStatusSort.CompareTo(y.AccountStatusSort);

                // Fall back on orginal company name if sort values are the same
                // (i.e. sub-sorts by Company Name)
                if (compareSort == 0)
                    return String.Compare(x.OriginalCompany, y.OriginalCompany, StringComparison.CurrentCulture);

                return compareSort;
            }
        }

        public MyAccountsService(ILogger<MyAccountsService> logger,
                                 IMyAccountsGetProcess getProcess,
                                 IMyAccountsDetailGetProcess getDetailProcess,
                                 IMyAccountDetailGetTransactionsRecentProcess getTransactionProcess,
                                 IMyAccountsStatusDeriver statusDeriverProcess)
        {
            _logger = logger;
            _getProcess = getProcess;
            _getDetailProcess = getDetailProcess;
            _getTransactionProcess = getTransactionProcess;
            _statusDeriverProcess = statusDeriverProcess;
        }

        public async Task<MyAccountsResultDto> GetMyAccounts(string userId)
        {
            return await GetMyAccounts(userId, DateTime.Now);
        }

        public async Task<MyAccountsResultDto> GetMyAccountsSummary(string accountId)
        {
            return await GetMyAccountsSummary(accountId, DateTime.Now);
        }

        // Provides interface to allow unit testing without being tied to 'now'
        internal async Task<MyAccountsResultDto> GetMyAccounts(string userId, DateTime currentDateTime)
        {
            CaseFlowMyAccountsDto caseFlowDto = await _getProcess.GetMyAccounts(userId);
            
            MyAccountsResultDto resultDto = new MyAccountsResultDto();
            resultDto.Accounts = new List<MyAccountsSummaryResultDto>();

            foreach (ICaseFlowAccountCommon caseFlowSummaryDto in caseFlowDto.Summaries)
            {
                MyAccountsSummaryResultDto accountDto = new MyAccountsSummaryResultDto();
                PopulateCommonFields(caseFlowSummaryDto, accountDto, currentDateTime);
                
                resultDto.Accounts.Add(accountDto);
            }

            resultDto.Accounts.Sort(new AccountComparer());

            return resultDto;
        }

        internal async Task<MyAccountsResultDto> GetMyAccountsSummary(string accountId, DateTime currentDateTime)
        {
            CaseFlowMyAccountsDto caseFlowDto = await _getProcess.GetMyAccountsSummary(accountId);

            MyAccountsResultDto resultDto = new MyAccountsResultDto();
            resultDto.Accounts = new List<MyAccountsSummaryResultDto>();

            foreach (ICaseFlowAccountCommon caseFlowSummaryDto in caseFlowDto.Summaries)
            {
                MyAccountsSummaryResultDto accountDto = new MyAccountsSummaryResultDto();
                PopulateCommonFields(caseFlowSummaryDto, accountDto, currentDateTime);

                resultDto.Accounts.Add(accountDto);
            }

            resultDto.Accounts.Sort(new AccountComparer());

            return resultDto;
        }

        public async Task<MyAccountsDetailResultDto> GetMyAccountsDetail(string userId, string lowellReference)
        {
            return await GetMyAccountsDetail(userId, lowellReference, DateTime.Now);
        }

        // Provides interface to allow unit testing without being tied to 'now'
        internal async Task<MyAccountsDetailResultDto> GetMyAccountsDetail(string userId, string lowellReference, DateTime currentDateTime)
        {
            CaseFlowMyAccountsDetailDto caseFlowAccount = await _getDetailProcess.GetMyAccountsDetail(lowellReference);
            RecievedTransactionsDto caseFlowTransactions = await _getTransactionProcess.GetTransactionsRecent(lowellReference, 5);

            MyAccountsDetailResultDto resultDto = new MyAccountsDetailResultDto();
            PopulateCommonFields(caseFlowAccount, resultDto, currentDateTime);

            // Fields included in Detail only (not on My Accounts summary)
            resultDto.AccountMessage = caseFlowAccount.ExcludedMessage;

            resultDto.PlanMessages = caseFlowAccount.PlanMessages;

            resultDto.RecentTransactions = new List<MyAccountsDetailResultDto.RecentTransaction>();

            // CaseFlow returns empty content (and 204) if there are no transactions, rather than empty list
            if (caseFlowTransactions != null)
            {
                foreach (PaymentDetails payment in caseFlowTransactions.Payments)
                {
                    MyAccountsDetailResultDto.RecentTransaction transaction =
                        new MyAccountsDetailResultDto.RecentTransaction()
                        {
                            Date = payment.Date,
                            Amount = payment.Amount,
                            Description = payment.Description,
                            Type = payment.Type,
                            RollingBalance = payment.RollingBalance
                        };

                    resultDto.RecentTransactions.Add(transaction);
                }
            }

            return resultDto;
        }

        //
        // Private
        //
        void PopulateCommonFields(ICaseFlowAccountCommon accountDto, MyAccountsSummaryResultDto resultDto, DateTime currentDateTime)
        {
            resultDto.OriginalCompany = accountDto.ClientName;
            resultDto.AccountReference = accountDto.LowellReference;
            resultDto.OutstandingBalance = accountDto.OutstandingBalance ?? 0.0M;
            resultDto.HasArrangement = accountDto.HasArrangement;

            resultDto.DiscountedBalance = DeriveDiscountedBalance(accountDto);

            if (accountDto.HasArrangement && accountDto.OutstandingBalance > 0.0M)
            {
                resultDto.PaymentPlanMethod = accountDto.PlanType;
                resultDto.PaymentPlanFrequency = accountDto.PlanFrequency;
                resultDto.PaymentPlanAmount = accountDto.RegularAccountPaymentAmount;
            }

            // Note: expiry check only uses date part of current date, because the expiry is only a date
            if (accountDto.DiscountAmount != null
                && accountDto.DiscountExpiry != null && currentDateTime.Date <= accountDto.DiscountExpiry
                && !IsDiscountAccepted(accountDto)
                && !_statusDeriverProcess.IsWithSolicitors(accountDto)
                && !(accountDto.HasArrangement && accountDto.PlanType == "Direct Debit") )
            {
                resultDto.DiscountOfferAmount = accountDto.DiscountAmount;
                resultDto.DiscountOfferExpiry = accountDto.DiscountExpiry;
            }

            if (accountDto.Arrears > 0 && accountDto.HasArrangement)
                resultDto.PaymentPlanArrearsAmount = accountDto.Arrears;

            if (accountDto.HasArrangement &&
                (accountDto.PlanType == "Direct Debit"
                || accountDto.PlanType == "Debit Card"
                || accountDto.PlanType == "Credit Card"))
            {
                resultDto.PaymentPlanIsAutomated = true;
            }

            _statusDeriverProcess.DeriveStatusAndStatusSort(accountDto, resultDto);

            // Override 'Can Make Payment' to false, if with solicitors (worked out by status deriver)
            if (resultDto.AccountStatusIsWithSolicitors)
                resultDto.CanMakePayment = false;
            else
                resultDto.CanMakePayment = accountDto.CanMakePayment;

            resultDto.CanAmendPlan = accountDto.CanAmendPlan;
            resultDto.DirectDebitInFlight = accountDto.DirectDebitInFlight;
            resultDto.NextPlanPaymentDate = accountDto.NextPlanPaymentDate;
            resultDto.AddedSinceLastLogin = accountDto.AddedSinceLastLogin;
            resultDto.NeverAllowPlanTransfer = accountDto.NeverAllowPlanTransfer;
            resultDto.PlanPendingTransfer = accountDto.PlanPendingTransfer;
            resultDto.PlanTransferredFrom = accountDto.PlanTransferredFrom;
        }

        decimal? DeriveDiscountedBalance(ICaseFlowAccountCommon caseFlowAccountDto)
        {
            // If balances are equal, indicates no balance
            if (!IsDiscountAccepted(caseFlowAccountDto))
                return null;

            return caseFlowAccountDto.DiscountedBalance;
        }

        bool IsDiscountAccepted(ICaseFlowAccountCommon caseFlowAccountDto)
        {
            return (caseFlowAccountDto.OutstandingBalance != caseFlowAccountDto.DiscountedBalance) && caseFlowAccountDto.HasArrangement;
        }
    }
}
