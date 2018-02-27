using System;
using Elastic.Search.Core.Extensions;
using Nest;

namespace Elastic.Search.Core.Models
{
    [ElasticsearchType(Name = "payments", IdProperty = nameof(Confirmation))]
    public class ElasticPaymentModel
    {
        [Keyword]
        public int Confirmation { get; set; }

        [Ignore]
        public PaymentDateSearchType DateType { get; set; }

        [Text]
        public string PaymentType { get; set; }

        [Number]
        public decimal Amount { get; set; }

        [Number]
        public decimal FeeAmount { get; set; }

        [Text]
        public string Status { get; set; }

        [Text]
        public string Channel { get; set; }

        [Keyword]
        public string CardIssuer { get; set; }

        [Keyword]
        public string Color { get; set; }

        [Text]
        public string FirstName { get; set; }

        [Text]
        public string LastName { get; set; }

        [Keyword]
        public string DebitAccountMask { get; set; }

        [Keyword]
        public string CreditAccount { get; set; }

        [Keyword]
        public string DebitAccount { get; set; }
        
        [Text]
        public string Email { get; set; }
        
        [Keyword]
        public string PhoneOne { get; set; }
        
        [Keyword]
        public string PhoneTwo { get; set; }
        
        [Date]
        public DateTime? EnteredDate { get; set; }

        [Date]
        public DateTime? ActuatedDate { get; set; }

        [Date]
        public DateTime ScheduledDate { get; set; }

        [Boolean]
        public bool HasAuditDetails { get; set; }

        [Keyword]
        public string CollectorId { get; set; }

        [Keyword]
        public string CreditSiteId { get; set; }
        
        [Text(Index = false)]
        public string HashValue => this.GetMd5Hash();
    }
}