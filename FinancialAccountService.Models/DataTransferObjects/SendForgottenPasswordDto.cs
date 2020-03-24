using System.Collections.Generic;

namespace FinancialAccountService.Models.DataTransferObjects
{
    public class SendForgottenPasswordDto : ReplayableDto
    {
        public string Name { get; set; }
        public string EmailAddress { get; set; }
        public List<DataItem> Data { get; set; }

        public class DataItem
        {
            public string Key { get; set; } = "PasswordResetUrl";
            public string Value { get; set; }
        }
    }
}
