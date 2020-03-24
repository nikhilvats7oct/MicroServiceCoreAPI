using System.Collections.Generic;

namespace FinancialAccountService.Models.DataTransferObjects
{
    public class SendEmailDto : ReplayableDto
    {
        public string Name { get; set; }
        public string EmailAddress { get; set; }
        public List<DataItem> Data { get; set; }

        public class DataItem
        {
            public string Key { get; set; } = "key";
            public string Value { get; set; }
        }
    }
}
