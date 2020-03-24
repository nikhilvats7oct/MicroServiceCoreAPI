using FinancialAccountService.Models.DataTransferObjects;
using FinancialAccountService.Processes;
using FinancialAccountService.Proxy.Interfaces;
using GenFu;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Linq;

namespace Processes.Unit.Tests
{
    [TestClass]
    public class CheckIsWebRegisteredProcessTests
    {
        private Mock<ILogger<CheckIsWebRegisteredProcess>> _mockLogger;
        private CheckIsWebRegisteredProcess _process;
        private Mock<ICaseflowApiProxy> _mockApi;

        [TestInitialize]
        public void SetUp()
        {
            _mockLogger = new Mock<ILogger<CheckIsWebRegisteredProcess>>();
            _mockApi = new Mock<ICaseflowApiProxy>();
            _process = new CheckIsWebRegisteredProcess(_mockLogger.Object, _mockApi.Object);
        }

        [Ignore]
        [TestMethod]
        public void ValidateWebRegisteredCheck_ChecksThatDataReturnedHasNoWebAccountRegistered_ReturnsTrue()
        {
            var dto = new WebRegisteredDto()
            {
                LowellReference = "100262575"
            };

            var recievedCustomerSummariesDto = new RecievedCustomerSummariesDto();

            GenFu.GenFu.Configure<RecievedCustomerSummaryDto>()
                .Fill(x => x.LowellReference, () => { return "100262575"; })
                .Fill(x => x.HasWebAccount, () => { return false; });

            var customerSummaries = A.ListOf<RecievedCustomerSummaryDto>(20);

            recievedCustomerSummariesDto.Summaries = customerSummaries;

            _mockApi.Setup(x => x.CheckHasWebAccountAsync(dto)).ReturnsAsync(recievedCustomerSummariesDto);

            var result = _process.CheckHasWebAccountAsync(dto).Result;

            Assert.IsTrue(result);
        }

        [Ignore]
        [TestMethod]
        public void ValidateWebRegisteredCheck_ChecksThatDataReturnedHasNoWebAccountRegistered_ReturnsFalse()
        {
            var dto = new WebRegisteredDto()
            {

                LowellReference = "100262575"

            };

            var recievedCustomerSummariesDto = new RecievedCustomerSummariesDto();

            GenFu.GenFu.Configure<RecievedCustomerSummaryDto>()
                .Fill(x => x.LowellReference, () => { return "100262575"; })
                .Fill(x => x.HasWebAccount, () => { return false; });

            var customerSummaries = A.ListOf<RecievedCustomerSummaryDto>(20);

            GenFu.GenFu.Configure<RecievedCustomerSummaryDto>()
                .Fill(x => x.LowellReference, () => { return "100262575"; })
                .Fill(x => x.HasWebAccount, () => { return true; });

            var customerSummary = A.ListOf<RecievedCustomerSummaryDto>(1);

            recievedCustomerSummariesDto.Summaries = customerSummaries;

            recievedCustomerSummariesDto.Summaries.Add(customerSummary.First());

            _mockApi.Setup(x => x.CheckHasWebAccountAsync(dto)).ReturnsAsync(recievedCustomerSummariesDto);

            var result = _process.CheckHasWebAccountAsync(dto).Result;

            Assert.IsFalse(result);
        }
    }
}
