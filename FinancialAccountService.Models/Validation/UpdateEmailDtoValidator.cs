using FinancialAccountService.Models.DataTransferObjects;
using FluentValidation;

namespace FinancialAccountService.Models.Validation
{
    public class UpdateEmailDtoValidator : AbstractValidator<UpdateEmailDto>
    {
        public UpdateEmailDtoValidator()
        {
            RuleFor(x => x.EmailAddress).Cascade(CascadeMode.StopOnFirstFailure)
                .NotEmpty().WithMessage(ValidationMessages.InvalidEmailAddress)
                .EmailAddress().WithMessage(ValidationMessages.InvalidEmailAddress);

            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("Invalid user ID");
        }
    }
}
