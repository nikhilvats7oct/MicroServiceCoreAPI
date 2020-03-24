using FinancialAccountService.Models.DataTransferObjects;
using FinancialAccountService.Processes;
using FinancialAccountService.Proxy.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Processes.Unit.Tests
{
    [TestClass]
    public class CompleteRegistrationProcessTests
    {
        private Mock<ILogger<CompleteRegistrationProcess>> _mockLogger;
        private CompleteRegistrationProcess _process;
        private Mock<ICaseflowApiProxy> _mockApi;

        [TestInitialize]
        public void SetUp()
        {
            _mockLogger = new Mock<ILogger<CompleteRegistrationProcess>>();
            _mockApi = new Mock<ICaseflowApiProxy>();
            _process = new CompleteRegistrationProcess(_mockLogger.Object, _mockApi.Object);
        }

        [Ignore]
        [TestMethod]
        public void CompleteRegistrationProcessAsync_SendsDataToAp_ReturnsResults()
        {
            var dto = new CompleteRegistrationDto()
            {
                EmailAddress = "Test@test.com",
                LowellReference = "100262575",
                Company = 1,
                UserId = new Guid().ToString()
            };

            var result = _process.CompleteRegistrationProcessAsync(dto);

            Assert.IsInstanceOfType(result, typeof(Task<CreatedUserDto>));

            _mockApi.Verify(x => x.CompleteRegistrationAsync(dto, It.IsAny<string>()),Times.Once);
        }
    }
}
