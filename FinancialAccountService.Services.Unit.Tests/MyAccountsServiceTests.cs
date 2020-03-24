using FinancialAccountService.Models.DataTransferObjects;
using FinancialAccountService.Models.Interfaces;
using FinancialAccountService.Processes;
using FinancialAccountService.Processes.Interfaces;
using FinancialAccountService.Services;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Threading.Tasks;

namespace Services.Unit.Tests
{
    [TestClass]
    public class MyAccountsServiceTests
    {
        private Mock<ILogger<MyAccountsService>> _mockLogger;
        private Mock<ILogger<MyAccountsStatusDeriver>> _mockMyAccountsStatusDeriverlogger;
        private Mock<IMyAccountsGetProcess> _mockGetProcess;
        private Mock<IMyAccountsDetailGetProcess> _mockGetDetailsProcess;
        private Mock<IMyAccountDetailGetTransactionsRecentProcess> _mockMyAccountDetailGetTransactionsRecentProcess;

        private Mock<IMyAccountsStatusDeriver> _mockStatusDeriverProcess;

        private MyAccountsService _service;

        [TestInitialize]
        public void Initialise()
        {
            _mockLogger = new Mock<ILogger<MyAccountsService>>();
            _mockMyAccountsStatusDeriverlogger = new Mock<ILogger<MyAccountsStatusDeriver>>();
            _mockGetProcess = new Mock<IMyAccountsGetProcess>(MockBehavior.Strict);
            _mockGetDetailsProcess = new Mock<IMyAccountsDetailGetProcess>(MockBehavior.Strict);
            _mockMyAccountDetailGetTransactionsRecentProcess = new Mock<IMyAccountDetailGetTransactionsRecentProcess>();

            _mockStatusDeriverProcess = new Mock<IMyAccountsStatusDeriver>();

            // Default recent transactions to return empty list
            _mockMyAccountDetailGetTransactionsRecentProcess.Setup(
                x => x.GetTransactionsRecent(It.IsAny<String>(), It.IsAny<uint>()))
                .Returns(Task.FromResult(new RecievedTransactionsDto()));

            _service = new MyAccountsService(_mockLogger.Object, _mockGetProcess.Object, _mockGetDetailsProcess.Object, _mockMyAccountDetailGetTransactionsRecentProcess.Object, _mockStatusDeriverProcess.Object);
        }

        [TestMethod]
        public void GetMyAccounts_WhenProvidedUserId_RetrievesAccounts_And_OutputsTransformedModels()
        {
            // Arrange
            const string userId = "Anything456";

            CaseFlowMyAccountsDto caseFlowDto = new CaseFlowMyAccountsDto()
            {
                Summaries = new List<CaseFlowMyAccountsDto.AccountSummaryDto>()
                {
                    new CaseFlowMyAccountsDto.AccountSummaryDto()
                    {
                        LowellReference = "1234",
                        ClientName = "BarclayBob",
                        OutstandingBalance = 1234.56M,
                        CanMakePayment = false,

                        HasArrangement = true,
                        PlanType = "Direct Debit",
                        PlanFrequency = "Monthly",
                        RegularAccountPaymentAmount = 123.45M
                    },

                    new CaseFlowMyAccountsDto.AccountSummaryDto()
                    {
                        LowellReference = "abc345",
                        ClientName = "Wah",
                        OutstandingBalance = 101.22M,
                        CanMakePayment = true,

                        // Ensure works with nulls
                        HasArrangement = false,
                        PlanType = null,
                        PlanFrequency = null,
                        RegularAccountPaymentAmount = null
                    },
                }
            };

            _mockGetProcess.Setup(x => x.GetMyAccounts(userId)).Returns(Task.FromResult(caseFlowDto));

            // Act
            MyAccountsResultDto resultDto = _service.GetMyAccounts(userId).Result;

            // Assert
            Assert.AreEqual(2, resultDto.Accounts.Count);
            {
                MyAccountsSummaryResultDto account = resultDto.Accounts[0];
                Assert.AreEqual("1234", account.AccountReference);
                Assert.AreEqual("BarclayBob", account.OriginalCompany);
                Assert.AreEqual(1234.56M, account.OutstandingBalance);
                Assert.AreEqual(false, account.CanMakePayment);

                Assert.AreEqual("Direct Debit", account.PaymentPlanMethod);
                Assert.AreEqual("Monthly", account.PaymentPlanFrequency);
                Assert.AreEqual(123.45M, account.PaymentPlanAmount);
            }
            {
                MyAccountsSummaryResultDto account = resultDto.Accounts[1];
                Assert.AreEqual("abc345", account.AccountReference);
                Assert.AreEqual("Wah", account.OriginalCompany);
                Assert.AreEqual(101.22M, account.OutstandingBalance);
                Assert.AreEqual(true, account.CanMakePayment);

                Assert.AreEqual(null, account.PaymentPlanMethod);
                Assert.AreEqual(null, account.PaymentPlanFrequency);
                Assert.AreEqual(null, account.PaymentPlanAmount);
            }
        }

