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
    public class MyAccountDetailGetTransactionsRecentProcessTests
    {
        private Mock<ILogger<MyAccountDetailGetTransactionsRecentProcess>> _mockLogger;
        private Mock<ICaseflowApiProxy> _mockProxy;
        private MyAccountDetailGetTransactionsRecentProcess _process;

        [TestInitialize]
        public void Initialise()
        {
            _mockLogger = new Mock<ILogger<MyAccountDetailGetTransactionsRecentProcess>>();
            _mockProxy = new Mock<ICaseflowApiProxy>(MockBehavior.Strict);

            _process = new MyAccountDetailGetTransactionsRecentProcess(_mockLogger.Object, _mockProxy.Object);
        }


        [TestMethod]
        public void GetTransactionsRecent_ShouldCallProxy_Once()
        {
            const string testLowellReference = "anything123";

            // Arrange
            // Content does not matter, as straight pass through of objects
            var caseFlowDto = new RecievedTransactionsDto();

            _mockProxy.Setup(x => x.GetTransactionsAsync(testLowellReference, 5))
                .Returns(Task.FromResult(caseFlowDto));

            //Act
            var result = _process.GetTransactionsRecent(testLowellReference, 5).Result;

            //Assert
            Assert.AreEqual(caseFlowDto, result);

            _mockProxy.Verify(x => x.GetTransactionsAsync(testLowellReference, 5), Times.Once);
        }

    }
}
