namespace FinancialAccountService.Models.DataTransferObjects
{
    public class CreatedUserDto : ResultDto
    {
        public string LowellReference { get; set; }
        public string EmailAddress { get; set; }
        public int Company { get; set; }
    }
}
