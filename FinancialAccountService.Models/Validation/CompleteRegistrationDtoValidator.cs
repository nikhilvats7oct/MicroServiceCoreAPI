using FinancialAccountService.Models.DataTransferObjects;
using FluentValidation;

namespace FinancialAccountService.Models.Validation
{
    public class CompleteRegistrationDtoValidator : AbstractValidator<CompleteRegistrationDto>
    {
        public CompleteRegistrationDtoValidator()
        {
            RuleFor(x => x.EmailAddress)
                .NotEmpty().WithMessage(ValidationMessages.InvalidEmailAddress)
                .EmailAddress().WithMessage(ValidationMessages.InvalidEmailAddress)
                .Matches(@"^((?!lowell).)*$").WithMessage(ValidationMessages.InvalidEmailAddress);

            RuleFor(x => x.LowellReference)
                .NotEmpty();

            RuleFor(x => x.UserId)
                .NotEmpty();
        }
    }
}