        [TestMethod]
        public void GetMyAccounts_WhenProvidedUserId_RetrievesAccounts_CallsStatusDeriver_And_OrdersCorrectlyByStatusSortThenAccountName()
        {
            // Note: using mock Status Deriver, so as to avoid exhaustively testing statuses here, as responsibility
            // for deriving statuses is with concrete Status Deriver class

            // Mock will apply some status sort (and status) values, allowing testing of sort / sub-sort behaviour
            // in service class under test

            _mockStatusDeriverProcess.Setup(x => x.DeriveStatusAndStatusSort(
                    It.IsAny<ICaseFlowAccountCommon>(),
                    It.IsAny<MyAccountsSummaryResultDto>()))
                .Callback<ICaseFlowAccountCommon, MyAccountsSummaryResultDto>(
                    (caseFlowAccountDto, resultDto) =>
                    {
                        // Status deriver places 'Company' (long integer) field into sort field to facilitate testing
                        Debug.Assert(caseFlowAccountDto.Company != null);
                        resultDto.AccountStatusSort = (int)caseFlowAccountDto.Company.Value;
                    });

            // Get returns some CaseFlow dto's with mixed up sort ordering
            // Ordering is by AccountStatusSort then ClientName
            CaseFlowMyAccountsDto caseFlowDto = new CaseFlowMyAccountsDto()
            {
                Summaries = new List<CaseFlowMyAccountsDto.AccountSummaryDto>()
                {
                    new CaseFlowMyAccountsDto.AccountSummaryDto() { Company = 4, ClientName = "Def" },
                    new CaseFlowMyAccountsDto.AccountSummaryDto() { Company = 100, ClientName = "jjj" },
                    new CaseFlowMyAccountsDto.AccountSummaryDto() { Company = 3, ClientName = "1a" },
                    new CaseFlowMyAccountsDto.AccountSummaryDto() { Company = 1, ClientName = "zzz" },
                    new CaseFlowMyAccountsDto.AccountSummaryDto() { Company = 4, ClientName = "Ghi" },
                    new CaseFlowMyAccountsDto.AccountSummaryDto() { Company = 3, ClientName = "1b" },
                    new CaseFlowMyAccountsDto.AccountSummaryDto() { Company = 4, ClientName = "Abc" },
                    new CaseFlowMyAccountsDto.AccountSummaryDto() { Company = 2, ClientName = "wibble" },
                }
            };
            _mockGetProcess.Setup(x => x.GetMyAccounts("anything")).Returns(Task.FromResult(caseFlowDto));

            //
            // Check output is correctly ordered
            //
            MyAccountsResultDto outputDto = _service.GetMyAccounts("anything").Result;
            Assert.AreEqual(8, outputDto.Accounts.Count);

            // Sort Value 1
            {
                MyAccountsSummaryResultDto accountDto = outputDto.Accounts[0];
                Assert.AreEqual("zzz", accountDto.OriginalCompany);
                Assert.AreEqual(1, accountDto.AccountStatusSort);
            }

            // Sort Value 2
            {
                MyAccountsSummaryResultDto accountDto = outputDto.Accounts[1];
                Assert.AreEqual("wibble", accountDto.OriginalCompany);
                Assert.AreEqual(2, accountDto.AccountStatusSort);
            }

            // Sort Value 3
            {
                MyAccountsSummaryResultDto accountDto = outputDto.Accounts[2];
                Assert.AreEqual("1a", accountDto.OriginalCompany);
                Assert.AreEqual(3, accountDto.AccountStatusSort);
            }
            {
                MyAccountsSummaryResultDto accountDto = outputDto.Accounts[3];
                Assert.AreEqual("1b", accountDto.OriginalCompany);
                Assert.AreEqual(3, accountDto.AccountStatusSort);
            }

            // Sort Value 4
            {
                MyAccountsSummaryResultDto accountDto = outputDto.Accounts[4];
                Assert.AreEqual("Abc", accountDto.OriginalCompany);
                Assert.AreEqual(4, accountDto.AccountStatusSort);
            }
            {
                MyAccountsSummaryResultDto accountDto = outputDto.Accounts[5];
                Assert.AreEqual("Def", accountDto.OriginalCompany);
                Assert.AreEqual(4, accountDto.AccountStatusSort);
            }
            {
                MyAccountsSummaryResultDto accountDto = outputDto.Accounts[6];
                Assert.AreEqual("Ghi", accountDto.OriginalCompany);
                Assert.AreEqual(4, accountDto.AccountStatusSort);
            }

            // Sort Value 100
            {
                MyAccountsSummaryResultDto accountDto = outputDto.Accounts[7];
                Assert.AreEqual("jjj", accountDto.OriginalCompany);
                Assert.AreEqual(100, accountDto.AccountStatusSort);
            }
        }

        [TestMethod]
        public void GetMyAccountsSummary_WhenProvidedAccountId_RetrievesAccounts_And_OutputsTransformedModels()
        {
            // Arrange
            const string accountId = "Anything456";

            CaseFlowMyAccountsDto caseFlowDto = new CaseFlowMyAccountsDto()
            {
                Summaries = new List<CaseFlowMyAccountsDto.AccountSummaryDto>()
                {
                    new CaseFlowMyAccountsDto.AccountSummaryDto()
                    {
                        LowellReference = "1234",
                        ClientName = "BarclayBob",
                        OutstandingBalance = 1234.56M,
                        CanMakePayment = false,

                        HasArrangement = true,
                        PlanType = "Direct Debit",
                        PlanFrequency = "Monthly",
                        RegularAccountPaymentAmount = 123.45M
                    },

                    new CaseFlowMyAccountsDto.AccountSummaryDto()
                    {
                        LowellReference = "abc345",
                        ClientName = "Wah",
                        OutstandingBalance = 101.22M,
                        CanMakePayment = true,

                        // Ensure works with nulls
                        HasArrangement = false,
                        PlanType = null,
                        PlanFrequency = null,
                        RegularAccountPaymentAmount = null
                    },
                }
            };

            _mockGetProcess.Setup(x => x.GetMyAccountsSummary(accountId)).Returns(Task.FromResult(caseFlowDto));

            // Act
            MyAccountsResultDto resultDto = _service.GetMyAccountsSummary(accountId).Result;

            // Assert
            Assert.AreEqual(2, resultDto.Accounts.Count);
            {
                MyAccountsSummaryResultDto account = resultDto.Accounts[0];
                Assert.AreEqual("1234", account.AccountReference);
                Assert.AreEqual("BarclayBob", account.OriginalCompany);
                Assert.AreEqual(1234.56M, account.OutstandingBalance);
                Assert.AreEqual(false, account.CanMakePayment);

                Assert.AreEqual("Direct Debit", account.PaymentPlanMethod);
                Assert.AreEqual("Monthly", account.PaymentPlanFrequency);
                Assert.AreEqual(123.45M, account.PaymentPlanAmount);
            }
            {
                MyAccountsSummaryResultDto account = resultDto.Accounts[1];
                Assert.AreEqual("abc345", account.AccountReference);
                Assert.AreEqual("Wah", account.OriginalCompany);
                Assert.AreEqual(101.22M, account.OutstandingBalance);
                Assert.AreEqual(true, account.CanMakePayment);

                Assert.AreEqual(null, account.PaymentPlanMethod);
                Assert.AreEqual(null, account.PaymentPlanFrequency);
                Assert.AreEqual(null, account.PaymentPlanAmount);
            }
        }

