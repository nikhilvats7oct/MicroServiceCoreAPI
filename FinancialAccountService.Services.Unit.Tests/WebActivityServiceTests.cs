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
    public class WebActivityServiceTests
    {
        private Mock<ILogWebActivityProcess> _mockProcess;
        private WebActivityService _service;

        [TestInitialize]
        public void Initialise()
        {
            _mockProcess = new Mock<ILogWebActivityProcess>();
            _service = new WebActivityService(_mockProcess.Object);
        }

        [TestMethod]
        public void LogWebActivityTest()
        {
            WebActivityDto webActivity = new WebActivityDto()
            {
                LowellReference = "123456789",
                Company = 0,
                DateTime = DateTime.Now,
                Guid = Guid.NewGuid().ToString(),
                WebActionID = 123
            };

            _mockProcess.Setup(x => x.LogWebActivity(webActivity)).Returns(Task.CompletedTask);

            Task result = _service.LogWebActivity(webActivity);

            Assert.AreEqual(TaskStatus.RanToCompletion, result.Status);

            _mockProcess.Verify(x => x.LogWebActivity(webActivity), Times.Once);
        }

    }
}
