using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Nest;

namespace Elastic.Search.Core.Models
{
    [ElasticsearchType(Name = "payments", IdProperty = nameof(Id))]
    public class Payment
    {
        [Ignore]
        public Guid Id { get; set; }

        [Text]
        public string PaymentType { get; set; }

        [Number]
        public decimal Amount { get; set; }

        [Text]
        public string Channel { get; set; }

        [Keyword]
        public string CardIssuer { get; set; }

        [Keyword]
        public string Color { get; set; }

        [Keyword]
        public string FirstName { get; set; }

        [Keyword]
        public string LastName { get; set; }

        [Keyword]
        public string Confirmation { get; set; }

        [Keyword]
        public string DebitAccountMask { get; set; }

        [Keyword]
        public string CreditAccount { get; set; }

        [Keyword]
        public string DebitAccount { get; set; }

        public List<string> Emails { get; set; }

        public List<string> Phones { get; set; }

        [Date]
        public DateTime EnteredDate { get; set; }

        [Date]
        public DateTime ActuatedDate { get; set; }

        [Date]
        public DateTime ScheduledDate { get; set; }

        [Boolean]
        public bool IsAudit { get; set; }
    }
}