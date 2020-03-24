using FinancialAccountService.Models.DataTransferObjects;
using FinancialAccountService.Processes;
using FinancialAccountService.Proxy.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Processes.Unit.Tests
{
    [TestClass]

    public class MyAccountsGetProcessTests
    {
        private Mock<ILogger<MyAccountsGetProcess>> _mockLogger;
        private Mock<ICaseflowApiProxy> _mockProxy;
        private MyAccountsGetProcess _process;

        [TestInitialize]
        public void Initialise()
        {
            _mockLogger = new Mock<ILogger<MyAccountsGetProcess>>();
            _mockProxy = new Mock<ICaseflowApiProxy>(MockBehavior.Strict);

            _process = new MyAccountsGetProcess(_mockLogger.Object, _mockProxy.Object);
        }


        [TestMethod]
        public void GetMyAccounts_ShouldCallProxy_Once()
        {
            const string testLoginId = "anything123";

            // Arrange
            // Content does not matter, as straight pass through of objects
            var caseFlowDto = new CaseFlowMyAccountsDto();
            
            _mockProxy.Setup(x => x.GetMyAccountsAsync(testLoginId))
                .Returns(Task.FromResult(caseFlowDto));

            //Act
            var result = _process.GetMyAccounts(testLoginId).Result;

            //Assert
            Assert.AreEqual(caseFlowDto, result);

            _mockProxy.Verify(x => x.GetMyAccountsAsync(testLoginId), Times.Once);
        }

        [TestMethod]
        public void GetMyAccounts_FiltersDeletes()
        {
            const string testLoginId = "anything123";

            List<CaseFlowMyAccountsDto.AccountSummaryDto> deleted_accounts = new List<CaseFlowMyAccountsDto.AccountSummaryDto>()
            {
                new CaseFlowMyAccountsDto.AccountSummaryDto() { IsClosedHidden = true, LowellReference = "11111111" },
                new CaseFlowMyAccountsDto.AccountSummaryDto() { IsClosedHidden = true, LowellReference = "22222222" }
            };

            List<CaseFlowMyAccountsDto.AccountSummaryDto> active_accounts = new List<CaseFlowMyAccountsDto.AccountSummaryDto>()
            {
                new CaseFlowMyAccountsDto.AccountSummaryDto() { IsClosedHidden = false, LowellReference = "33333333" },
                new CaseFlowMyAccountsDto.AccountSummaryDto() { IsClosedHidden = false, LowellReference = "44444444" },
                new CaseFlowMyAccountsDto.AccountSummaryDto() { IsClosedHidden = false, LowellReference = "55555555" },
                new CaseFlowMyAccountsDto.AccountSummaryDto() { IsClosedHidden = false, LowellReference = "66666666" },
                new CaseFlowMyAccountsDto.AccountSummaryDto() { IsClosedHidden = false, LowellReference = "77777777" },
                new CaseFlowMyAccountsDto.AccountSummaryDto() { IsClosedHidden = false, LowellReference = "88888888" }
            };

            // Arrange            
            CaseFlowMyAccountsDto caseFlowDto = new CaseFlowMyAccountsDto() { Summaries = new List<CaseFlowMyAccountsDto.AccountSummaryDto>() };
            caseFlowDto.Summaries.AddRange(deleted_accounts);
            caseFlowDto.Summaries.AddRange(active_accounts);

            _mockProxy.Setup(x => x.GetMyAccountsAsync(testLoginId))
                .Returns(Task.FromResult(caseFlowDto));

            //Act
            var result = _process.GetMyAccounts(testLoginId).Result;

            //Assert
            Assert.AreEqual(active_accounts.Count, result.Summaries.Count);

            foreach (CaseFlowMyAccountsDto.AccountSummaryDto account in active_accounts)
            {
                Assert.IsTrue(result.Summaries.Contains(account));
            }

            foreach (CaseFlowMyAccountsDto.AccountSummaryDto account in deleted_accounts)
            {
                Assert.IsFalse(result.Summaries.Contains(account));
            }

            _mockProxy.Verify(x => x.GetMyAccountsAsync(testLoginId), Times.Once);
        }

        [TestMethod]
        public void GetMyAccountsSummary_ShouldCallProxy_Once()
        {
            const string testAccountId = "6436";

            // Arrange
            // Content does not matter, as straight pass through of objects
            var caseFlowDto = new CaseFlowMyAccountsDto();

            _mockProxy.Setup(x => x.GetMyAccountsSummaryAsync(testAccountId))
                .Returns(Task.FromResult(caseFlowDto));

            //Act
            var result = _process.GetMyAccountsSummary(testAccountId).Result;

            //Assert
            Assert.AreEqual(caseFlowDto, result);

            _mockProxy.Verify(x => x.GetMyAccountsSummaryAsync(testAccountId), Times.Once);
        }

        [TestMethod]
        public void GetMyAccountsSummary_FiltersDeletes()
        {
            const string testAccountId = "456546";

            List<CaseFlowMyAccountsDto.AccountSummaryDto> deleted_accounts = new List<CaseFlowMyAccountsDto.AccountSummaryDto>()
            {
                new CaseFlowMyAccountsDto.AccountSummaryDto() { IsClosedHidden = true, LowellReference = "11111111" },
                new CaseFlowMyAccountsDto.AccountSummaryDto() { IsClosedHidden = true, LowellReference = "22222222" }
            };

            List<CaseFlowMyAccountsDto.AccountSummaryDto> active_accounts = new List<CaseFlowMyAccountsDto.AccountSummaryDto>()
            {
                new CaseFlowMyAccountsDto.AccountSummaryDto() { IsClosedHidden = false, LowellReference = "33333333" },
                new CaseFlowMyAccountsDto.AccountSummaryDto() { IsClosedHidden = false, LowellReference = "44444444" },
                new CaseFlowMyAccountsDto.AccountSummaryDto() { IsClosedHidden = false, LowellReference = "55555555" },
                new CaseFlowMyAccountsDto.AccountSummaryDto() { IsClosedHidden = false, LowellReference = "66666666" },
                new CaseFlowMyAccountsDto.AccountSummaryDto() { IsClosedHidden = false, LowellReference = "77777777" },
                new CaseFlowMyAccountsDto.AccountSummaryDto() { IsClosedHidden = false, LowellReference = "88888888" }
            };

            // Arrange            
            CaseFlowMyAccountsDto caseFlowDto = new CaseFlowMyAccountsDto() { Summaries = new List<CaseFlowMyAccountsDto.AccountSummaryDto>() };
            caseFlowDto.Summaries.AddRange(deleted_accounts);
            caseFlowDto.Summaries.AddRange(active_accounts);

            _mockProxy.Setup(x => x.GetMyAccountsSummaryAsync(testAccountId))
                .Returns(Task.FromResult(caseFlowDto));

            //Act
            var result = _process.GetMyAccountsSummary(testAccountId).Result;

            //Assert
            Assert.AreEqual(active_accounts.Count, result.Summaries.Count);

            foreach (CaseFlowMyAccountsDto.AccountSummaryDto account in active_accounts)
            {
                Assert.IsTrue(result.Summaries.Contains(account));
            }

            foreach (CaseFlowMyAccountsDto.AccountSummaryDto account in deleted_accounts)
            {
                Assert.IsFalse(result.Summaries.Contains(account));
            }

            _mockProxy.Verify(x => x.GetMyAccountsSummaryAsync(testAccountId), Times.Once);
        }
    }
}
