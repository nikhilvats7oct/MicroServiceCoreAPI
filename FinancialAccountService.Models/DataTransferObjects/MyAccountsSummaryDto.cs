using FinancialAccountService.Models.Validation;
using FluentValidation.Attributes;

namespace FinancialAccountService.Models.DataTransferObjects
{ 
    [Validator(typeof(MyAccountsSummaryDtoValidator))]
    public class MyAccountsSummaryDto
    {
        public string AccountId { get; set; }
    }
}
