using FinancialAccountService.Models.Validation;
using FluentValidation.Attributes;

namespace FinancialAccountService.Models.DataTransferObjects
{
    [Validator(typeof(ForgotPasswordDtoValidator))]
    public class ForgotPasswordDto : ReplayableDto
    {
        public string CallBackUrl { get; set; }
        public string EmailAddress { get; set; }
        public string UserId { get; set; }
        public string EmailName { get; set; }
        public string LowellRef { get; set; }
    }
}