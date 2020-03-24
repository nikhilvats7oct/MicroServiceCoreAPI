using FinancialAccountService.Models.DataTransferObjects;
using FinancialAccountService.Models.Interfaces;
using FinancialAccountService.Processes;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;

namespace Processes.Unit.Tests
{
    [TestClass]
    public class MyAccountsStatusDeriverTests
    {
        private Mock<ILogger<MyAccountsStatusDeriver>> _mockLogger;
        private MyAccountsStatusDeriver _process;

        class CaseFlowAccountCommonTest : ICaseFlowAccountCommon
        {
            public string ClientName { get; set; }
            public string LowellReference { get; set; }
            public decimal? OutstandingBalance { get; set; }
            public decimal? DiscountAmount { get; set; }
            public DateTime? DiscountExpiry { get; set; }
            public decimal? DiscountedBalance { get; set; }
            public string PlanType { get; set; }
            public string PlanFrequency { get; set; }
            public DateTime? NextPlanPaymentDate { get; set; }
            public decimal? RegularAccountPaymentAmount { get; set; }
            public bool CanMakePayment { get; set; }
            public bool CanSetupIndividualPlan { get; set; }
            public bool HasArrangement { get; set; }
            public decimal? Arrears { get; set; }
            public long? Company { get; set; }
            public bool CanAmendPlan { get; set; }
            public bool DirectDebitInFlight { get; set; }
            public bool IsClosedHidden { get; set; }
            public bool AddedSinceLastLogin { get; set; }
            public bool NeverAllowPlanTransfer { get; set; }
            public bool PlanPendingTransfer { get; set; }
            public string PlanTransferredFrom { get; set; }
        }

        [TestInitialize]
        public void Initialise()
        {
            _mockLogger = new Mock<ILogger<MyAccountsStatusDeriver>>();
            _process = new MyAccountsStatusDeriver(_mockLogger.Object);
        }


        // Expected sort order
        // from User Acceptance Criteria:
        // (Plan In Place,No payment arrangement,With Lowell Solicitors,View account details only,Closed)
        private const int StatusSort_PlanInPlace = 1;
        private const int StatusSort_NoPaymentArrangement = 2;
        private const int StatusSort_WithLowellSolicitors = 3;
        private const int StatusSort_ViewAccountDetailsOnly = 4;
        private const int StatusSort_Closed = 5;


        [TestMethod]

        [DataRow(null, null)]
        [DataRow(0D, null)]
        [DataRow(-0.1D, null)]
        [DataRow(-1D, null)]
        [DataRow(-2D, null)]
        [DataRow(-1000D, null)]

        // Company code makes no difference. If at solicitors, still show closed.
        [DataRow(0D, 2L)]

        public void DeriveStatusAndStatusSort_WhenCaseFlowDtoPropertiesIndicate_Closed_SetsStatusAndStatusName_On_ServiceDto(
            double? testOutstandingBalance, long? testCompany)
        {
            // Note: using double because 'decimal' can't be used in DataRow

            CaseFlowAccountCommonTest caseFlowDto = new CaseFlowAccountCommonTest()
            {
                OutstandingBalance = (decimal?)testOutstandingBalance,

                CanSetupIndividualPlan = false,
                CanMakePayment = false,

                // Should not be affected by company code
                Company = testCompany
            };

            MyAccountsSummaryResultDto resultDtoToBeUpdated = new MyAccountsSummaryResultDto();

            _process.DeriveStatusAndStatusSort(caseFlowDto, resultDtoToBeUpdated);

            Assert.AreEqual("Closed", resultDtoToBeUpdated.AccountStatus);
            Assert.AreEqual(StatusSort_Closed, resultDtoToBeUpdated.AccountStatusSort);

            Assert.AreEqual(true, resultDtoToBeUpdated.AccountStatusIsClosed);
            Assert.AreEqual(false, resultDtoToBeUpdated.AccountStatusIsWithSolicitors);
            Assert.AreEqual(false, resultDtoToBeUpdated.AccountStatusIsViewOnly);

            Assert.AreEqual(false, resultDtoToBeUpdated.CanManageAccount);
        }

        [TestMethod]

        // Test one criteria not matching
        [DataRow(0.1D, false, false)]   // has balance

        [DataRow(0D, true, false)]      // } has one flag set
        [DataRow(0D, false, true)]

        // All not matching
        [DataRow(0.1D, true, true)]

        // Combinations (two not matching)
        [DataRow(0.1D, true, false)]
        [DataRow(0.1D, false, true)]
        [DataRow(0D, true, true)]

