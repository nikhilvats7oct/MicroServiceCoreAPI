using FinancialAccountService.Models.Validation;
using FluentValidation.Attributes;

namespace FinancialAccountService.Models.DataTransferObjects
{
    [Validator(typeof(RegistrationEmailDtoValidator))]
    public class RegistrationEmailDto : ReplayableDto
    {
        public string LowellReference { get; set; }
        public string EmailAddress { get; set; }
        public string CallBackUrl { get; set; }
        public string EmailName { get; set; }
    }
}