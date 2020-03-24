using System;
using System.Collections.Generic;
using System.Text;

namespace FinancialAccountService.Models.DataTransferObjects
{
    public class WebActivityDto
    {
        public string LowellReference { get; set; }
        public int Company { get; set; }
        public DateTime DateTime { get; set; }
        public int WebActionID { get; set; }
        public string Guid { get; set; }
    }
}