        [TestMethod]
        public void GetMyAccountsSummary_WhenProvidedAccountId_RetrievesAccounts_CallsStatusDeriver_And_OrdersCorrectlyByStatusSortThenAccountName()
        {
            // Note: using mock Status Deriver, so as to avoid exhaustively testing statuses here, as responsibility
            // for deriving statuses is with concrete Status Deriver class

            // Mock will apply some status sort (and status) values, allowing testing of sort / sub-sort behaviour
            // in service class under test

            _mockStatusDeriverProcess.Setup(x => x.DeriveStatusAndStatusSort(
                    It.IsAny<ICaseFlowAccountCommon>(),
                    It.IsAny<MyAccountsSummaryResultDto>()))
                .Callback<ICaseFlowAccountCommon, MyAccountsSummaryResultDto>(
                    (caseFlowAccountDto, resultDto) =>
                    {
                        // Status deriver places 'Company' (long integer) field into sort field to facilitate testing
                        Debug.Assert(caseFlowAccountDto.Company != null);
                        resultDto.AccountStatusSort = (int)caseFlowAccountDto.Company.Value;
                    });

            // Get returns some CaseFlow dto's with mixed up sort ordering
            // Ordering is by AccountStatusSort then ClientName
            CaseFlowMyAccountsDto caseFlowDto = new CaseFlowMyAccountsDto()
            {
                Summaries = new List<CaseFlowMyAccountsDto.AccountSummaryDto>()
                {
                    new CaseFlowMyAccountsDto.AccountSummaryDto() { Company = 4, ClientName = "Def" },
                    new CaseFlowMyAccountsDto.AccountSummaryDto() { Company = 100, ClientName = "jjj" },
                    new CaseFlowMyAccountsDto.AccountSummaryDto() { Company = 3, ClientName = "1a" },
                    new CaseFlowMyAccountsDto.AccountSummaryDto() { Company = 1, ClientName = "zzz" },
                    new CaseFlowMyAccountsDto.AccountSummaryDto() { Company = 4, ClientName = "Ghi" },
                    new CaseFlowMyAccountsDto.AccountSummaryDto() { Company = 3, ClientName = "1b" },
                    new CaseFlowMyAccountsDto.AccountSummaryDto() { Company = 4, ClientName = "Abc" },
                    new CaseFlowMyAccountsDto.AccountSummaryDto() { Company = 2, ClientName = "wibble" },
                }
            };
            _mockGetProcess.Setup(x => x.GetMyAccountsSummary("anything")).Returns(Task.FromResult(caseFlowDto));

            //
            // Check output is correctly ordered
            //
            MyAccountsResultDto outputDto = _service.GetMyAccountsSummary("anything").Result;
            Assert.AreEqual(8, outputDto.Accounts.Count);

            // Sort Value 1
            {
                MyAccountsSummaryResultDto accountDto = outputDto.Accounts[0];
                Assert.AreEqual("zzz", accountDto.OriginalCompany);
                Assert.AreEqual(1, accountDto.AccountStatusSort);
            }

            // Sort Value 2
            {
                MyAccountsSummaryResultDto accountDto = outputDto.Accounts[1];
                Assert.AreEqual("wibble", accountDto.OriginalCompany);
                Assert.AreEqual(2, accountDto.AccountStatusSort);
            }

            // Sort Value 3
            {
                MyAccountsSummaryResultDto accountDto = outputDto.Accounts[2];
                Assert.AreEqual("1a", accountDto.OriginalCompany);
                Assert.AreEqual(3, accountDto.AccountStatusSort);
            }
            {
                MyAccountsSummaryResultDto accountDto = outputDto.Accounts[3];
                Assert.AreEqual("1b", accountDto.OriginalCompany);
                Assert.AreEqual(3, accountDto.AccountStatusSort);
            }

            // Sort Value 4
            {
                MyAccountsSummaryResultDto accountDto = outputDto.Accounts[4];
                Assert.AreEqual("Abc", accountDto.OriginalCompany);
                Assert.AreEqual(4, accountDto.AccountStatusSort);
            }
            {
                MyAccountsSummaryResultDto accountDto = outputDto.Accounts[5];
                Assert.AreEqual("Def", accountDto.OriginalCompany);
                Assert.AreEqual(4, accountDto.AccountStatusSort);
            }
            {
                MyAccountsSummaryResultDto accountDto = outputDto.Accounts[6];
                Assert.AreEqual("Ghi", accountDto.OriginalCompany);
                Assert.AreEqual(4, accountDto.AccountStatusSort);
            }

            // Sort Value 100
            {
                MyAccountsSummaryResultDto accountDto = outputDto.Accounts[7];
                Assert.AreEqual("jjj", accountDto.OriginalCompany);
                Assert.AreEqual(100, accountDto.AccountStatusSort);
            }
        }

