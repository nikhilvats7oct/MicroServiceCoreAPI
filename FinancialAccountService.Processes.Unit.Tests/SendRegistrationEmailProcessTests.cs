using FinancialAccountService.Models;
using FinancialAccountService.Models.DataTransferObjects;
using FinancialAccountService.Processes;
using FinancialAccountService.Proxy.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Processes.Unit.Tests
{
    [TestClass]
    public class SendRegistrationEmailProcessTests
    {
        private Mock<ILogger<SendRegistrationEmailProcess>> _mockLogger;
        private SendRegistrationEmailProcess _process;
        private Mock<ICaseflowApiProxy> _mockApi;

        [TestInitialize]
        public void SetUp()
        {
            _mockLogger = new Mock<ILogger<SendRegistrationEmailProcess>>();
            _mockApi = new Mock<ICaseflowApiProxy>();
            _process = new SendRegistrationEmailProcess(_mockLogger.Object, _mockApi.Object);
        }

        [Ignore]
        [TestMethod]
        public void SendRegistrationEmailAsync_CreatesASendEmailDtoObjectAndSendsToCaseflow_ApiReturnsResultDto()
        {
            var dto = new RegistrationEmailDto()
            {
                CallBackUrl = "https://www.lowell.co.uk",
                EmailAddress = "Test@test.com",
                LowellReference = "100262575"
            };

            var sendEmail = new SendEmailDto()
            {
                EmailAddress = "Test@test.com",
                Data = new List<SendEmailDto.DataItem>()
                {
                    new SendEmailDto.DataItem { Value = "https://www.lowell.co.uk" }
                }
            };

             _process.SendRegistrationEmailAsync(dto);
            _mockApi.Verify(x => x.SendRegistrationEmailAsync(sendEmail, It.IsAny<string>(), It.IsAny<string>()));
        }
    }
}
