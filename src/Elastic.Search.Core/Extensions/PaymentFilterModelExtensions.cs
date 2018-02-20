using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Elastic.Search.Core.Models;
using Elastic.Search.Core.Service.Abstract;
using Nest;

namespace Elastic.Search.Core.Extensions
{
    public static class PaymentFilterModelExtensions
    {
        /// <summary>
        /// Build sort based on <see cref="PaymentFilterModel"/> filter.
        /// </summary>
        public static IList<ISort> BuildSort(this PaymentFilterModel filter, FiledSettingsCollection settingsCollection)
        {
            if (string.IsNullOrWhiteSpace(filter.SortingField) || string.IsNullOrWhiteSpace(filter.SortingDirection))
            {
                return null;
            }

            var propertyInfo = GetPropertyInfo(filter.SortingField);
            if (propertyInfo == null)
            {
                return null;
            }

            if (!settingsCollection.AllowSort(propertyInfo.Name))
            {
                return null;
            }

            var descriptor = new SortFieldDescriptor<ElasticPaymentModel>();

            if (filter.SortingDirection.Equals("ASC", StringComparison.InvariantCultureIgnoreCase))
            {
                descriptor.Ascending();
            }

            if (filter.SortingDirection.Equals("DESC", StringComparison.InvariantCultureIgnoreCase))
            {
                descriptor.Descending();
            }

            var parameter = Expression.Parameter(typeof(ElasticPaymentModel), "x");
            var sortExpression = Expression.Lambda<Func<ElasticPaymentModel, object>>(
                Expression.Convert(Expression.Property(parameter, propertyInfo), typeof(object)), parameter);

            if (propertyInfo.PropertyType == typeof(string))
            {
                //https://www.elastic.co/guide/en/elasticsearch/client/net-api/current/field-inference.html
                var expression = sortExpression.AppendSuffix("keyword");
                descriptor.Field(expression);
            }
            else
            {
                descriptor.Field(sortExpression);
            }

            return new List<ISort> { descriptor };
        }

