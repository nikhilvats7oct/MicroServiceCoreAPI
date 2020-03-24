using FinancialAccountService.Models.DataTransferObjects;
using FinancialAccountService.Models.Validation;
using FinancialAccountService.Processes.Interfaces;
using FinancialAccountService.Services;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Services.Unit.Tests
{
    [TestClass]
    public class RegisterServiceTests
    {
        private RegisterService _registerService;
        private Mock<ILogger<RegisterService>> _mockLogger;
        private Mock<ICheckDataProtectionProcess> _mockDpaCheckProcess;
        private Mock<ICheckIsWebRegisteredProcess> _mockWebRegisteredProcess;
        private Mock<ISendRegistrationEmailProcess> _mockSendRegistrationEmailProcess;
        private Mock<ICompleteRegistrationProcess> _mockCompleteRegistrationProcess;
        private Mock<ICompleteDeregistrationProcess> _completeDeregistrationProcess;

        [TestInitialize]
        public void Initialize()
        {
            _mockLogger = new Mock<ILogger<RegisterService>>();
            _mockDpaCheckProcess = new Mock<ICheckDataProtectionProcess>();
            _mockWebRegisteredProcess = new Mock<ICheckIsWebRegisteredProcess>();
            _mockSendRegistrationEmailProcess = new Mock<ISendRegistrationEmailProcess>();
            _mockCompleteRegistrationProcess = new Mock<ICompleteRegistrationProcess>();
            _completeDeregistrationProcess = new Mock<ICompleteDeregistrationProcess>();


            _registerService = new RegisterService(_mockLogger.Object,
                                                    _mockDpaCheckProcess.Object,
                                                   _mockWebRegisteredProcess.Object,
                                                   _mockSendRegistrationEmailProcess.Object,
                                                   _mockCompleteRegistrationProcess.Object,
                _completeDeregistrationProcess.Object);

        }

        [TestMethod]
        public void SendRegistrationEmailAsync_ShouldCallSendRegistrationEmailAsync_Once()
        {
            //Arrange
            var registrationEmailDto = new RegistrationEmailDto()
            {
                EmailAddress = It.IsAny<string>(),
                CallBackUrl = It.IsAny<string>()
            };

            //Act
            var result = _registerService.SendRegistrationEmailAsync(registrationEmailDto);

            //Assert
            _mockSendRegistrationEmailProcess.Verify(x => x.SendRegistrationEmailAsync(registrationEmailDto), Times.Once);
        }


        [TestMethod]
        public void CompleteRegistrationAsync_ShouldCallCompleteRegistration_Once()
        {
            //Arrange
            var completeRegistrationDto = new CompleteRegistrationDto()
            {
                EmailAddress = It.IsAny<string>(),
                LowellReference = It.IsAny<string>(),
                UserId = It.IsAny<string>()
            };

            //Act
            var result = _registerService.CompleteRegistrationAsync(completeRegistrationDto);

            //Assert
            _mockCompleteRegistrationProcess.Verify(x => x.CompleteRegistrationProcessAsync(completeRegistrationDto), Times.Once);
        }

        [TestMethod]
        public void Validate_DpaCheckPasses_ReturnsResultIsSucessfulTrue()
        {
            var dto = new RegisterValidationDto();

            _mockDpaCheckProcess.Setup(x => x.CheckDataProtection(It.IsAny<RegisterValidationDto>())).ReturnsAsync(true);

            var result = _registerService.CheckDataProtectionAsync(dto);

            Assert.IsTrue(result.Result.IsSuccessful);
        }

        [TestMethod]
        public void Validate_DpaCheckFails_ReturnsResultIsSucessfulFalse()
        {
            var dto = new RegisterValidationDto();

            _mockDpaCheckProcess.Setup(x => x.CheckDataProtection(It.IsAny<RegisterValidationDto>())).ReturnsAsync(false);

            var result = _registerService.CheckDataProtectionAsync(dto);

            Assert.IsFalse(result.Result.IsSuccessful);
            Assert.AreEqual(ValidationMessages.DataProtectionCheckFailed, result.Result.MessageForUser);
        }

        [TestMethod]
        public void Validate_DpaCheckFails_And_WebRegistedCheckPasses_ReturnsResultIsSucessfulFalse()
        {
            var dto = new WebRegisteredDto();

            _mockWebRegisteredProcess.Setup(x => x.CheckHasWebAccountAsync(It.IsAny<WebRegisteredDto>())).ReturnsAsync(true);

            var result = _registerService.CheckIsWebRegisteredAsync(dto);

            Assert.IsFalse(result.Result.IsSuccessful);
        }

        [Ignore]
        [TestMethod]
        public void Validate_DpaCheckPasses_And_WebRegistedCheckFails_ReturnsResultIsSucessfulFalse()
        {
            var dto = new WebRegisteredDto();

            _mockDpaCheckProcess.Setup(x => x.CheckDataProtection(It.IsAny<RegisterValidationDto>())).ReturnsAsync(true);
            _mockWebRegisteredProcess.Setup(x => x.CheckHasWebAccountAsync(It.IsAny<WebRegisteredDto>())).ReturnsAsync(false);

            var result = _registerService.CheckIsWebRegisteredAsync(dto);

            Assert.IsFalse(result.Result.IsSuccessful);
        }

        [TestMethod]
        public void SCompleteDeregistrationAsyncc_CompleteDeregistrationAsync_Once()
        {
            //Arrange
            var deregistrationDto = new CompleteDeregistrationDto()
            {

            };

            //Act
            var result = _registerService.CompleteDeregistrationAsync(deregistrationDto);

            //Assert
            _completeDeregistrationProcess.Verify(x => x.CompleteDeregistrationAsync(deregistrationDto), Times.Once);
        }
    }
}
