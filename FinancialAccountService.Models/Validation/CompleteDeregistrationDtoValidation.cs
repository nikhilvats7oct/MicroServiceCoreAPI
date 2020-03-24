using FinancialAccountService.Models.DataTransferObjects;
using FluentValidation;

namespace FinancialAccountService.Models.Validation
{
    public class CompleteDeregistrationDtoValidation : AbstractValidator<CompleteDeregistrationDto>
    {
        public CompleteDeregistrationDtoValidation()
        {
            RuleFor(x => x.UserId)
                .NotEmpty();
        }
    }
}