        [TestMethod]
        public void GetMyAccountsDetail_WhenProvideLowellReference_RetrievesAccount_OutputsTransformedModel()
        {
            const string userId = "anyuser123";
            const string lowellReference = "wibble";

            {
                // Arrange
                CaseFlowMyAccountsDetailDto caseFlowDto = new CaseFlowMyAccountsDetailDto()
                {
                    LowellReference = "1234",
                    ClientName = "BarclayBob",
                    OutstandingBalance = 1234.56M,
                    CanMakePayment = false,

                    HasArrangement = true,
                    PlanType = "Direct Debit",
                    PlanFrequency = "Monthly",
                    RegularAccountPaymentAmount = 123.45M,

                    ExcludedMessage = "Test excl"
                };

                _mockGetDetailsProcess.Setup(x => x.GetMyAccountsDetail(lowellReference)).Returns(Task.FromResult(caseFlowDto));

                // Act
                MyAccountsDetailResultDto resultDto = _service.GetMyAccountsDetail(userId, lowellReference).Result;

                // Assert
                {
                    Assert.AreEqual("1234", resultDto.AccountReference);
                    Assert.AreEqual("BarclayBob", resultDto.OriginalCompany);
                    Assert.AreEqual(1234.56M, resultDto.OutstandingBalance);
                    Assert.AreEqual(false, resultDto.CanMakePayment);

                    Assert.AreEqual("Direct Debit", resultDto.PaymentPlanMethod);
                    Assert.AreEqual("Monthly", resultDto.PaymentPlanFrequency);
                    Assert.AreEqual(123.45M, resultDto.PaymentPlanAmount);

                    Assert.AreEqual("Test excl", resultDto.AccountMessage);
                }
            }
        }

        [TestMethod]
        [DataRow(123.55D, 123.54D, true, 123.54D)]
        [DataRow(1.0D, 2.0D, true, 2.0D)]
        [DataRow(3.0D, 2.0D, true, 2.0D)]
        [DataRow(3.0D, 0.0D, true, 0.0D)]
        [DataRow(0.0D, 2.0D, true, 2.0D)]
        [DataRow(0.0D, null, true, null)]
        [DataRow(1.0D, null, true, null)]
        [DataRow(null, null, true, null)]
        [DataRow(null, 2.0D, true, 2.0D)]
        [DataRow(123.55D, 123.54D, false, null)]
        [DataRow(1.0D, 2.0D, false, null)]
        [DataRow(3.0D, 2.0D, false, null)]
        [DataRow(3.0D, 0.0D, false, null)]
        [DataRow(0.0D, 2.0D, false, null)]
        [DataRow(0.0D, null, false, null)]
        [DataRow(1.0D, null, false, null)]
        [DataRow(null, null, false, null)]
        [DataRow(null, 2.0D, false, null)]
        public void GetMyAccountsDetail_WhenProvideLowellReference_AndDiscountBalanceNotEqualToOutstandingBalanceOrNull_RetrievesAccount_DiscountBalanceOutput(
            double? testOutstandingBalance, double? testDiscountedBalance, bool hasArrangement, double? expectedResult)
        {
            // Discount Balance will be different to Outstanding Balance if there's a discount

            // Arrange
            const string userId = "anyuser123";
            const string lowellReference = "wibble";

            CaseFlowMyAccountsDetailDto caseFlowDto = new CaseFlowMyAccountsDetailDto()
            {
                OutstandingBalance = (decimal?)testOutstandingBalance,    // doubles in params, because MSTest does not support decimal
                DiscountedBalance = (decimal?)testDiscountedBalance,
                HasArrangement = hasArrangement
            };

            _mockGetDetailsProcess.Setup(x => x.GetMyAccountsDetail(lowellReference)).Returns(Task.FromResult(caseFlowDto));

            // Act
            MyAccountsDetailResultDto resultDto = _service.GetMyAccountsDetail(userId, lowellReference).Result;

            // Assert
            Assert.AreEqual((decimal?)expectedResult, resultDto.DiscountedBalance);

            // Ensure outstanding balance unaffected
            Assert.AreEqual((decimal?)testOutstandingBalance ?? 0, resultDto.OutstandingBalance);
        }

        [TestMethod]
        [DataRow(1.0D, 1.0D)]
        [DataRow(123.45D, 123.45D)]
        [DataRow(0.0D, 0.0D)]
        [DataRow(null, null)]
        public void GetMyAccountsDetail_WhenProvideLowellReference_AndDiscountedBalanceEqualToOutstandingBalance_RetrievesAccount_DiscountBalanceNOTOutput(
            double? testOutstandingBalance, double? testDiscountedBalance)
        {
            // Discount Balance will EQUAL Outstanding Balance if there isn't a discount

            // Arrange
            const string userId = "anyuser123";
            const string lowellReference = "wibble";

            CaseFlowMyAccountsDetailDto caseFlowDto = new CaseFlowMyAccountsDetailDto()
            {
                OutstandingBalance = (decimal?)testOutstandingBalance,    // doubles in params, because MSTest does not support decimal
                DiscountedBalance = (decimal?)testDiscountedBalance
            };

            _mockGetDetailsProcess.Setup(x => x.GetMyAccountsDetail(lowellReference)).Returns(Task.FromResult(caseFlowDto));

            // Act
            MyAccountsDetailResultDto resultDto = _service.GetMyAccountsDetail(userId, lowellReference).Result;

            // Assert
            Assert.AreEqual(null, resultDto.DiscountedBalance);

            // Ensure outstanding balance unaffected
            Assert.AreEqual((decimal?)testOutstandingBalance ?? 0, resultDto.OutstandingBalance);
        }

        [TestMethod]

        // Has Arrangement & Balance - passes through values
        [DataRow(true, 0.01D, "Direct Debit", "Monthly", 9999.99D, "Direct Debit", "Monthly", 9999.99D)]
        [DataRow(true, 1000.99D, "Direct Debit", "Monthly", 9999.99D, "Direct Debit", "Monthly", 9999.99D)]

        // Has Arrangement false - suppresses values
        [DataRow(false, 0.01D, "Direct Debit", "Monthly", 9999.99D, null, null, null)]

        // Balance <= 0 - suppresses values
        [DataRow(true, null, "Direct Debit", "Monthly", 9999.99D, null, null, null)]
        [DataRow(true, 0D, "Direct Debit", "Monthly", 9999.99D, null, null, null)]
        [DataRow(true, -0.01D, "Direct Debit", "Monthly", 9999.99D, null, null, null)]

        // Both invalid - suppresses values
        [DataRow(false, -0.01D, "Direct Debit", "Monthly", 9999.99D, null, null, null)]

