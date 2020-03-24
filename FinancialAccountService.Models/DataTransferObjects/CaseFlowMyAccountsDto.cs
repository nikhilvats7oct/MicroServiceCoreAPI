﻿using System;
using System.Collections.Generic;
using FinancialAccountService.Models.Interfaces;

namespace FinancialAccountService.Models.DataTransferObjects
{
    public class CaseFlowMyAccountsDto
    {
        public class AccountSummaryDto : ICaseFlowAccountCommon
        {
            public string LowellReference { get; set; }
            public string ClientName { get; set; }
            public decimal? OutstandingBalance { get; set; }
            public decimal? DiscountedBalance { get; set; }
            public decimal? Arrears { get; set; }
            public bool CanSetupIndividualPlan { get; set; }
            public bool CanMakePayment { get; set; }
            public decimal? RegularAccountPaymentAmount { get; set; }
            public bool HasArrangement { get; set; }
            public string PlanType { get; set; }
            public string PlanFrequency { get; set; }
            public DateTime? NextPlanPaymentDate { get; set; }
            public decimal? DiscountAmount { get; set; }
            public DateTime? DiscountExpiry { get; set; }
            public long? Company { get; set; }
            public bool CanAmendPlan { get;set; }
            public bool DirectDebitInFlight { get; set; }
            public bool IsClosedHidden { get; set; }
            public bool AddedSinceLastLogin { get; set; }
            public bool NeverAllowPlanTransfer { get; set; }
            public bool PlanPendingTransfer { get; set; }
            public string PlanTransferredFrom { get; set; }
        }

        public List<AccountSummaryDto> Summaries { get; set; }
    }
}