        public void DeriveStatusAndStatusSort_WhenCaseFlowDtoPropertiesIndicate_NOT_Closed_SetsAnotherStatusAndStatusName_On_ServiceDto(
            double? testOutstandingBalance, bool testCanSetupIndividualPlan, bool testCanMakePayment)
        {
            // Note: using double because 'decimal' can't be used in DataRow

            CaseFlowAccountCommonTest caseFlowDto = new CaseFlowAccountCommonTest()
            {
                OutstandingBalance = (decimal?)testOutstandingBalance,

                CanSetupIndividualPlan = testCanSetupIndividualPlan,
                CanMakePayment = testCanMakePayment
            };

            MyAccountsSummaryResultDto resultDtoToBeUpdated = new MyAccountsSummaryResultDto();

            _process.DeriveStatusAndStatusSort(caseFlowDto, resultDtoToBeUpdated);

            // Checks NOT at status
            Assert.AreNotEqual("Closed", resultDtoToBeUpdated.AccountStatus);
            Assert.AreNotEqual(StatusSort_Closed, resultDtoToBeUpdated.AccountStatusSort);

            Assert.AreEqual(false, resultDtoToBeUpdated.AccountStatusIsClosed);
        }

        [TestMethod]
        public void DeriveStatusAndStatusSort_WhenCaseFlowDtoPropertiesIndicate_WithSolicitors_SetsStatusAndStatusName_On_ServiceDto()
        {
            CaseFlowAccountCommonTest caseFlowDto = new CaseFlowAccountCommonTest()
            {
                Company = 2,  // *** IMPORTANT: if change method to company number, must also change CLOSED test

                // Is superceded by 'Closed' therefore requires balance
                // (not exhaustively testing this because 'Closed' tests check with company code 2)
                OutstandingBalance = 0.1M
            };

            MyAccountsSummaryResultDto resultDtoToBeUpdated = new MyAccountsSummaryResultDto();

            _process.DeriveStatusAndStatusSort(caseFlowDto, resultDtoToBeUpdated);

            Assert.AreEqual("With Lowell Solicitors", resultDtoToBeUpdated.AccountStatus);
            Assert.AreEqual(StatusSort_WithLowellSolicitors, resultDtoToBeUpdated.AccountStatusSort);

            Assert.AreEqual(false, resultDtoToBeUpdated.AccountStatusIsClosed);
            Assert.AreEqual(true, resultDtoToBeUpdated.AccountStatusIsWithSolicitors);
            Assert.AreEqual(false, resultDtoToBeUpdated.AccountStatusIsViewOnly);

            Assert.AreEqual(false, resultDtoToBeUpdated.CanManageAccount);
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow(0L)]
        [DataRow(1L)]
        [DataRow(3L)]
        [DataRow(1000L)]
        [DataRow(-1L)]
        public void DeriveStatusAndStatusSort_WhenCaseFlowDtoPropertiesIndicate_NOT_WithSolicitors_SetsAnotherStatusAndStatusName_On_ServiceDto(
            long? testCompany)
        {
            CaseFlowAccountCommonTest caseFlowDto = new CaseFlowAccountCommonTest()
            {
                Company = testCompany,      // *** IMPORTANT: if change method to company number, must also change CLOSED test

                // Is superceded by 'Closed' therefore requires balance
                // (not exhaustively testing this because 'Closed' tests check with company code 2)
                OutstandingBalance = 0.1M
            };

            MyAccountsSummaryResultDto resultDtoToBeUpdated = new MyAccountsSummaryResultDto();

            _process.DeriveStatusAndStatusSort(caseFlowDto, resultDtoToBeUpdated);

            // Checks NOT at status
            Assert.AreNotEqual("With Lowell Solicitors", resultDtoToBeUpdated.AccountStatus);
            Assert.AreNotEqual(StatusSort_WithLowellSolicitors, resultDtoToBeUpdated.AccountStatusSort);

            Assert.AreEqual(false, resultDtoToBeUpdated.AccountStatusIsWithSolicitors);
        }

        [TestMethod]

        [DataRow(0.1D)]
        [DataRow(1D)]
        [DataRow(2D)]
        [DataRow(1000D)]
        public void DeriveStatusAndStatusSort_WhenCaseFlowDtoPropertiesIndicate_ViewAccountOnly_SetsStatusAndStatusName_On_ServiceDto(
            double? testOutstandingBalance)
        {
            // Note: using double because 'decimal' can't be used in DataRow

            CaseFlowAccountCommonTest caseFlowDto = new CaseFlowAccountCommonTest()
            {
                OutstandingBalance = (decimal?)testOutstandingBalance,

                CanSetupIndividualPlan = false,
                CanMakePayment = false
            };

            MyAccountsSummaryResultDto resultDtoToBeUpdated = new MyAccountsSummaryResultDto();

            _process.DeriveStatusAndStatusSort(caseFlowDto, resultDtoToBeUpdated);

            Assert.AreEqual("View Account Details Only", resultDtoToBeUpdated.AccountStatus);
            Assert.AreEqual(StatusSort_ViewAccountDetailsOnly, resultDtoToBeUpdated.AccountStatusSort);

            Assert.AreEqual(false, resultDtoToBeUpdated.AccountStatusIsClosed);
            Assert.AreEqual(false, resultDtoToBeUpdated.AccountStatusIsWithSolicitors);
            Assert.AreEqual(true, resultDtoToBeUpdated.AccountStatusIsViewOnly);

            Assert.AreEqual(false, resultDtoToBeUpdated.CanManageAccount);
        }

