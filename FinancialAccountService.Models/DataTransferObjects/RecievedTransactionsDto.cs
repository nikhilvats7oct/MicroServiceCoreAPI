using System.Collections.Generic;

namespace FinancialAccountService.Models.DataTransferObjects
{
    public class RecievedTransactionsDto
    {
        public List<PaymentDetails> Payments { get; set; }

        public RecievedTransactionsDto()
        {
            Payments = new List<PaymentDetails>();
        }
    }
}