        /// <summary>
        /// Build query based on <see cref="PaymentFilterModel"/> filter.
        /// </summary>
        public static QueryContainer BuildQuery(this PaymentFilterModel filter,
            IHashingService hashingService,
            FiledSettingsCollection settingsCollection)
        {
            var container = new QueryContainer();

            if (!string.IsNullOrWhiteSpace(filter.Confirmation))
            {
                if (settingsCollection.IsExactMatch(nameof(filter.Confirmation)))
                {
                    container &= new QueryContainerDescriptor<ElasticPaymentModel>()
                        .QueryString(qs => qs
                            .Fields(f => f.Field(payment => payment.Confirmation))
                            .Query(filter.Confirmation)
                        );
                }
                else
                {
                    container &= new QueryContainerDescriptor<ElasticPaymentModel>()
                        .Wildcard(w => w
                            .Field(p => p.Confirmation.Suffix("keyword"))
                            .Value($"*{filter.Confirmation}*")
                        );
                }
            }

            if (!string.IsNullOrWhiteSpace(filter.FirstName))
            {
                if (settingsCollection.IsExactMatch(nameof(filter.FirstName)))
                {
                    container &= new QueryContainerDescriptor<ElasticPaymentModel>()
                        .QueryString(qs => qs
                            .Fields(f => f.Field(payment => payment.FirstName))
                            .Query(filter.FirstName)
                        );
                }
                else
                {
                    container &= new QueryContainerDescriptor<ElasticPaymentModel>()
                        .Wildcard(w => w
                            .Field(p => p.FirstName.Suffix("keyword"))
                            .Value($"*{filter.FirstName}*")
                        );
                }
            }

            if (!string.IsNullOrWhiteSpace(filter.LastName))
            {
                if (settingsCollection.IsExactMatch(nameof(filter.LastName)))
                {
                    container &= new QueryContainerDescriptor<ElasticPaymentModel>()
                        .Wildcard(w => w
                            .Field(p => p.LastName.Suffix("keyword"))
                            .Value($"*{filter.LastName}*")
                        );
                }
                else
                {
                    container &= new QueryContainerDescriptor<ElasticPaymentModel>()
                        .QueryString(qs => qs
                            .Fields(f => f.Field(payment => payment.LastName))
                            .Query(filter.LastName)
                        );
                }
            }

            if (!string.IsNullOrWhiteSpace(filter.CreditAccount))
            {
                var searchValue = filter.CreditAccount;

                if (settingsCollection.IsCrypted(nameof(filter.CreditAccount)))
                {
                    searchValue = hashingService.Encrypt(filter.CreditAccount);
                }

                container &= new QueryContainerDescriptor<ElasticPaymentModel>()
                    .QueryString(qs => qs
                        .Fields(f => f.Field(payment => payment.CreditAccount))
                        .Query(searchValue)
                    );
            }

            if (!string.IsNullOrWhiteSpace(filter.DebitAccount))
            {
                var searchValue = filter.DebitAccount;

                if (settingsCollection.IsCrypted(nameof(filter.DebitAccount)))
                {
                    searchValue = hashingService.Encrypt(filter.DebitAccount);
                }

                container &= new QueryContainerDescriptor<ElasticPaymentModel>()
                    .QueryString(qs => qs
                        .Fields(f => f.Field(payment => payment.DebitAccount))
                        .Query(searchValue)
                    );
            }


            if (!string.IsNullOrWhiteSpace(filter.Email))
            {
                if (settingsCollection.IsExactMatch(nameof(filter.Email)))
                {
                    container &= new QueryContainerDescriptor<ElasticPaymentModel>()
                        .Wildcard(w => w
                            .Field(p => p.Email.Suffix("keyword"))
                            .Value($"*{filter.Email}*")
                        );
                }
                else
                {
                    container &= new QueryContainerDescriptor<ElasticPaymentModel>()
                        .QueryString(qs => qs
                            .Fields(f => f.Field(payment => payment.Email))
                            .Query(filter.Email)
                        );
                }
            }

            if (!string.IsNullOrWhiteSpace(filter.Phone))
            {
                if (settingsCollection.IsExactMatch(nameof(filter.Phone)))
                {
                    container &=
                        new QueryContainerDescriptor<ElasticPaymentModel>()
                            .Wildcard(mm => mm
                                .Field(payment => payment.PhoneOne)
                                .Value($"*{filter.Phone}*")
                            ) ||
                        new QueryContainerDescriptor<ElasticPaymentModel>()
                            .Wildcard(mm => mm
                                .Field(payment => payment.PhoneTwo)
                                .Value($"*{filter.Phone}*")
                            );
                }
                else
                {
                    container &= new QueryContainerDescriptor<ElasticPaymentModel>()
                        .MultiMatch(mm => mm
                            .Fields(f => f
                                .Field(payment => payment.PhoneOne)
                                .Field(payment => payment.PhoneTwo)
                            )
                            .Query(filter.Phone)
                        );
                }
            }

            if (!string.IsNullOrWhiteSpace(filter.RetreiveSelfEnteredPayments))
            {
                container &= new QueryContainerDescriptor<ElasticPaymentModel>()
                    .QueryString(qs => qs
                        .Fields(f => f.Field(payment => payment.CollectorId))
                        .Query(filter.RetreiveSelfEnteredPayments)
                    );
            }

            if (!string.IsNullOrWhiteSpace(filter.RetreiveSiteSpecificPayments))
            {
                container &= new QueryContainerDescriptor<ElasticPaymentModel>()
                    .QueryString(qs => qs
                        .Fields(f => f.Field(payment => payment.CreditSiteId))
                        .Query(filter.RetreiveSiteSpecificPayments)
                    );
            }

            #region Apply amount filter

            if (Math.Abs(filter.PaymentAmountFrom) > 0 && Math.Abs(filter.PaymentAmountTo) < 0)
            {
                container &= new QueryContainerDescriptor<ElasticPaymentModel>()
                    .Range(qs => qs.Field(f => f.Amount)
                        .GreaterThanOrEquals((double)filter.PaymentAmountFrom)
                    );
            }

            if (Math.Abs(filter.PaymentAmountFrom) < 0 && Math.Abs(filter.PaymentAmountTo) > 0)
            {
                container &= new QueryContainerDescriptor<ElasticPaymentModel>()
                    .Range(qs => qs.Field(f => f.Amount)
                        .LessThanOrEquals((double)filter.PaymentAmountTo)
                    );
            }

            if (Math.Abs(filter.PaymentAmountFrom) > 0 && Math.Abs(filter.PaymentAmountTo) > 0)
            {
                container &= new QueryContainerDescriptor<ElasticPaymentModel>()
                    .Range(qs => qs.Field(f => f.Amount)
                        .GreaterThanOrEquals((double)filter.PaymentAmountFrom)
                        .LessThan((double)filter.PaymentAmountTo)
                    );
            }

            #endregion

            #region Apply date filter

            Expression<Func<ElasticPaymentModel, object>> objectPath = null;

            switch (filter.DateType)
            {
                case PaymentDateSearchType.ActuatedDate:
                    objectPath = payment => payment.ActuatedDate;
                    break;
                case PaymentDateSearchType.EntryDate:
                    objectPath = payment => payment.EnteredDate;
                    break;
                case PaymentDateSearchType.ScheduleDate:
                    objectPath = payment => payment.ScheduledDate;
                    break;
            }

            if (objectPath != null)
            {
                ApplyDateFilter(ref container, objectPath, filter.PaymentStartDate, filter.PaymentEndDate);
            }

            #endregion

            return container;
        }

        /// <summary>
        /// Apply generic date time query on <see cref="QueryContainer"/>.
        /// </summary>
        private static void ApplyDateFilter(ref QueryContainer container,
            Expression<Func<ElasticPaymentModel, object>> objectPath,
            DateTime? startDate,
            DateTime? endDate)
        {
            if (startDate.HasValue && !endDate.HasValue)
            {
                container &= new QueryContainerDescriptor<ElasticPaymentModel>()
                    .DateRange(qs => qs.Field(objectPath)
                        .GreaterThanOrEquals(startDate)
                    );
            }

            if (!startDate.HasValue && endDate.HasValue)
            {
                container &= new QueryContainerDescriptor<ElasticPaymentModel>()
                    .DateRange(qs => qs.Field(f => f.ScheduledDate)
                        .LessThanOrEquals(endDate)
                    );
            }

            if (startDate.HasValue && endDate.HasValue)
            {
                container &= new QueryContainerDescriptor<ElasticPaymentModel>()
                    .DateRange(qs => qs.Field(f => f.ScheduledDate)
                        .GreaterThanOrEquals(startDate)
                        .LessThanOrEquals(endDate)
                    );
            }
        }

        private static PropertyInfo GetPropertyInfo(string propertyName)
        {
            if (string.IsNullOrWhiteSpace(propertyName))
            {
                return null;
            }

            foreach (var property in typeof(ElasticPaymentModel).GetProperties())
            {
                if (!property.Name.Equals(propertyName, StringComparison.InvariantCultureIgnoreCase))
                {
                    continue;
                }

                return property;
            }

            return null;
        }
    }
}
