using FinancialAccountService.Models.Validation;
using FluentValidation.TestHelper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;

namespace Validation.Unit.Tests
{
    [TestClass]
    public class CompleteRegistrationDtoValidatorTests
    {
        private CompleteRegistrationDtoValidator _completeRegistrationDtoValidator;

        [TestInitialize]
        public void Setup()
        {
            _completeRegistrationDtoValidator = new CompleteRegistrationDtoValidator();
        }

        [TestMethod]
        [DataRow("test@test.com", "Valid .com")]
        [DataRow("abc@abc.co.uk", "valid .co.uk")]
        [DataRow("benjamin.hopper@gmail.co.uk", "Email is valid")]
        [DataRow("benjamin.hopper@gmail.couk", "Email is valid")]
        [DataRow("benjamin.hopper@gmail.c", "Email is valid")]
        [DataRow("b_h@gmail.co.uk", "Email is valid")]
        [DataRow("benjaminhopper@benjaminhopper.co.uk", "Email is valid")]
        public void EmailAddress_CorrectEmail_ReturnsNoErrors(string dataRow, string message)
        {
            Trace.WriteLine(message);
            _completeRegistrationDtoValidator.ShouldNotHaveValidationErrorFor(x => x.EmailAddress, dataRow);
        }

        [TestMethod]
        [DataRow("test@test", "Isn't full email address")]
        [DataRow(null, "Is null")]
        [DataRow("", "is empty")]
        [DataRow("lowell@abc.co.uk", "Contains the word lowell")]
        [DataRow("benjamin.hopper@lowellgroup.co.uk", "Email cannot contain the word Lowell")]
        [DataRow("benjamin.hopper@lowell.co.uk", "Email cannot contain the word Lowell")]
        [DataRow("benjamin.lowell@group.co.uk", "Email cannot contain the word Lowell")]
        public void EmailAddress_IncorrectEmailAddress_ReturnsErrors(string dataRow, string message)
        {
            Trace.WriteLine(message);
            _completeRegistrationDtoValidator.ShouldHaveValidationErrorFor(x => x.EmailAddress, dataRow);
        }

        [TestMethod]
        [DataRow("abc", "valid lowell reference")]
        public void lowellReference_Correct_ReturnsNoErrors(string dataRow, string message)
        {
            Trace.WriteLine(message);
            _completeRegistrationDtoValidator.ShouldNotHaveValidationErrorFor(x => x.LowellReference, dataRow);
        }

        [TestMethod]
        [DataRow(null, "Null Callback url")]
        [DataRow("", "Incorrect Callback url")]
        public void CallBackUrl_Incorrect_ReturnsErrors(string dataRow, string message)
        {
            Trace.WriteLine(message);
            _completeRegistrationDtoValidator.ShouldHaveValidationErrorFor(x => x.LowellReference, dataRow);
        }

        [TestMethod]
        [DataRow(("62FA647C-AD54-4BCC-A860-E5A2664B019D"), "valid user id")]
        public void UserId_Correct_ReturnsNoErrors(string dataRow, string message)
        {
            Trace.WriteLine(message);
            _completeRegistrationDtoValidator.ShouldNotHaveValidationErrorFor(x => x.UserId, dataRow);
        }

        [TestMethod]
        [DataRow(null, "Null Callback url")]
        [DataRow("", "Incorrect Callback url")]
        public void UserId_Incorrect_ReturnsErrors(string dataRow, string message)
        {
            Trace.WriteLine(message);
            var guid = new Guid();
            _completeRegistrationDtoValidator.ShouldHaveValidationErrorFor(x => x.UserId, dataRow);
        }
    }
}
