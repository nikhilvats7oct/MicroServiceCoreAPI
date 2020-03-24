using FinancialAccountService.Models.Validation;
using FluentValidation.Attributes;

namespace FinancialAccountService.Models.DataTransferObjects
{
    [Validator(typeof(MyAccountsDtoValidator))]
    public class MyAccountsDto
    {
        public string UserId { get; set; }
    }
}
