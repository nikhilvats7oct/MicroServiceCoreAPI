using FinancialAccountService.Models.Validation;
using FluentValidation.TestHelper;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Validation.Unit.Tests
{
    [TestClass]
    public class RegistrationEmailDtoValidatorTests
    {
        private RegistrationEmailDtoValidator _registrationEmailDtoValidator;

        [TestInitialize]
        public void Setup()
        {
            _registrationEmailDtoValidator = new RegistrationEmailDtoValidator();
        }

        [TestMethod]
        [DataRow("test@test.com", "Valid .com")]
        [DataRow("abc@abc.co.uk", "valid .co.uk")]
        public void EmailAddress_CorrectEmail_ReturnsNoErrors(string dataRow, string message)
        {
            _registrationEmailDtoValidator.ShouldNotHaveValidationErrorFor(x => x.EmailAddress, dataRow);
        }


        [TestMethod]
        [DataRow("test@test", "Isn't full email address")]
        [DataRow(null, "Is null")]
        [DataRow("", "is empty")]
        [DataRow("lowell@abc.co.uk", "Contains the word lowell")]
        public void EmailAddress_IncorrectEmailAddress_ReturnsErrors(string dataRow, string message)
        {
            _registrationEmailDtoValidator.ShouldHaveValidationErrorFor(x => x.EmailAddress, dataRow);
        }

        [TestMethod]
        [DataRow("some data", "Valid string")]
        public void CallBackUrl_Correct_ReturnsNoErrors(string dataRow, string message)
        {
            _registrationEmailDtoValidator.ShouldNotHaveValidationErrorFor(x => x.CallBackUrl, dataRow);
        }

        [TestMethod]
        [DataRow(null, "Null Callback url")]
        [DataRow("", "Incorrect Callback url")]
        public void CallBackUrl_Incorrect_ReturnsNoErrors(string dataRow, string message)
        {
            _registrationEmailDtoValidator.ShouldHaveValidationErrorFor(x => x.CallBackUrl, dataRow);
        }
    }
}
