using System.Collections;
using System.Collections.Generic;

namespace Melior.InterviewQuestion.Types
{
    public class Account
    {
        public string AccountNumber { get; set; }
        public decimal Balance { get; set; }
        public AccountStatus Status { get; set; }
        public ICollection<PaymentScheme> AllowedPaymentSchemes { get; set; }
    }
}

