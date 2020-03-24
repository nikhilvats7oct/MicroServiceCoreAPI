using System;

namespace FinancialAccountService.Models.Interfaces
{
    // Common fields used in acount summary and detail
    public interface ICaseFlowAccountCommon
    {
        string ClientName { get; set; }
        string LowellReference { get; set; }
        decimal? OutstandingBalance { get; set; }
        decimal? DiscountAmount { get; set; }
        DateTime? DiscountExpiry { get; set; }
        decimal? DiscountedBalance { get; set; }
        decimal? Arrears { get; set; }
        string PlanType { get; set; }
        string PlanFrequency { get; set; }
        DateTime? NextPlanPaymentDate { get; set; }
        decimal? RegularAccountPaymentAmount { get; set; }
        bool CanMakePayment { get; set; }
        bool CanSetupIndividualPlan { get; set; }
        bool CanAmendPlan { get; set; }
        bool HasArrangement { get; set; }
        long? Company { get; set; }
        bool DirectDebitInFlight { get; set; }
        bool IsClosedHidden { get; set; }
        bool AddedSinceLastLogin { get; set; }
        bool NeverAllowPlanTransfer { get; set; }
        bool PlanPendingTransfer { get; set; }
        string PlanTransferredFrom { get; set; }
    }
}
