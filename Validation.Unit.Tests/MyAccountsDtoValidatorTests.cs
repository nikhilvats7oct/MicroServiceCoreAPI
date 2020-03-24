using FinancialAccountService.Models.DataTransferObjects;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Validation.Unit.Tests._Utility;

namespace Validation.Unit.Tests
{
    [TestClass]
    public class MyAccountsDtoValidatorTests
    {
        private IValidator<MyAccountsDto> _validator;

        [TestInitialize]
        public void Initialise()
        {
            _validator = ModelValidatorInstantiator.GetValidatorFromModelAttribute<MyAccountsDto>();
        }

        [TestMethod]
        [DataRow("00000000-0000-0000-0000-000000000000")]
        [DataRow("2af83e33-fee9-4926-aae4-6634c005e379")]
        [DataRow("91a92e0f-d392-4b34-b1b7-415e91d27331")]
        [DataRow("56529441-d979-462b-b021-64f044b43a60")]
        public void UserId_WhenValidateViaAttribute_WithValidGuid_Then_ReturnsNoError(string testUserId)
        {
            MyAccountsDto model = new MyAccountsDto();

            model.UserId = testUserId;

            {
                ValidationResult result = _validator.Validate(model);
                Assert.AreEqual(true, result.IsValid);
                Assert.AreEqual(0, result.Errors.Count);
            }
        }

        [TestMethod]
        [DataRow("00000000-0000-0000-0000-00000000000")]
        [DataRow("0000000-0000-0000-0000-000000000000")]
        [DataRow("wah")]
        [DataRow("")]
        [DataRow("56529441-d979-462b-b021-64f044b43a6G")]   // note G at end (should be hex: 0-9, A-F)
        public void UserId_WhenValidateViaAttribute_WithInvalidGuid_Then_ReturnsError(string testUserId)
        {
            MyAccountsDto model = new MyAccountsDto();

            model.UserId = testUserId;

            {
                ValidationResult result = _validator.Validate(model);
                Assert.AreEqual(false, result.IsValid);
                Assert.AreEqual(1, result.Errors.Count);
                Assert.AreEqual("UserId", result.Errors[0].PropertyName);
                Assert.AreEqual("Must be a GUID.", result.Errors[0].ErrorMessage);
            }
        }
    }
}
