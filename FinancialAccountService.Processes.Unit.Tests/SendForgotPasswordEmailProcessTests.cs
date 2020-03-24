using FinancialAccountService.Models.DataTransferObjects;
using FinancialAccountService.Processes;
using FinancialAccountService.Proxy.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;

namespace Processes.Unit.Tests
{
    [TestClass]
    public class SendForgotPasswordEmailProcessTests
    {
        private Mock<ILogger<SendForgotPasswordEmailProcess>> _mockLogger;
        private SendForgotPasswordEmailProcess _process;
        private Mock<ICaseflowApiProxy> _mockApi;

        [TestInitialize]
        public void SetUp()
        {
            _mockLogger = new Mock<ILogger<SendForgotPasswordEmailProcess>>();
            _mockApi = new Mock<ICaseflowApiProxy>();
            _process = new SendForgotPasswordEmailProcess(_mockLogger.Object, _mockApi.Object);
        }

        [TestMethod]
        public void SendForgotPasswordAsync_CallsSendForgotPasswordAsync_Once()
        {
            var dto = new ForgotPasswordDto()
            {
                CallBackUrl = "https://www.lowell.co.uk",
                EmailAddress = "Test@test.com",
                UserId = "100262575",
                LowellRef = "12345678"
            };

            var forgottenPassword = new SendForgottenPasswordDto()
            {
                EmailAddress = "Test@test.com",
                Data = new List<SendForgottenPasswordDto.DataItem>()
                {
                    new SendForgottenPasswordDto.DataItem { Value = "https://www.lowell.co.uk" }
                }
            };

            _process.SendForgotPasswordAsync(dto);

            Assert.AreEqual(1, _mockApi.Invocations.Count);
            Assert.AreEqual("SendForgotPasswordAsync", _mockApi.Invocations[0].Method.Name);
        }
    }
}
