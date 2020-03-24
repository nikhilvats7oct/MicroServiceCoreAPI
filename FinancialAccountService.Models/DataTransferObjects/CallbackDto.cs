using System;

namespace FinancialAccountService.Models.DataTransferObjects
{
    public class CallbackDto
    {
        public string LowellReference { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime WindowStart { get; set; }
        public DateTime WindowEnd { get; set; }
        public string Note { get; set; }
        public string Requestor { get; set; }
    }
}