        public void GetMyAccountsDetail_WhenProvideLowellReference_AndSatisfiesPlanConditions_RetrievesAccount_OutputsPlan(
            bool testHasArrangement,
            double? testOutstandingBalance,
            string testPlanType,
            string testPlanFrequency,
            double? testRegularAccountPaymentAmount,

            string expectedPaymentPlanMethod,
            string expectedPaymentPlanFrequency,
            double? expectedPaymentPlanAmount)
        {
            // Arrange
            const string userId = "anyuser123";
            const string lowellReference = "wibble";

            CaseFlowMyAccountsDetailDto caseFlowDto = new CaseFlowMyAccountsDetailDto()
            {
                HasArrangement = testHasArrangement,
                OutstandingBalance = (decimal?)testOutstandingBalance,    // doubles in params, because MSTest does not support decimal
                PlanType = testPlanType,
                PlanFrequency = testPlanFrequency,
                RegularAccountPaymentAmount = (decimal?)testRegularAccountPaymentAmount
            };

            _mockGetDetailsProcess.Setup(x => x.GetMyAccountsDetail(lowellReference)).Returns(Task.FromResult(caseFlowDto));

            // Act
            MyAccountsDetailResultDto resultDto = _service.GetMyAccountsDetail(userId, lowellReference).Result;

            // Assert
            Assert.AreEqual(expectedPaymentPlanMethod, resultDto.PaymentPlanMethod);
            Assert.AreEqual(expectedPaymentPlanFrequency, resultDto.PaymentPlanFrequency);
            Assert.AreEqual((decimal?)expectedPaymentPlanAmount, resultDto.PaymentPlanAmount);

            // Ensure outstanding balance unaffected
            Assert.AreEqual((decimal?)testOutstandingBalance ?? 0, resultDto.OutstandingBalance);
        }

        [TestMethod]
        public void GetMyAccountsDetail_WhenProvideLowellReference_RetrievesAccount_CallsStatusDeriver()
        {
            // Note: using mock Status Deriver, so as to avoid exhaustively testing statuses here, as responsibility
            // for deriving statuses is with concrete Status Deriver class

            // Mock will apply a status value, allowing us to ensure it is called

            // Arrange
            const string userId = "anyuser123";
            const string lowellReference = "wibble";

            _mockStatusDeriverProcess.Setup(x => x.DeriveStatusAndStatusSort(
                    It.IsAny<ICaseFlowAccountCommon>(),
                    It.IsAny<MyAccountsSummaryResultDto>()))
                .Callback<ICaseFlowAccountCommon, MyAccountsSummaryResultDto>(
                    (caseFlowAccountDto, resultDto) =>
                    {
                        // Status deriver places 'ClientName' into status to facilitate testing
                        Debug.Assert(caseFlowAccountDto.ClientName != null);
                        resultDto.AccountStatus = caseFlowAccountDto.ClientName;
                    });

            CaseFlowMyAccountsDetailDto caseFlowDto = new CaseFlowMyAccountsDetailDto()
            {
                ClientName = "Def"
            };

            _mockGetDetailsProcess.Setup(x => x.GetMyAccountsDetail(lowellReference)).Returns(Task.FromResult(caseFlowDto));

            // Act
            MyAccountsDetailResultDto outputDto = _service.GetMyAccountsDetail(userId, lowellReference).Result;

            Assert.AreEqual("Def", outputDto.AccountStatus);
        }


        [TestMethod]

        // Discount Amount present, not passed expiry and not accepted discount (Outstanding Balance == Discounted Balance)
        // AND not with solicitors (NOT company 2)
        [DataRow(1, 100.0D, 100.0D, 55.99D, "15/06/2012", "15/06/2012 23:59", 0L, false, "", 55.99D, "15/06/2012")]     // on expiry date (end minute of day)
        [DataRow(2, 100.0D, 100.0D, 55.99D, "15/06/2012", "15/06/2012 00:00", 0L, false, "", 55.99D, "15/06/2012")]     // on expiry date (start of day)

        [DataRow(3, 100.0D, 100.0D, 55.99D, "15/06/2012", "14/06/2012 23:59", 0L, false, "", 55.99D, "15/06/2012")]     // prior
        [DataRow(4, 100.0D, 100.0D, 55.99D, "15/06/2012", "14/06/2012 00:00", 0L, false, "", 55.99D, "15/06/2012")]
        [DataRow(5, 100.0D, 100.0D, 55.99D, "15/06/2012", "01/01/1950 00:00", 0L, false, "", 55.99D, "15/06/2012")]

        // Other company numbers (not Solicitors) - still outputs
        [DataRow(11, 100.0D, 100.0D, 55.99D, "15/06/2012", "01/01/1950 00:00", null, false, "", 55.99D, "15/06/2012")]
        [DataRow(12, 100.0D, 100.0D, 55.99D, "15/06/2012", "01/01/1950 00:00", 1L, false, "", 55.99D, "15/06/2012")]
        [DataRow(13, 100.0D, 100.0D, 55.99D, "15/06/2012", "01/01/1950 00:00", 3L, false, "", 55.99D, "15/06/2012")]

        // No arrangement OR Plan Type NOT direct debit - still outputs
        [DataRow(21, 100.0D, 100.0D, 55.99D, "15/06/2012", "15/06/2012 23:59", 0L, false, "Direct Debit", 55.99D, "15/06/2012")]     // note Arrangement=False, Plan Type DD
        [DataRow(22, 100.0D, 100.0D, 55.99D, "15/06/2012", "15/06/2012 23:59", 0L, true, "Cash", 55.99D, "15/06/2012")]  // note Arrangement=True, Plan Type not DD

        // Missing discount amount - suppresses offer
        [DataRow(31, 100.0D, 100.0D, null, "15/06/2012", "15/06/2012 00:00", 0L, false, "", null, null)]

        // Missing expiry - suppresses offer
        [DataRow(41, 100.0D, 100.0D, 55.99D, null, "15/06/2012 00:00", 0L, false, "", null, null)]

