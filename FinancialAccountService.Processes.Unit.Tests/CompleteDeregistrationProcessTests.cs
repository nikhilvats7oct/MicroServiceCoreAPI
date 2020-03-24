using System;
using System.Collections.Generic;
using System.Text;
using FinancialAccountService.Models.DataTransferObjects;
using FinancialAccountService.Processes;
using FinancialAccountService.Proxy.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Processes.Unit.Tests
{
    [TestClass]
    public class CompleteDeregistrationProcessTests
    {

        private Mock<ILogger<CompleteDeregistrationProcess>> _mockLogger;
        private Mock<ICaseflowApiProxy> _caseflowApiProxy;
        private CompleteDeregistrationProcess _complaCompleteDeregistrationProcess;

        [TestInitialize]
        public void Initialize()
        {
            _mockLogger = new Mock<ILogger<CompleteDeregistrationProcess>>();
            _caseflowApiProxy = new Mock<ICaseflowApiProxy>();
            _complaCompleteDeregistrationProcess = new CompleteDeregistrationProcess(_mockLogger.Object,_caseflowApiProxy.Object);
        }

        [TestMethod]
        public void CompleteDeregistrationAsync_CallsCompleteDeregistrationAsync_Once()
        {
            //Arrange
            var deregistrationDto = new CompleteDeregistrationDto()
            {
                ReplayId = "124567"
            };

            //Act
            _complaCompleteDeregistrationProcess.CompleteDeregistrationAsync(deregistrationDto);

            //Assert
            _caseflowApiProxy.Verify(x => x.CompleteDeregistrationAsync(deregistrationDto, deregistrationDto.ReplayId), Times.Once);
        }
    }
}
