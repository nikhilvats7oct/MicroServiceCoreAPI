using FinancialAccountService.Models.Validation;
using FluentValidation.Attributes;

namespace FinancialAccountService.Models.DataTransferObjects
{
    [Validator(typeof(UpdateEmailDtoValidator))]
    public class UpdateEmailDto : ReplayableDto
    {
        public string UserId { get; set; }
        public string EmailAddress { get; set; }
    }
}
