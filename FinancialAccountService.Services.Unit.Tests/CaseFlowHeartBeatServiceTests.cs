using System;
using FinancialAccountService.Models.DataTransferObjects;
using FinancialAccountService.Processes.Interfaces;
using FinancialAccountService.Services;
using FinancialAccountService.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Services.Unit.Tests
{
    [TestClass]
    public class CaseFlowHeartBeatServiceTests
    {
        private Mock<ILogger<CaseflowHeartbeatService>> _loggerMock;
        private Mock<ICaseflowHeartbeatProcess> _caseflowHeartbeatProcessMock;

        private ICaseflowHeartbeatService _caseflowHeartbeatService;

        [TestInitialize]
        public void Initialise()
        {
            _loggerMock = new Mock<ILogger<CaseflowHeartbeatService>>();
            _caseflowHeartbeatProcessMock = new Mock<ICaseflowHeartbeatProcess>(MockBehavior.Strict);

            _caseflowHeartbeatService = new CaseflowHeartbeatService(
                _loggerMock.Object,
                _caseflowHeartbeatProcessMock.Object);
        }

        [TestMethod]
        public void CallHeartbeatAsync_CallsProxyAndReturnsDto()
        {
            HeartBeatDto dtoReturned = new HeartBeatDto();

            _caseflowHeartbeatProcessMock.Setup(x => x.CallHeartbeatAsync())
                .ReturnsAsync(dtoReturned);

            HeartBeatDto result = _caseflowHeartbeatService.CallHeartbeatAsync().Result;

            _caseflowHeartbeatProcessMock.Verify(x => x.CallHeartbeatAsync(), Times.Once);

            Assert.AreEqual(dtoReturned, result);
        }

    }
}
