using FinancialAccountService.Models.DataTransferObjects;
using FinancialAccountService.Processes.Interfaces;
using FinancialAccountService.Proxy.Interfaces;
using FinancialAccountService.Services;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Services.Unit.Tests
{
    [TestClass]
    public class ForgotPasswordServiceTests
    {
        private Mock<ILogger<ForgotPasswordService>> _mockLogger;
        private ForgotPasswordService _forgotPasswordService;
        private Mock<ISendForgotPasswordEmailProcess> _sendForgotPasswordEmailProcess;

        private ForgotPasswordDto _forgotPasswordDto;

        [TestInitialize]
        public void Initialize()
        {
            _mockLogger = new Mock<ILogger<ForgotPasswordService>>();
            _sendForgotPasswordEmailProcess = new Mock<ISendForgotPasswordEmailProcess>();
            _forgotPasswordService = new ForgotPasswordService(_mockLogger.Object, _sendForgotPasswordEmailProcess.Object);
            _forgotPasswordDto = new ForgotPasswordDto()
            {
                EmailAddress = It.IsAny<string>(),
                CallBackUrl = It.IsAny<string>(),
                UserId = It.IsAny<string>(),
                LowellRef = It.IsAny<string>()
            };
        }

        [TestMethod]
        public void SendForgotPasswordAsync_ShouldCallProxy_Once()
        {
            //Act
            var result = _forgotPasswordService.SendForgotPasswordAsync(_forgotPasswordDto);

            //Assert
            _sendForgotPasswordEmailProcess.Verify(x => x.SendForgotPasswordAsync(_forgotPasswordDto), Times.Once);
        }
    }
}