        // Past expiry date - suppresses offer
        [DataRow(51, 100.0D, 100.0D, 55.99D, "15/06/2012", "16/06/2012 00:00", 0L, false, "", null, null)]
        [DataRow(52, 100.0D, 100.0D, 55.99D, "15/06/2012", "16/06/2012 23:59", 0L, false, "", null, null)]
        [DataRow(53, 100.0D, 100.0D, 55.99D, "15/06/2012", "17/06/2012 00:00", 0L, false, "", null, null)]
        [DataRow(54, 100.0D, 100.0D, 55.99D, "15/06/2012", "01/01/2020 00:00", 0L, false, "", null, null)]

        // Accepted offer (Outstanding Balance <> Discounted Balance) - suppresses offer output
        [DataRow(61, 100.0D, 99.9D, 55.99D, "15/06/2012", "15/06/2012 00:00", 0L, true, "", null, null)]
        [DataRow(62, null, 100.0D, 55.99D, "15/06/2012", "15/06/2012 00:00", 0L, true, "", null, null)]
        [DataRow(63, 100.0D, null, 55.99D, "15/06/2012", "15/06/2012 00:00", 0L, true, "", null, null)]
        
        // With solicitors (company 2) - suppresses offer
        [DataRow(71, 100.0D, 100.0D, 55.99D, "15/06/2012", "01/01/1950 00:00", 2L, false, "", null, null)]

        // Direct debit plan - suppresses offer
        [DataRow(81, 100.0D, 100.0D, 55.99D, "15/06/2012", "15/06/2012 23:59", 0L, true, "Direct Debit", null, null)]     // on expiry date (end minute of day)

        public void GetMyAccountsDetail_WhenProvideLowellReference_AndSatisfiesDiscountOfferConditions_RetrievesAccount_OutputsDiscountOffer(
            int testNumber,
            double? testOutstandingBalance,
            double? testDiscountedBalance,
            double? testDiscountAmount,     // indicates offer
            string testDiscountExpiryString,
            string testCurrentDateTimeString,
            long? testCompany,
            bool testHasArrangement,
            string testPlanType,

            double? expectedDiscountOfferAmount,
            string expectedDiscountOfferExpiryString)
        {
            // Can't use DateTime in attribute, therefore need to convert from strings
            DateTime? testDiscountExpiry = ConvertTestStringToNullableDateTime(testDiscountExpiryString, "d");
            DateTime testCurrentDateTime = ConvertTestStringToDateTime(testCurrentDateTimeString, "g");
            DateTime? expectedDiscountOfferExpiry = ConvertTestStringToNullableDateTime(expectedDiscountOfferExpiryString, "d");

            // Test assumption - 'Local' date can be compared with 'Unspecified' date
            {
                DateTime dtNow = DateTime.Now;
                Assert.AreEqual(DateTimeKind.Local, dtNow.Kind);

                DateTime dtNowMinusHour = new DateTime(dtNow.Year, dtNow.Month, dtNow.Day, dtNow.Hour, dtNow.Minute, dtNow.Second).AddHours(-1);
                Assert.AreEqual(DateTimeKind.Unspecified, dtNowMinusHour.Kind);

                Assert.IsTrue(dtNowMinusHour < dtNow);
                Assert.IsTrue(dtNow > dtNowMinusHour);

                DateTime dtNowPlusHour = new DateTime(dtNow.Year, dtNow.Month, dtNow.Day, dtNow.Hour, dtNow.Minute, dtNow.Second).AddHours(1);
                Assert.AreEqual(DateTimeKind.Unspecified, dtNowMinusHour.Kind);

                Assert.IsTrue(dtNowPlusHour > dtNow);
                Assert.IsTrue(dtNow < dtNowPlusHour);
            }

            //
            // Arrange
            //
            const string userId = "anyuser123";
            const string lowellReference = "wibble";

            CaseFlowMyAccountsDetailDto caseFlowDto = new CaseFlowMyAccountsDetailDto()
            {
                OutstandingBalance = (decimal?)testOutstandingBalance,  // doubles in params, because MSTest does not support decimal
                DiscountedBalance = (decimal?)testDiscountedBalance,
                DiscountAmount = (decimal?)testDiscountAmount,
                DiscountExpiry = testDiscountExpiry,
                Company = testCompany,
                HasArrangement = testHasArrangement,
                PlanType = testPlanType
            };

            _mockGetDetailsProcess.Setup(x => x.GetMyAccountsDetail(lowellReference)).Returns(Task.FromResult(caseFlowDto));

            // Must use concrete Status Deriver, so that Solicitor (company 2) logic is followed
            MyAccountsService service = new MyAccountsService(_mockLogger.Object,
                _mockGetProcess.Object,
                _mockGetDetailsProcess.Object,
                _mockMyAccountDetailGetTransactionsRecentProcess.Object,
                new MyAccountsStatusDeriver(_mockMyAccountsStatusDeriverlogger.Object));     // **** IMPORTANT BIT

            //
            // Act
            //

            // Using service created above, so as to utilise concrete Status Deriver
            MyAccountsDetailResultDto resultDto = service.GetMyAccountsDetail(userId, lowellReference, testCurrentDateTime).Result;

            //
            // Assert
            //
            Assert.AreEqual((decimal?)expectedDiscountOfferAmount, resultDto.DiscountOfferAmount);
            Assert.AreEqual(expectedDiscountOfferExpiry, resultDto.DiscountOfferExpiry);
        }

        [TestMethod]

        //
        // Not in arrears - output arrears value will be null
        //
        [DataRow(null, false, null)]
        [DataRow(0.0D, false, null)]
        [DataRow(-0.01D, false, null)]
        [DataRow(-1D, false, null)]
        [DataRow(-1000D, false, null)]

        // Check with plan in place - does not affect if no arrears
        [DataRow(null, true, null)]
        [DataRow(0.0D, true, null)]
        [DataRow(-0.01D, true, null)]
        [DataRow(-1D, true, null)]
        [DataRow(-1000D, true, null)]

