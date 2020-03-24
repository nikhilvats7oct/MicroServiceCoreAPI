using FinancialAccountService.Models.DataTransferObjects;
using FinancialAccountService.Processes;
using FinancialAccountService.Proxy.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Threading.Tasks;

namespace Processes.Unit.Tests
{
    [TestClass]
    public class MyAccountsDetailGetProcessTests
    {
        private Mock<ILogger<MyAccountsDetailGetProcess>> _mockLogger;
        private Mock<ICaseflowApiProxy> _mockProxy;
        private MyAccountsDetailGetProcess _process;

        [TestInitialize]
        public void Initialise()
        {
            _mockLogger = new Mock<ILogger<MyAccountsDetailGetProcess>>();
            _mockProxy = new Mock<ICaseflowApiProxy>(MockBehavior.Strict);

            _process = new MyAccountsDetailGetProcess(_mockLogger.Object, _mockProxy.Object);
        }


        [TestMethod]
        public void GetMyAccounts_ShouldCallProxy_Once()
        {
            const string testLoginId = "anything123";

            // Arrange
            // Content does not matter, as straight pass through of objects
            var caseFlowDto = new CaseFlowMyAccountsDetailDto();

            _mockProxy.Setup(x => x.GetMyAccountsDetailAsync(testLoginId))
                .Returns(Task.FromResult(caseFlowDto));

            //Act
            var result = _process.GetMyAccountsDetail(testLoginId).Result;

            //Assert
            Assert.AreEqual(caseFlowDto, result);

            _mockProxy.Verify(x => x.GetMyAccountsDetailAsync(testLoginId), Times.Once);
        }

    }
}
