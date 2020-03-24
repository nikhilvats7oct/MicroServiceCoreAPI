using System;
using System.Collections.Generic;
using System.Text;
using FinancialAccountService.Models.DataTransferObjects;
using FinancialAccountService.Models.Validation;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Validation.Unit.Tests._Utility;

namespace Validation.Unit.Tests
{
    [TestClass]
    public class MyAccountsDetailDtoValidatorTests
    {
        IValidator<MyAccountsDetailDto> _validator;
        private MyAccountsDetailDto _model;

        [TestInitialize]
        public void Initialise()
        {
            _validator = ModelValidatorInstantiator.GetValidatorFromModelAttribute<MyAccountsDetailDto>();

            // Contains sufficient setup for a model to be valid
            // Tests will change this to be invalid and create error states
            _model = new MyAccountsDetailDto();
        }

        [TestMethod]
        [DataRow("0")] // minimal
        [DataRow("012345678")] // less than 10
        [DataRow("0123456789")] // exactly 10
        public void LowellReference_WhenValidateViaAttribute_WithValidEntry_UpToTenCharacters_Then_ReturnsNoError(
            string testLowellRef)
        {
            _model.LowellReference = testLowellRef;

            Assert.IsTrue(_model.LowellReference.Length <= 10);

            {
                ValidationResult result = _validator.Validate(_model);
                Assert.AreEqual(true, result.IsValid);
                Assert.AreEqual(0, result.Errors.Count);
            }
        }

        [TestMethod]
        [DataRow("0123456789A")] // 11 chars
        [DataRow("0123456789AB")] // 12 chars
        public void LowellReference_WhenValidateViaAttribute_WithInvalidEntry_MoreThanTenCharacters_Then_ReturnsError(
            string testLowellRef)
        {
            _model.LowellReference = testLowellRef;

            Assert.IsTrue(_model.LowellReference.Length > 10);

            {
                ValidationResult result = _validator.Validate(_model);
                Assert.AreEqual(false, result.IsValid);
                {
                    var errors = result.Errors;
                    Assert.AreEqual(1, errors.Count);
                    Assert.AreEqual("Invalid Lowell reference", errors[0].ErrorMessage);
                }
            }
        }

        [TestMethod]
        [DataRow("abcde")] // alpha lowercase
        [DataRow("fghij")]
        [DataRow("klmno")]
        [DataRow("pqrst")]
        [DataRow("uvwxyz")]

        [DataRow("ABCDE")] // alpha uppercase
        [DataRow("FGHIJ")]
        [DataRow("KLMNO")]
        [DataRow("PQRST")]
        [DataRow("UVWXYZ")]

        [DataRow("0123456789")] // numeric

        [DataRow("KjAb8364")] // mixed
        public void LowellReference_WhenValidateViaAttribute_WithValidEntry_ContainsAlphanumeric_Then_ReturnsNoError(
            string testLowellRef)
        {
            _model.LowellReference = testLowellRef;

            {
                ValidationResult result = _validator.Validate(_model);
                Assert.AreEqual(true, result.IsValid);
                Assert.AreEqual(0, result.Errors.Count);
            }
        }

        [TestMethod]
        [DataRow(" ")] // Spaces not allowed
        [DataRow("  ")]
        [DataRow("    ")]

        [DataRow("!")] // Special characters (examples)
        [DataRow("#")]
        [DataRow("<")]
        [DataRow(">")]

        [DataRow("h[")] // Mixed
        [DataRow("]J")]
        [DataRow("A$B")]
        [DataRow("!1<")]
        public void LowellReference_WhenValidateViaAttribute_WithInvalidEntry_ContainsNonAlphanumeric_Then_ReturnsError(
            string testLowellRef)
        {
            _model.LowellReference = testLowellRef;

            {
                ValidationResult result = _validator.Validate(_model);
                Assert.AreEqual(false, result.IsValid);
                {
                    var errors = result.Errors;
                    Assert.AreEqual(1, errors.Count);
                    Assert.AreEqual("Invalid Lowell reference", errors[0].ErrorMessage);
                }
            }
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        public void LowellReference_WhenValidateViaAttribute_WithMissing_Then_ReturnsError(
            string testLowellRef)
        {
            _model.LowellReference = testLowellRef;

            {
                ValidationResult result = _validator.Validate(_model);
                Assert.AreEqual(false, result.IsValid);
                {
                    var errors = result.Errors;
                    Assert.AreEqual(1, errors.Count);
                    Assert.AreEqual("Lowell reference required", errors[0].ErrorMessage);
                }
            }
        }

        [DataRow("CM2/0584")]
        [TestMethod]
        public void LowellReference_LowellReferenceWithForwardSlashValid(string lowell_reference)
        {
            _model.LowellReference = lowell_reference;

            ValidationResult result = _validator.Validate(_model);
            Assert.IsTrue(result.IsValid);
            Assert.AreEqual(0, result.Errors.Count);
        }

        [DataRow("CM2-0584")]
        [TestMethod]
        public void LowellReference_LowellReferenceWithHyphenValid(string lowell_reference)
        {
            _model.LowellReference = lowell_reference;

            ValidationResult result = _validator.Validate(_model);
            Assert.IsTrue(result.IsValid);
            Assert.AreEqual(0, result.Errors.Count);
        }
    }
}