        //
        // In arrears - only outputs if also Has Arrangement
        //

        // No arrangement
        [DataRow(0.01D, false, null)]
        [DataRow(1000.99D, false, null)]

        // Has Arrangement
        [DataRow(0.01D, true, 0.01D)]
        [DataRow(1000.99D, true, 1000.99D)]

        public void GetMyAccountsDetail_WhenProvideLowellReference_RetrievesAccount_OutputsArrears(
            double? testArrears, bool testHasArrangement,
            double? expectedPaymentPlanArrearsAmount)
        {
            // Arrange
            const string userId = "anyuser123";
            const string lowellReference = "wibble";

            CaseFlowMyAccountsDetailDto caseFlowDto = new CaseFlowMyAccountsDetailDto()
            {
                Arrears = (decimal?)testArrears,
                HasArrangement = testHasArrangement
            };

            _mockGetDetailsProcess.Setup(x => x.GetMyAccountsDetail(lowellReference)).Returns(Task.FromResult(caseFlowDto));

            // Act
            MyAccountsDetailResultDto resultDto = _service.GetMyAccountsDetail(userId, lowellReference).Result;

            // Assert
            Assert.AreEqual((decimal?)expectedPaymentPlanArrearsAmount, resultDto.PaymentPlanArrearsAmount);
        }

        [TestMethod]

        // No Arrangement - always returns false, regardless of type
        [DataRow(false, "Cash", false)]
        [DataRow(false, "anything else", false)]

        [DataRow(false, "Direct Debit", false)]
        [DataRow(false, "Debit Card", false)]
        [DataRow(false, "Credit Card", false)]

        // Has Arrangement - returns true / false as appropriate

        [DataRow(true, "Cash", false)]
        [DataRow(true, "anything else", false)]

        [DataRow(true, "Direct Debit", true)]
        [DataRow(true, "Debit Card", true)]
        [DataRow(true, "Credit Card", true)]

        public void GetMyAccountsDetail_WhenProvideLowellReference_RetrievesAccount_OutputsPaymentPlanIsAutomated(
            bool testHasArrangement, string testPlanType,
            bool expectedPaymentPlanIsAutomated)
        {
            // Arrange
            const string userId = "anyuser123";
            const string lowellReference = "wibble";

            CaseFlowMyAccountsDetailDto caseFlowDto = new CaseFlowMyAccountsDetailDto()
            {
                HasArrangement = testHasArrangement,
                PlanType = testPlanType
            };

            _mockGetDetailsProcess.Setup(x => x.GetMyAccountsDetail(lowellReference)).Returns(Task.FromResult(caseFlowDto));

            // Act
            MyAccountsDetailResultDto resultDto = _service.GetMyAccountsDetail(userId, lowellReference).Result;

            // Assert
            Assert.AreEqual(expectedPaymentPlanIsAutomated, resultDto.PaymentPlanIsAutomated);
        }

        [TestMethod]
        public void GetMyAccountsDetail_WhenProvideLowellReference_With_NullTransactions_Then_OutputsEmptyTransactionsList()
        {
            // Arrange
            const string userId = "anyuser123";
            const string lowellReference = "wibble";

            CaseFlowMyAccountsDetailDto caseFlowAccountDto = new CaseFlowMyAccountsDetailDto();

            _mockGetDetailsProcess.Setup(x => x.GetMyAccountsDetail(lowellReference)).Returns(Task.FromResult(caseFlowAccountDto));
            _mockMyAccountDetailGetTransactionsRecentProcess.Setup(x => x.GetTransactionsRecent(lowellReference, 5))
                .Returns(Task.FromResult((RecievedTransactionsDto)null));  // important bit - NULL

            // Act
            MyAccountsDetailResultDto resultDto = _service.GetMyAccountsDetail(userId, lowellReference).Result;

            // Assert
            Assert.AreEqual(0, resultDto.RecentTransactions.Count);
        }

        [TestMethod]
        public void GetMyAccountsDetail_WhenProvideLowellReference_With_NoTransactions_Then_OutputsEmptyTransactionsList()
        {
            // Arrange
            const string userId = "anyuser123";
            const string lowellReference = "wibble";

            CaseFlowMyAccountsDetailDto caseFlowAccountDto = new CaseFlowMyAccountsDetailDto();
            RecievedTransactionsDto caseFlowReceivedTransactionsDto = new RecievedTransactionsDto();

            _mockGetDetailsProcess.Setup(x => x.GetMyAccountsDetail(lowellReference)).Returns(Task.FromResult(caseFlowAccountDto));
            _mockMyAccountDetailGetTransactionsRecentProcess.Setup(x => x.GetTransactionsRecent(lowellReference, 5)).Returns(Task.FromResult(caseFlowReceivedTransactionsDto));

            // Act
            MyAccountsDetailResultDto resultDto = _service.GetMyAccountsDetail(userId, lowellReference).Result;

            // Assert
            Assert.AreEqual(0, resultDto.RecentTransactions.Count);
        }

