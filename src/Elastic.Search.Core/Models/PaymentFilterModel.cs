using System;

namespace Elastic.Search.Core.Models
{
    public class PaymentFilterModel : PagingCriteria
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Confirmation { get; set; }
        
        public string CreditAccount { get; set; }
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

        public string RetreiveSelfEnteredPayments { get; set; }

        public string RetreiveSiteSpecificPayments { get; set; }
        
        public bool HasAnySearchConditions()
        {
            if (!string.IsNullOrWhiteSpace(CreditAccount))
                return true;
            if (!string.IsNullOrWhiteSpace(FirstName))
                return true;
            if (!string.IsNullOrWhiteSpace(LastName))
                return true;
            if (!string.IsNullOrWhiteSpace(Confirmation))
                return true;
            if (!string.IsNullOrWhiteSpace(DebitAccount))
                return true;
            if (!string.IsNullOrWhiteSpace(Email))
                return true;
            if (!string.IsNullOrWhiteSpace(Phone))
                return true;
            if (!string.IsNullOrWhiteSpace(PaymentType))
                return true;
            if (Math.Abs(PaymentAmountFrom) > 0 || Math.Abs(PaymentAmountTo) > 0)
                return true;
            if (PaymentStartDate.HasValue || PaymentEndDate.HasValue)
                return true;
            if (!string.IsNullOrWhiteSpace(RetreiveSelfEnteredPayments))
                return true;
            if (!string.IsNullOrWhiteSpace(RetreiveSiteSpecificPayments))
                return true;

            return false;
        }
    }

    public enum PaymentDateSearchType
    {
        ActuatedDate,
        EntryDate,
        ScheduleDate
    }
}
