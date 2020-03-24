using FluentValidation.Attributes;

namespace FinancialAccountService.Models.DataTransferObjects
{
    [Validator(typeof(CompleteRegistrationDto))]
    public class CompleteRegistrationDto : ReplayableDto
    {
        public string LowellReference { get; set; }
        public string EmailAddress { get; set; }
        public string UserId { get; set; }
        public int Company { get; set; }
    }
}
