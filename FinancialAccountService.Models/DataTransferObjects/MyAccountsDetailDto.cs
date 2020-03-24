using FinancialAccountService.Models.Validation;
using FluentValidation.Attributes;

namespace FinancialAccountService.Models.DataTransferObjects
{
    [Validator(typeof(MyAccountsDetailDtoValidator))]
    public class MyAccountsDetailDto
    {
        public string UserId { get; set; }
        public string LowellReference { get; set; }
    }
}