        [TestMethod]

        // Test one criteria not matching
        [DataRow(null, false, false)]   // } no balance
        [DataRow(0D, false, false)]

        [DataRow(0.1D, true, false)]
        [DataRow(0.1D, false, true)]    // } has one flag set

        // All not matching
        [DataRow(0D, true, true)]

        // Combinations (two not matching)
        [DataRow(0D, true, false)]
        [DataRow(0D, false, true)]

        [DataRow(0.1D, true, true)]

        public void DeriveStatusAndStatusSort_WhenCaseFlowDtoPropertiesIndicate_NOT_ViewAccountOnly_SetsAnotherStatusAndStatusName_On_ServiceDto(
            double? testOutstandingBalance, bool testCanSetupIndividualPlan, bool testCanMakePayment)
        {
            // Note: using double because 'decimal' can't be used in DataRow

            CaseFlowAccountCommonTest caseFlowDto = new CaseFlowAccountCommonTest()
            {
                OutstandingBalance = (decimal?)testOutstandingBalance,

                CanSetupIndividualPlan = testCanSetupIndividualPlan,
                CanMakePayment = testCanMakePayment
            };

            MyAccountsSummaryResultDto resultDtoToBeUpdated = new MyAccountsSummaryResultDto();

            _process.DeriveStatusAndStatusSort(caseFlowDto, resultDtoToBeUpdated);

            // Checks NOT at status
            Assert.AreNotEqual("View Account Details Only", resultDtoToBeUpdated.AccountStatus);
            Assert.AreNotEqual(StatusSort_ViewAccountDetailsOnly, resultDtoToBeUpdated.AccountStatusSort);

            Assert.AreEqual(false, resultDtoToBeUpdated.AccountStatusIsViewOnly);
        }

        [TestMethod]

        [DataRow(0.1D)]
        [DataRow(1D)]
        [DataRow(2D)]
        [DataRow(1000D)]

        public void DeriveStatusAndStatusSort_WhenCaseFlowDtoPropertiesIndicate_PlanInPlace_SetsStatusAndStatusName_On_ServiceDto(
            double? testOutstandingBalance)
        {
            // Note: using double because 'decimal' can't be used in DataRow

            CaseFlowAccountCommonTest caseFlowDto = new CaseFlowAccountCommonTest()
            {
                OutstandingBalance = (decimal?)testOutstandingBalance,

                HasArrangement = true,

                CanMakePayment = true       // must set this or 'can set up plan' to avoid falling into View Only
            };

            MyAccountsSummaryResultDto resultDtoToBeUpdated = new MyAccountsSummaryResultDto();

            _process.DeriveStatusAndStatusSort(caseFlowDto, resultDtoToBeUpdated);

            Assert.AreEqual("Plan in Place", resultDtoToBeUpdated.AccountStatus);
            Assert.AreEqual(StatusSort_PlanInPlace, resultDtoToBeUpdated.AccountStatusSort);

            Assert.AreEqual(false, resultDtoToBeUpdated.AccountStatusIsClosed);
            Assert.AreEqual(false, resultDtoToBeUpdated.AccountStatusIsWithSolicitors);
            Assert.AreEqual(false, resultDtoToBeUpdated.AccountStatusIsViewOnly);

            Assert.AreEqual(true, resultDtoToBeUpdated.CanManageAccount);
        }

        [TestMethod]

        // Test one criteria not matching 
        [DataRow(0.1D, false)]
        [DataRow(0.1D, null)]

        [DataRow(0D, true)]
        [DataRow(null, true)]

        // Both criteria not matching
        [DataRow(0D, false)]
        [DataRow(null, false)]

