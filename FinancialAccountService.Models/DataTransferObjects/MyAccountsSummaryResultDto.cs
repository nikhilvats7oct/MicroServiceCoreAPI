using System;

namespace FinancialAccountService.Models.DataTransferObjects
{
    public class MyAccountsSummaryResultDto
    {
        public string OriginalCompany { get; set; }
        public string AccountReference { get; set; }
        public decimal OutstandingBalance { get; set; }
        public decimal? DiscountedBalance { get; set; }
        public string AccountStatus { get; set; }
        public int AccountStatusSort { get; set; }

        public bool AccountStatusIsClosed { get; set; }     // indicates that account should be formatted as closed
        public bool AccountStatusIsWithSolicitors { get; set; }
        public bool AccountStatusIsViewOnly { get; set; }

        public bool CanMakePayment { get; set; }
        public bool CanAmendPlan { get; set; }
        public bool CanManageAccount { get; set; }
        public bool DirectDebitInFlight { get; set; }

        // Will all be null if payment plan not in place
        public decimal? PaymentPlanAmount { get; set; }
        public string PaymentPlanFrequency { get; set; }
        public string PaymentPlanMethod { get; set; }
        public DateTime? NextPlanPaymentDate { get; set; }

        // Will all be null if no discount offer available
        public decimal? DiscountOfferAmount { get; set; }
        public DateTime? DiscountOfferExpiry { get; set; }

        public decimal? PaymentPlanArrearsAmount { get; set; }         // will be null if not in arrears or does not have payment plan
        public bool PaymentPlanIsAutomated { get; set; }
        public bool AddedSinceLastLogin { get; set; }
        public bool HasArrangement { get; set; }
        public bool NeverAllowPlanTransfer { get; set; }
        public bool PlanPendingTransfer { get; set; }
        public string PlanTransferredFrom { get; set; }
    }
}
