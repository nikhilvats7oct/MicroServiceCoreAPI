using System.Threading;
using System.Threading.Tasks;
using FinancialAccountService.Models.DataTransferObjects;
using FinancialAccountService.Processes;
using FinancialAccountService.Processes.Interfaces;
using FinancialAccountService.Proxy.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Processes.Unit.Tests
{
    [TestClass]
    public class CaseflowHeartbeatProcessTests
    {
        private Mock<ILogger<CaseflowHeartbeatProcess>> _loggerMock;
        private Mock<ICaseflowApiProxy> _caseflowApiProxyMock;

        private ICaseflowHeartbeatProcess _caseflowHeartbeatProcess;

        [TestInitialize]
        public void Initialise()
        {
            _loggerMock = new Mock<ILogger<CaseflowHeartbeatProcess>>(MockBehavior.Strict);
            _caseflowApiProxyMock = new Mock<ICaseflowApiProxy>(MockBehavior.Strict);

            _caseflowHeartbeatProcess = new CaseflowHeartbeatProcess(
                _loggerMock.Object,
                _caseflowApiProxyMock.Object);
        }

        [TestMethod]
        public void CallHeartbeatAsync_CallsHeartbeatProxyAndPopulatesDtoWithTimeSpan()
        {
            CaseflowPingDto dtoReturnedFromApi = new CaseflowPingDto();

            _caseflowApiProxyMock.Setup(x => x.HitHeartBeat())
                .Callback(() =>
                {
                    Thread.Sleep(50);
                })
                .ReturnsAsync(dtoReturnedFromApi);

            HeartBeatDto result = _caseflowHeartbeatProcess.CallHeartbeatAsync().Result;

            _caseflowApiProxyMock.Verify(x => x.HitHeartBeat(), Times.Once);

            Assert.AreEqual("Caseflow Ping From Account Service", result.ServiceName);
            Assert.AreEqual(null, result.Details);

            Assert.IsTrue(result.RunningElapsedTime.TotalMilliseconds >= 50);
            Assert.IsTrue(result.TotalElapsedTime.TotalMilliseconds >= 50);
            Assert.AreEqual(result.RunningElapsedTime, result.TotalElapsedTime);    // same

            Assert.AreEqual(null, result.Status);
            Assert.AreEqual(null, result.ChildHeartBeat);
        }

    }
}
