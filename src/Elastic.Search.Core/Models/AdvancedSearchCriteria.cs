using System;

namespace Elastic.Search.Core.Models
{
    public class AdvancedSearchCriteria : PagingCriteria
    {
        public string CreditAccount { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Confirmation { get; set; }
        public string DebitAccountMask { get; set; }
        public string DebitAccount { get; set; }

        public string Email { get; set; }
        public string Phone { get; set; }

        public string PaymentType { get; set; }

        public double PaymentAmountFrom { get; set; }
        public double PaymentAmountTo { get; set; }

        public DateTime? PaymentStartDate { get; set; }
        public DateTime? PaymentEndDate { get; set; }

        public string Color { get; set; }

        public PaymentDateSearchType DateType { get; set; } = PaymentDateSearchType.EntryDate;

        public bool Exclude { get; set; }

        public string CardIssuer { get; set; }
    }

    public enum PaymentDateSearchType
    {
        ActuatedDate,
        EntryDate,
        ScheduleDate
    }
}
