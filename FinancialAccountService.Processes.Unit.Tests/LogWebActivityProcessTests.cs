using FinancialAccountService.Models.DataTransferObjects;
using FinancialAccountService.Processes;
using FinancialAccountService.Proxy.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Threading.Tasks;

namespace Processes.Unit.Tests
{
    [TestClass]
    public class LogWebActivityProcessTests
    {
        private Mock<ICaseflowApiProxy> _mockCaseFlowProxy;
        private LogWebActivityProcess _process;

        [TestInitialize]
        public void Initialise()
        {
            _mockCaseFlowProxy = new Mock<ICaseflowApiProxy>(MockBehavior.Strict);
            _process = new LogWebActivityProcess(_mockCaseFlowProxy.Object);
        }

        [TestMethod]
        public void LogWebActivityTest()
        {
            WebActivityDto webActivity = new WebActivityDto()
            {
                LowellReference = "123456789",
                Company = 0,
                DateTime = DateTime.Now,
                Guid = Guid.NewGuid().ToString(),
                WebActionID = 123
            };

            _mockCaseFlowProxy.Setup(x => x.LogWebActivity(webActivity)).Returns(Task.CompletedTask);

            Task result = _process.LogWebActivity(webActivity);

            Assert.AreEqual(TaskStatus.RanToCompletion, result.Status);

            _mockCaseFlowProxy.Verify(x => x.LogWebActivity(webActivity), Times.Once);
        }

    }
}
