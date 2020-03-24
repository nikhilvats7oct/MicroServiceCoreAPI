using System;
using FinancialAccountService.Models.DataTransferObjects;
using FluentValidation;
namespace FinancialAccountService.Models.Validation
{   
    public class MyAccountsSummaryDtoValidator : AbstractValidator<MyAccountsSummaryDto>
    {
        public MyAccountsSummaryDtoValidator()
        {
            RuleFor(x => x.AccountId)
                .NotEmpty()
                .WithMessage("Must not be empty");
        }        
    }
}