        public void DeriveStatusAndStatusSort_WhenCaseFlowDtoPropertiesIndicate_NOT_PlanInPlace_SetsAnotherStatusAndStatusName_On_ServiceDto(
            double? testOutstandingBalance, bool testIsPlanInPlace)
        {
            // Note: using double because 'decimal' can't be used in DataRow

            CaseFlowAccountCommonTest caseFlowDto = new CaseFlowAccountCommonTest()
            {
                OutstandingBalance = (decimal?)testOutstandingBalance,

                HasArrangement = testIsPlanInPlace,

                CanMakePayment = true       // must set this or 'can set up plan' to avoid falling into View Only
            };

            MyAccountsSummaryResultDto resultDtoToBeUpdated = new MyAccountsSummaryResultDto();

            _process.DeriveStatusAndStatusSort(caseFlowDto, resultDtoToBeUpdated);

            // Checks NOT at status
            Assert.AreNotEqual("Plan in Place", resultDtoToBeUpdated.AccountStatus);
            Assert.AreNotEqual(StatusSort_PlanInPlace, resultDtoToBeUpdated.AccountStatusSort);
        }

        [TestMethod]

        [DataRow(0.1D, false)]      // As per acceptance criteria: has balance, no plan
        [DataRow(1D, false)]
        [DataRow(2D, false)]
        [DataRow(1000D, false)]

        // Other scenarios also fall into this, because they don't match other statuses
        // In practice these should not be valid combinations in CaseFlow
        [DataRow(0D, false)]        // becasue CanMakePayment=true in test, does not go in closed or view only
        [DataRow(null, false)]
        [DataRow(0D, true)]

        public void DeriveStatusAndStatusSort_WhenCaseFlowDtoPropertiesIndicate_NoPaymentArrangement_SetsStatusAndStatusName_On_ServiceDto(
            double? testOutstandingBalance, bool testIsPlanInPlace)
        {
            // Note: using double because 'decimal' can't be used in DataRow

            CaseFlowAccountCommonTest caseFlowDto = new CaseFlowAccountCommonTest()
            {
                OutstandingBalance = (decimal?)testOutstandingBalance,

                HasArrangement = testIsPlanInPlace,

                CanMakePayment = true       // must set this or 'can set up plan' to avoid falling into View Only
            };

            MyAccountsSummaryResultDto resultDtoToBeUpdated = new MyAccountsSummaryResultDto();

            _process.DeriveStatusAndStatusSort(caseFlowDto, resultDtoToBeUpdated);

            Assert.AreEqual("There are no payments set up on this account.", resultDtoToBeUpdated.AccountStatus);
            Assert.AreEqual(StatusSort_NoPaymentArrangement, resultDtoToBeUpdated.AccountStatusSort);

            Assert.AreEqual(false, resultDtoToBeUpdated.AccountStatusIsClosed);
            Assert.AreEqual(false, resultDtoToBeUpdated.AccountStatusIsWithSolicitors);
            Assert.AreEqual(false, resultDtoToBeUpdated.AccountStatusIsViewOnly);

            Assert.AreEqual(true, resultDtoToBeUpdated.CanManageAccount);
        }

        [TestMethod]

        // Test one criteria not matching (plan in place)
        [DataRow(0.1D, true)]

        public void DeriveStatusAndStatusSort_WhenCaseFlowDtoPropertiesIndicate_NOT_NoPaymentArrangement_SetsAnotherStatusAndStatusName_On_ServiceDto(
            double? testOutstandingBalance, bool testIsPlanInPlace)
        {
            // Note: using double because 'decimal' can't be used in DataRow

            CaseFlowAccountCommonTest caseFlowDto = new CaseFlowAccountCommonTest()
            {
                OutstandingBalance = (decimal?)testOutstandingBalance,

                HasArrangement = testIsPlanInPlace,

                CanMakePayment = true       // must set this or 'can set up plan' to avoid falling into View Only
            };

            MyAccountsSummaryResultDto resultDtoToBeUpdated = new MyAccountsSummaryResultDto();

            _process.DeriveStatusAndStatusSort(caseFlowDto, resultDtoToBeUpdated);

            // Checks NOT at status
            Assert.AreNotEqual("No Payment Arrangement", resultDtoToBeUpdated.AccountStatus);
            Assert.AreNotEqual(StatusSort_NoPaymentArrangement, resultDtoToBeUpdated.AccountStatusSort);
        }

        [TestMethod]
        public void IsWithSolicitors_WhenCaseFlowDtoCompanyIs2_ReturnsTrue()
        {
            CaseFlowAccountCommonTest caseFlowDto = new CaseFlowAccountCommonTest()
            {
                Company = 2
            };

            Assert.AreEqual(true, _process.IsWithSolicitors(caseFlowDto));
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow(1L)]
        [DataRow(3L)]
        [DataRow(999L)]
        public void IsWithSolicitors_WhenCaseFlowDtoCompanyIsNot2_ReturnsTrue(long? testCompany)
        {
            CaseFlowAccountCommonTest caseFlowDto = new CaseFlowAccountCommonTest()
            {
                Company = testCompany
            };

            Assert.AreEqual(false, _process.IsWithSolicitors(caseFlowDto));
        }
    }
}
