using FluentValidation.Attributes;

namespace FinancialAccountService.Models.DataTransferObjects
{

    [Validator(typeof(CompleteDeregistrationDto))]
    public class CompleteDeregistrationDto : ReplayableDto
    {
        public string UserId { get; set; }
    }
}

