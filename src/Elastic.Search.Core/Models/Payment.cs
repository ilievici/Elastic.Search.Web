using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Nest;

namespace Elastic.Search.Core.Models
{
    [ElasticsearchType(Name = "payments", IdProperty = nameof(Id))]
    public class Payment
    {
        public Payment()
        {
            ElasticId = Guid.NewGuid();
        }
        
        [Key]
        [Keyword]
        public long Confirmation { get; set; }

        [Ignore]
        public Guid ElasticId { get; set; }

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

        [Keyword]
        public string FirstName { get; set; }

        [Keyword]
        public string LastName { get; set; }

        [Keyword]
        public string DebitAccountMask { get; set; }

        [Keyword]
        public string CreditAccount { get; set; }

        [Keyword]
        public string DebitAccount { get; set; }

        public string Email { get; set; }

        public List<string> Phones
        {
            get
            {
                var list = new List<string>();
                if (string.IsNullOrWhiteSpace(PhoneOne))
                {
                    list.Add(PhoneOne);
                }
                if (string.IsNullOrWhiteSpace(PhoneTwo))
                {
                    list.Add(PhoneTwo);
                }

                return list;
            }
            set
            {
                if (value == null)
                {
                    PhoneOne = null;
                    PhoneTwo = null;
                    return;
                }

                if (value.Count == 1)
                {
                    PhoneOne = value[0];
                    PhoneTwo = null;
                    return;
                }

                if (value.Count >= 2)
                {
                    PhoneOne = value[0];
                    PhoneTwo = value[1];
                }
            }
        }
        
        public string PhoneOne { get; set; }

        public string PhoneTwo { get; set; }
        
        [Date]
        public DateTime? EnteredDate { get; set; }

        [Date]
        public DateTime? ActuatedDate { get; set; }

        [Date]
        public DateTime ScheduledDate { get; set; }

        [Boolean]
        public bool HasAuditDetails { get; set; }
    }
}