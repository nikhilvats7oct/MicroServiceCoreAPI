using FinancialAccountService.Models.DataTransferObjects;
using FluentValidation;

namespace FinancialAccountService.Models.Validation
{
    public class RegistrationEmailDtoValidator : AbstractValidator<RegistrationEmailDto>
    {
        public RegistrationEmailDtoValidator()
        {
            RuleFor(x => x.EmailAddress)
                .NotEmpty().WithMessage(ValidationMessages.InvalidEmailAddress)
                .EmailAddress().WithMessage(ValidationMessages.InvalidEmailAddress)
                .Matches(@"^((?!lowell).)*$").WithMessage(ValidationMessages.InvalidEmailAddress);

            RuleFor(x => x.CallBackUrl)
                .NotEmpty();
        }
    }
}