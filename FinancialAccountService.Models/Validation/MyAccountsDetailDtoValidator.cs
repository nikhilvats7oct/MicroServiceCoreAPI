using FinancialAccountService.Models.DataTransferObjects;
using FluentValidation;

namespace FinancialAccountService.Models.Validation
{
    public class MyAccountsDetailDtoValidator : AbstractValidator<MyAccountsDetailDto>
    {
        public MyAccountsDetailDtoValidator()
        {
            RuleFor(x => x.LowellReference)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .MaximumLength(10).WithMessage(ValidationMessages.InvalidLowellRef)
                .Matches(@"^[a-zA-Z0-9\/\-]*$").WithMessage(ValidationMessages.InvalidLowellRef)
                // ^ indicates match from start of string
                // $ indicates match to end of string
                // [ ] inside of this is the valid character ranges
                // * zero to infinity characters
                .NotEmpty().WithMessage(ValidationMessages.MissingLowellRef);
        }
    }
}