        [TestMethod]
        public void GetMyAccountsDetail_WhenProvideLowellReference_With_TwoTransactions_Then_OutputsTransaction()
        {
            // NOTE: Not testing 5 transactions (or more) because the number of transactions is controlled
            // by the proxy call (to CaseFlow)

            // Arrange
            const string userId = "anyuser123";
            const string lowellReference = "wibble";

            CaseFlowMyAccountsDetailDto caseFlowAccountDto = new CaseFlowMyAccountsDetailDto();
            RecievedTransactionsDto caseFlowReceivedTransactionsDto = new RecievedTransactionsDto()
            {
                Payments = new List<PaymentDetails>()
                {
                    new PaymentDetails()
                    {
                        Date = new DateTime(2018, 11, 25),
                        Amount = 123.45M,
                        Type = "Type",
                        RollingBalance = 678.91M
                    }
                }
            };

            _mockGetDetailsProcess.Setup(x => x.GetMyAccountsDetail(lowellReference)).Returns(Task.FromResult(caseFlowAccountDto));
            _mockMyAccountDetailGetTransactionsRecentProcess.Setup(x => x.GetTransactionsRecent(lowellReference, 5)).Returns(Task.FromResult(caseFlowReceivedTransactionsDto));

            // Act
            MyAccountsDetailResultDto resultDto = _service.GetMyAccountsDetail(userId, lowellReference).Result;

            // Assert
            Assert.AreEqual(1, resultDto.RecentTransactions.Count);
            {
                MyAccountsDetailResultDto.RecentTransaction transaction = resultDto.RecentTransactions[0];
                Assert.AreEqual(new DateTime(2018, 11, 25), transaction.Date);
                Assert.AreEqual(123.45M, transaction.Amount);
                Assert.AreEqual("Type", transaction.Type);
                Assert.AreEqual(678.91M, transaction.RollingBalance);
            }

            _mockMyAccountDetailGetTransactionsRecentProcess.Verify(x => x.GetTransactionsRecent(lowellReference, 5), Times.Once);
        }

        [TestMethod]
        public void GetMyAccountsDetail_WhenProvideLowellReference_With_FiveTransactions_Then_OutputsFiveTransactions()
        {
            // NOTE: Not testing 5 transactions (or more) because the number of transactions is controlled
            // by the proxy call (to CaseFlow)

            // Arrange
            const string userId = "anyuser123";
            const string lowellReference = "wibble";

            CaseFlowMyAccountsDetailDto caseFlowAccountDto = new CaseFlowMyAccountsDetailDto();
            RecievedTransactionsDto caseFlowReceivedTransactionsDto = new RecievedTransactionsDto()
            {
                Payments = new List<PaymentDetails>()
                {
                    new PaymentDetails()
                    {
                        Date = new DateTime(2018, 11, 25),
                        Amount = 123.45M,
                        Type = "Type",
                        RollingBalance = 678.91M
                    },
                    new PaymentDetails()
                    {
                        Date = new DateTime(2017, 10, 13),
                        Amount = 9999.99M,
                        Type = "Type II",
                        RollingBalance = 1000000.01M
                    }
                }
            };

            _mockGetDetailsProcess.Setup(x => x.GetMyAccountsDetail(lowellReference)).Returns(Task.FromResult(caseFlowAccountDto));
            _mockMyAccountDetailGetTransactionsRecentProcess.Setup(x => x.GetTransactionsRecent(lowellReference, 5)).Returns(Task.FromResult(caseFlowReceivedTransactionsDto));

            // Act
            MyAccountsDetailResultDto resultDto = _service.GetMyAccountsDetail(userId, lowellReference).Result;

            // Assert
            Assert.AreEqual(2, resultDto.RecentTransactions.Count);
            {
                MyAccountsDetailResultDto.RecentTransaction transaction = resultDto.RecentTransactions[0];
                Assert.AreEqual(new DateTime(2018, 11, 25), transaction.Date);
                Assert.AreEqual(123.45M, transaction.Amount);
                Assert.AreEqual("Type", transaction.Type);
                Assert.AreEqual(678.91M, transaction.RollingBalance);
            }
            {
                MyAccountsDetailResultDto.RecentTransaction transaction = resultDto.RecentTransactions[1];
                Assert.AreEqual(new DateTime(2017, 10, 13), transaction.Date);
                Assert.AreEqual(9999.99M, transaction.Amount);
                Assert.AreEqual("Type II", transaction.Type);
                Assert.AreEqual(1000000.01M, transaction.RollingBalance);
            }

            _mockMyAccountDetailGetTransactionsRecentProcess.Verify(x => x.GetTransactionsRecent(lowellReference, 5), Times.Once);
        }

        [TestMethod]

        // True from CaseFlow - overridden to false if with solicitors
        [DataRow(true, false, true)]
        [DataRow(true, true, false)]        // with Solicitors - overridden to false

        // False from CaseFlow - stays as false, regardless
        [DataRow(false, false, false)]
        [DataRow(false, true, false)]       // with Solicitors - stays false

        public void GetMyAccountsDetail_WhenProvideLowellReference_AndAccountWithSolicitors_RetrievesAccount_OverridesCanMakePaymentToFalse(
            bool testCanMakePaymentFlagFromCaseFlow,
            bool testIsWithSolicitorsFromStatusDeriver,
            bool expectedCanMakePaymentOutput)
        {
            // Arrange
            const string userId = "anyuser123";
            const string lowellReference = "wibble";

            CaseFlowMyAccountsDetailDto caseFlowDto = new CaseFlowMyAccountsDetailDto()
            {
                // Flag that may be overridden
                CanMakePayment = testCanMakePaymentFlagFromCaseFlow
            };

            _mockGetDetailsProcess.Setup(x => x.GetMyAccountsDetail(lowellReference)).Returns(Task.FromResult(caseFlowDto));

            _mockStatusDeriverProcess.Setup(x => x.DeriveStatusAndStatusSort(caseFlowDto, It.IsAny<MyAccountsSummaryResultDto>()))
                .Callback<ICaseFlowAccountCommon, MyAccountsSummaryResultDto>(
                    (caseFlowAccountDto, deriverResultDto) =>
                    {
                        // Status deriver places 'Is With Solicitors' flag into output DTO to facilitate testing
                        deriverResultDto.AccountStatusIsWithSolicitors = testIsWithSolicitorsFromStatusDeriver;
                    });

            // Act
            MyAccountsDetailResultDto resultDto = _service.GetMyAccountsDetail(userId, lowellReference).Result;

            // Assert
            Assert.AreEqual(expectedCanMakePaymentOutput, resultDto.CanMakePayment);
        }

        DateTime? ConvertTestStringToNullableDateTime(string value, string format)
        {
            if (value == null) return null;
            return ConvertTestStringToDateTime(value, format);
        }

        DateTime ConvertTestStringToDateTime(string value, string format)
        {
            return DateTime.ParseExact(value, format, new CultureInfo("en-GB"));
        }


    }
}

