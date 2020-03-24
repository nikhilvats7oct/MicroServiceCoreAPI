using System;

namespace FinancialAccountService.Models.DataTransferObjects
{
    public class RegisterValidationDto
    {
        public string LowellReference { get; set; }
        public string Postcode { get; set; }
        public DateTime DateOfBirth { get; set; }
    }
}