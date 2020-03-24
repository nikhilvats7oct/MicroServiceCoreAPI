using FinancialAccountService.Models.DataTransferObjects;
using FinancialAccountService.Processes;
using FinancialAccountService.Proxy.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;

namespace Processes.Unit.Tests
{
    [TestClass]
    public class CheckDataProtectionProcessTests
    {
        private Mock<ILogger<CheckDataProtectionProcess>> _mockLogger;
        private CheckDataProtectionProcess _checkDataProtectionProcess;
        private Mock<ICaseflowApiProxy> _mockCaseflowApiProxy;

        [TestInitialize]
        public void SetUp()
        {
            _mockLogger = new Mock<ILogger<CheckDataProtectionProcess>>();
            _mockCaseflowApiProxy = new Mock<ICaseflowApiProxy>();
            _checkDataProtectionProcess = new CheckDataProtectionProcess(_mockLogger.Object, _mockCaseflowApiProxy.Object);
        }

        [TestMethod]
        public void ValidateDpa_WhenCheckDpaReturnsMatchingPostCodeDateOfBirthAndLowellReference_ReturnsTrue()
        {
            var dto = new RegisterValidationDto()
            {
                DateOfBirth = DateTime.Parse("1985-11-10"),
                LowellReference = "100262575",
                Postcode = "LS01 0NE"
            };

            var model = new RecievedAccountDto()
            {
                DateOfBirth = "1985-11-10",
                LowellReference = "100262575",
                Postcode = "LS01 0NE"
            };

            _mockCaseflowApiProxy.Setup(x => x.CheckDataProtection(dto)).ReturnsAsync(model);

            var result = _checkDataProtectionProcess.CheckDataProtection(dto).Result;

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void ValidateDpa_WhenCheckDpaReturnsDifferentPostCode_ReturnsFalse()
        {
            var dto = new RegisterValidationDto()
            {
                DateOfBirth = DateTime.Parse("1985-11-10"),
                LowellReference = "100262575",
                Postcode = "LS01 0NE"
            };

            var model = new RecievedAccountDto()
            {
                DateOfBirth = "1985-11-10",
                LowellReference = "100262575",
                Postcode = "BD9 6AE"
            };

            _mockCaseflowApiProxy.Setup(x => x.CheckDataProtection(dto)).ReturnsAsync(model);

            var result = _checkDataProtectionProcess.CheckDataProtection(dto).Result;

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void ValidateDpa_WhenCheckDpaReturnsDifferentLowellReference_ReturnsFalse()
        {
            var dto = new RegisterValidationDto()
            {
                DateOfBirth = DateTime.Parse("1985-11-10"),
                LowellReference = "100262575",
                Postcode = "LS01 0NE"
            };

            var model = new RecievedAccountDto()
            {
                DateOfBirth = "1985-11-10",
                LowellReference = "100291640",
                Postcode = "LS01 0NE"
            };

            _mockCaseflowApiProxy.Setup(x => x.CheckDataProtection(dto)).ReturnsAsync(model);

            var result = _checkDataProtectionProcess.CheckDataProtection(dto).Result;

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void ValidateDpa_WhenCheckDpaReturnsDifferentDateOfBirth_ReturnsFalse()
        {
            var dto = new RegisterValidationDto()
            {
                DateOfBirth = DateTime.Parse("1985-11-10"),
                LowellReference = "100262575",
                Postcode = "LS01 0NE"
            };

            var model = new RecievedAccountDto()
            {
                DateOfBirth = "1979-07-01",
                LowellReference = "100262575",
                Postcode = "LS01 0NE"
            };

            _mockCaseflowApiProxy.Setup(x => x.CheckDataProtection(dto)).ReturnsAsync(model);

            var result = _checkDataProtectionProcess.CheckDataProtection(dto).Result;

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void ValidateDpa_WhenCheckDpaReturnsSamePostCodeInUpperCase_ReturnsTrue()
        {
            var dto = new RegisterValidationDto()
            {
                DateOfBirth = DateTime.Parse("1985-11-10"),
                LowellReference = "100262575",
                Postcode = "ls01 0ne"
            };

            var model = new RecievedAccountDto()
            {
                DateOfBirth = "1985-11-10",
                LowellReference = "100262575",
                Postcode = "LS01 0NE"
            };

            _mockCaseflowApiProxy.Setup(x => x.CheckDataProtection(dto)).ReturnsAsync(model);

            var result = _checkDataProtectionProcess.CheckDataProtection(dto).Result;

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void ValidateDpa_WhenCheckDpaReturnsSamePostCodeInLowerCase_ReturnsTrue()
        {
            var dto = new RegisterValidationDto()
            {
                DateOfBirth = DateTime.Parse("1985-11-10"),
                LowellReference = "100262575",
                Postcode = "LS01 0NE"
            };

            var model = new RecievedAccountDto()
            {
                DateOfBirth = "1985-11-10",
                LowellReference = "100262575",
                Postcode = "ls01 0ne"
            };

            _mockCaseflowApiProxy.Setup(x => x.CheckDataProtection(dto)).ReturnsAsync(model);

            var result = _checkDataProtectionProcess.CheckDataProtection(dto).Result;

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void ValidateDpa_WhenCheckDpaReturnsSamePostCodeWithSpace_ReturnsTrue()
        {
            var dto = new RegisterValidationDto()
            {
                DateOfBirth = DateTime.Parse("1985-11-10"),
                LowellReference = "100262575",
                Postcode = "LS010NE"
            };

            var model = new RecievedAccountDto()
            {
                DateOfBirth = "1985-11-10",
                LowellReference = "100262575",
                Postcode = "LS01 0NE"
            };

            _mockCaseflowApiProxy.Setup(x => x.CheckDataProtection(dto)).ReturnsAsync(model);

            var result = _checkDataProtectionProcess.CheckDataProtection(dto).Result;

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void ValidateDpa_WhenCheckDpaReturnsSamePostCodeWithoutSpace_ReturnsTrue()
        {
            var dto = new RegisterValidationDto()
            {
                DateOfBirth = DateTime.Parse("1985-11-10"),
                LowellReference = "100262575",
                Postcode = "LS01 0NE"
            };

            var model = new RecievedAccountDto()
            {
                DateOfBirth = "1985-11-10",
                LowellReference = "100262575",
                Postcode = "LS010NE"
            };

            _mockCaseflowApiProxy.Setup(x => x.CheckDataProtection(dto)).ReturnsAsync(model);

            var result = _checkDataProtectionProcess.CheckDataProtection(dto).Result;

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void ValidateDpa_WhenCheckDpaReturnsNull_ReturnsTrue()
        {
            var dto = new RegisterValidationDto()
            {
                DateOfBirth = DateTime.Parse("1985-11-10"),
                LowellReference = "100262575",
                Postcode = "LS01 0NE"
            };

            var model = new RecievedAccountDto();
            model = null;


            _mockCaseflowApiProxy.Setup(x => x.CheckDataProtection(dto)).ReturnsAsync(model);

            var result = _checkDataProtectionProcess.CheckDataProtection(dto).Result;

            Assert.IsFalse(result);
        }

    }
}
