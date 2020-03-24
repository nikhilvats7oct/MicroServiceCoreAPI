using System;
using System.Collections.Generic;

namespace FinancialAccountService.Models.DataTransferObjects
{
    public class MyAccountsDetailResultDto : MyAccountsSummaryResultDto
    {
        // Extends Summary information with more account detail

        public string AccountMessage { get; set; }

        public string[] PlanMessages { get; set; }
        
        public class RecentTransaction
        {
            public DateTime Date { get; set; }
            public string Description { get; set; }
            public string Type { get; set; }
            public decimal Amount { get; set; }
            public decimal RollingBalance { get; set; }
        }

        public List<RecentTransaction> RecentTransactions { get; set; }
    }
}
