using FinancialAccountService.Models.DataTransferObjects;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Validation.Unit.Tests._Utility;

namespace Validation.Unit.Tests
{  
    [TestClass]
    public class MyAccountsSummaryDtoValidatorTests
    {
        private IValidator<MyAccountsSummaryDto> _validator;

        [TestInitialize]
        public void Initialise()
        {
            _validator = ModelValidatorInstantiator.GetValidatorFromModelAttribute<MyAccountsSummaryDto>();
        }

        [TestMethod]
        [DataRow("1")]
        [DataRow("1234567890")]        
        public void AccountId_WhenValidateViaAttribute_WithValidId_Then_ReturnsNoError(string testAccountId)
        {
            MyAccountsSummaryDto model = new MyAccountsSummaryDto();

            model.AccountId = testAccountId;

            {
                ValidationResult result = _validator.Validate(model);
                Assert.AreEqual(true, result.IsValid);
                Assert.AreEqual(0, result.Errors.Count);
            }
        }

        [TestMethod]       
        [DataRow("")]       
        public void AccountId_WhenValidateViaAttribute_WithEmpatyId_Then_ReturnsError(string testAccountId)
        {
            MyAccountsSummaryDto model = new MyAccountsSummaryDto();

            model.AccountId = testAccountId;

            {
                ValidationResult result = _validator.Validate(model);
                Assert.AreEqual(false, result.IsValid);
                Assert.AreEqual(1, result.Errors.Count);
                Assert.AreEqual("AccountId", result.Errors[0].PropertyName);
                Assert.AreEqual("Must not be empty", result.Errors[0].ErrorMessage);
            }
        }
    }
}
