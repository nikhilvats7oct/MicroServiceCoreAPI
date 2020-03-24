using FinancialAccountService.Models.DataTransferObjects;
using FinancialAccountService.Processes.Interfaces;
using FinancialAccountService.Services;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Threading.Tasks;

namespace Services.Unit.Tests
{
    [TestClass]
    public class UsersServiceTests
    {
        private Mock<ILogger<UsersService>> _mockLogger;
        private Mock<IGetUserProcess> _mockGetUserProcess;
        private UsersService _service;

        [TestInitialize]
        public void Initialise()
        {
            _mockLogger = new Mock<ILogger<UsersService>>();
            _mockGetUserProcess = new Mock<IGetUserProcess>(MockBehavior.Strict);
            _service = new UsersService(_mockLogger.Object, _mockGetUserProcess.Object);
        }

        [TestMethod]
        public void GetUser_Test_ShouldCallProcessOnce()
        {
            CreatedUserDto user = new CreatedUserDto()
            {
                Company = 1,
                EmailAddress = "stewart.hartley@lowellgroup.co.uk",
                LowellReference = "123456789",
                IsSuccessful = true,
                MessageForUser = "hello"
            };

            string userId = Guid.NewGuid().ToString();

            _mockGetUserProcess.Setup(x => x.GetUser(userId)).Returns(Task.FromResult(user));

            CreatedUserDto result = _service.Get(userId).Result;

            Assert.AreEqual(user, result);

            //Check that the process was called once
            _mockGetUserProcess.Verify(x => x.GetUser(userId), Times.Once);
            _mockGetUserProcess.VerifyNoOtherCalls();
        }

    }
}
