using FinancialAccountService.Models.Validation;
using FluentValidation.TestHelper;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Validation.Unit.Tests
{
    [TestClass]
    public class CompleteDeregistrationDtoValidationTests
    {
        private CompleteDeregistrationDtoValidation _completeDeregistrationDtoValidation;

        [TestInitialize]
        public void Setup()
        {
            _completeDeregistrationDtoValidation = new CompleteDeregistrationDtoValidation();
        }

        [TestMethod]
        [DataRow("blah")]
        public void UserId_Correct_ReturnsNoErrors(string dataRow)
        {
            _completeDeregistrationDtoValidation.ShouldNotHaveValidationErrorFor(x => x.UserId, dataRow);
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        public void UserId_Incorrect_ReturnsErrors(string dataRow)
        {
            _completeDeregistrationDtoValidation.ShouldHaveValidationErrorFor(x => x.UserId, dataRow);
        }
    }
}
