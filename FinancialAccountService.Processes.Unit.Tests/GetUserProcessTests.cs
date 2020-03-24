using FinancialAccountService.Models.DataTransferObjects;
using FinancialAccountService.Processes;
using FinancialAccountService.Proxy.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Threading.Tasks;

namespace Processes.Unit.Tests
{
    [TestClass]
    public class GetUserProcessTests
    {
        private Mock<ILogger<GetUserProcess>> _mockLogger;
        private Mock<ICaseflowApiProxy> _mockProxy;
        private GetUserProcess _process;

        [TestInitialize]
        public void Initialise()
        {
            _mockLogger = new Mock<ILogger<GetUserProcess>>();
            _mockProxy = new Mock<ICaseflowApiProxy>(MockBehavior.Strict);
            _process = new GetUserProcess(_mockLogger.Object, _mockProxy.Object);
        }

        [TestMethod]
        public void GetUser_Test_Returns_Result_of_Proxy()
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

            _mockProxy.Setup(x => x.GetUserAsync(userId)).Returns(Task.FromResult(user));

            CreatedUserDto result = _process.GetUser(userId).Result;

            Assert.AreEqual(user, result);

            //Check that the proxy was called once
            _mockProxy.Verify(x => x.GetUserAsync(userId), Times.Once);
            _mockProxy.VerifyNoOtherCalls();
        }

    }
}
