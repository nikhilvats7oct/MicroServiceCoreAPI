using System;
using FinancialAccountService.Models.DataTransferObjects;
using FluentValidation;

namespace FinancialAccountService.Models.Validation
{
    public class MyAccountsDtoValidator : AbstractValidator<MyAccountsDto>
    {
        public MyAccountsDtoValidator()
        {
            RuleFor(x => x.UserId)
                .Must(BeGuid)
                .WithMessage("Must be a GUID.");
        }

        bool BeGuid(string s)
        {
            return Guid.TryParse(s, out var guidUnused);
        }
    }
}
