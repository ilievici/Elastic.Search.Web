using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Elastic.Search.Core.Infrastructure.Abstract;
using Elastic.Search.Core.Models;
using Elastic.Search.Core.Service.Abstract;
using Microsoft.Extensions.Logging;
using Nest;

namespace Elastic.Search.Core.Service
{
    /// <summary>
    /// Payments search service
    /// </summary>
    public class PaymentSearchService : IPaymentSearchService
    {
        private readonly IElasticClient _elastic;
        private readonly string _indexName;
        private readonly ILogger<PaymentSearchService> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="PaymentSearchService"/> class.
        /// </summary>
        public PaymentSearchService(IElasticClient elastic, IIndexConfigProvider indexConfigProvider, ILoggerFactory loggerFactory)
        {
            _elastic = elastic;
            _logger = loggerFactory.CreateLogger<PaymentSearchService>();
            _indexName = indexConfigProvider.GetIndexName();
        }

        /// <summary>
        /// Search payments by <see cref="AdvancedSearchCriteria"/> criterias.
        /// </summary>
        /// <param name="filter">The <see cref="AdvancedSearchCriteria"/>.</param>
        public async Task<SearchResult<Payment>> Search(AdvancedSearchCriteria filter)
        {
            var searchRequest = BuildSearchRequest(filter);

            var searchResults = await _elastic.SearchAsync<Payment>(searchRequest);

            var data = searchResults.Hits
                 .Select(ConvertHitToCustumer)
                 .AsEnumerable();

            return new SearchResult<Payment>
            {
                Results = data,
                Total = searchResults.Total,
                Page = filter.Page,
                TotalOnPage = filter.Take,
                ElapsedMilliseconds = searchResults.Took
            };
        }

        /// <summary>
        /// Anonymous method to translate from a Hit to <see cref="Payment"/> 
        /// </summary>
        private Payment ConvertHitToCustumer(IHit<Payment> hit)
        {
            Func<IHit<Payment>, Payment> func = x =>
            {
                hit.Source.Id = Guid.Parse(hit.Id);
                return hit.Source;
            };

            return func.Invoke(hit);
        }

        /// <summary>
        /// Builds the <see cref="SearchRequest{Payment}"/>.
        /// </summary>
        private SearchRequest<Payment> BuildSearchRequest(AdvancedSearchCriteria filter)
        {
            var searchRequest = new SearchRequest<Payment>(Indices.Parse(_indexName))
            {
                From = filter.Page,
                Size = filter.Take
            };

            var sort = GetSortMode(filter);
            if (sort != null && sort.Any())
            {
                searchRequest.Sort = sort;
            }

            var container = new QueryContainer();

#pragma warning disable S125 // Sections of code should not be "commented out"
            //container &= new QueryContainerDescriptor<Payment>()
            //    .QueryString(qs => qs
            //        .Fields(f => f.Field(payment => payment.IsAudit))
            //        .Query(filter.Exclude.ToString())
            //    );
#pragma warning restore S125 // Sections of code should not be "commented out"

            //NOTE: EXACT MATCH REQUIRED
            if (!string.IsNullOrWhiteSpace(filter.CreditAccount))
            {
                container &= new QueryContainerDescriptor<Payment>()
                    .QueryString(qs => qs
                        .Fields(f => f.Field(payment => payment.CreditAccount))
                        .Query(filter.CreditAccount)
                    );
            }

            //TODO: NEED TO APPLY "LIKE"
            if (!string.IsNullOrWhiteSpace(filter.FirstName))
            {
                container &= new QueryContainerDescriptor<Payment>()
                    .QueryString(qs => qs
                        .Fields(f => f.Field(payment => payment.FirstName))
                        .Query(filter.FirstName)
                    );
            }

            //TODO: NEED TO APPLY "LIKE"
            if (!string.IsNullOrWhiteSpace(filter.LastName))
            {
                container &= new QueryContainerDescriptor<Payment>()
                    .QueryString(qs => qs
                        .Fields(f => f.Field(payment => payment.LastName))
                        .Query(filter.LastName)
                    );
            }

            //NOTE: EXACT MATCH REQUIRED
            if (!string.IsNullOrWhiteSpace(filter.Confirmation))
            {
                container &= new QueryContainerDescriptor<Payment>()
                    .QueryString(qs => qs
                        .Fields(f => f.Field(payment => payment.Confirmation))
                        .Query(filter.Confirmation)
                    );
            }

            if (!string.IsNullOrWhiteSpace(filter.DebitAccount))
            {
                container &= new QueryContainerDescriptor<Payment>()
                    .QueryString(qs => qs
                        .Fields(f => f.Field(payment => payment.DebitAccount))
                        .Query(filter.DebitAccount)
                    );
            }

            if (!string.IsNullOrWhiteSpace(filter.DebitAccountMask))
            {
                container &= new QueryContainerDescriptor<Payment>()
                    .QueryString(qs => qs
                        .Fields(f => f.Field(payment => payment.DebitAccountMask))
                        .Query(filter.DebitAccountMask)
                    );
            }

            if (!string.IsNullOrWhiteSpace(filter.Email))
            {
                container &= new QueryContainerDescriptor<Payment>()
                    .QueryString(qs => qs
                        .Fields(f => f.Field(payment => payment.Emails))
                        .Query(filter.Email)
                    );
            }

            if (!string.IsNullOrWhiteSpace(filter.Phone))
            {
                container &= new QueryContainerDescriptor<Payment>()
                    .QueryString(qs => qs
                        .Fields(f => f.Field(payment => payment.Phones))
                        .Query(filter.Phone)
                    );
            }

            if (!string.IsNullOrWhiteSpace(filter.Color))
            {
                container &= new QueryContainerDescriptor<Payment>()
                    .QueryString(qs => qs
                        .Fields(f => f.Field(payment => payment.Color))
                        .Query(filter.Color)
                    );
            }

            if (!string.IsNullOrWhiteSpace(filter.CardIssuer))
            {
                container &= new QueryContainerDescriptor<Payment>()
                    .QueryString(qs => qs
                        .Fields(f => f.Field(payment => payment.CardIssuer))
                        .Query(filter.CardIssuer)
                    );
            }

            if (!string.IsNullOrWhiteSpace(filter.PaymentType))
            {
                container &= new QueryContainerDescriptor<Payment>()
                    .QueryString(qs => qs
                        .Fields(f => f.Field(payment => payment.PaymentType))
                        .Query(filter.PaymentType)
                    );
            }

            if (!string.IsNullOrWhiteSpace(filter.FirstName))
            {
                container &= new QueryContainerDescriptor<Payment>()
                    .QueryString(qs => qs
                        .Fields(f => f.Field(payment => payment.FirstName))
                        .Query(filter.FirstName)
                    );
            }

            if (Math.Abs(filter.PaymentAmountFrom) > 0 || Math.Abs(filter.PaymentAmountTo) > 0)
            {
                container &= new QueryContainerDescriptor<Payment>()
                    .Range(qs => qs.Field(f => f.Amount)
                        .GreaterThanOrEquals(filter.PaymentAmountFrom)
                        .LessThan(filter.PaymentAmountTo)
                    );
            }

            #region Apply date filter

            Expression<Func<Payment, object>> objectPath = null;

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

            searchRequest.Query = container;
            return searchRequest;
        }

        /// <summary>
        /// Gets sort mode based on <see cref="AdvancedSearchCriteria"/>
        /// </summary>
        private IList<ISort> GetSortMode(AdvancedSearchCriteria filter)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(filter.SortingField) || string.IsNullOrWhiteSpace(filter.SortingDirection))
                {
                    return new List<ISort>();
                }

                var propertyInfo = GetPropertyInfo(filter.SortingField);
                if (propertyInfo == null)
                {
                    _logger.LogWarning($"Tried to sort by undefined property \"{filter.SortingField}\".");
                    return new List<ISort>();
                }

                var descriptor = new SortFieldDescriptor<Payment>();

                if (filter.SortingDirection.Equals("ASC", StringComparison.InvariantCultureIgnoreCase))
                {
                    descriptor.Ascending();
                }

                if (filter.SortingDirection.Equals("DESC", StringComparison.InvariantCultureIgnoreCase))
                {
                    descriptor.Descending();
                }

                var parameter = Expression.Parameter(typeof(Payment), "x");
                var sortExpression = Expression.Lambda<Func<Payment, object>>(
                    Expression.Convert(Expression.Property(parameter, propertyInfo), typeof(object)), parameter);

                if (propertyInfo.PropertyType == typeof(string))
                {
                    //https://www.elastic.co/guide/en/elasticsearch/client/net-api/current/field-inference.html
                    var expression = sortExpression.AppendSuffix("keyword");
                    descriptor.Field(expression);
                }
                else
                if (propertyInfo.PropertyType.IsGenericType)
                {
                    //TODO: this is not done
                    _logger.LogWarning("Sort by generig type is not done...");
                    return new List<ISort>();
                }
                else
                {
                    descriptor.Field(sortExpression);
                }

                return new List<ISort> { descriptor };
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Exception on building SortFieldDescriptor<T>",
                    new
                    {
                        filter.SortingField,
                        filter.SortingDirection
                    }
                );

                return new List<ISort>();
            }
        }

        private PropertyInfo GetPropertyInfo(string propertyName)
        {
            if (string.IsNullOrWhiteSpace(propertyName))
            {
                return null;
            }

            foreach (var property in typeof(Payment).GetProperties())
            {
                if (!property.Name.Equals(propertyName, StringComparison.InvariantCultureIgnoreCase))
                {
                    continue;
                }

                return property;
            }

            return null;
        }

        /// <summary>
        /// Apply generic date time query on <see cref="QueryContainer"/>.
        /// </summary>
        private void ApplyDateFilter(ref QueryContainer container, Expression<Func<Payment, object>> objectPath, DateTime? startDate, DateTime? endDate)
        {
            if (startDate.HasValue && !endDate.HasValue)
            {
                container &= new QueryContainerDescriptor<Payment>()
                    .DateRange(qs => qs.Field(objectPath)
                        .GreaterThanOrEquals(startDate)
                    );
            }

            if (!startDate.HasValue && endDate.HasValue)
            {
                container &= new QueryContainerDescriptor<Payment>()
                    .DateRange(qs => qs.Field(f => f.ScheduledDate)
                        .LessThanOrEquals(endDate)
                    );
            }

            if (startDate.HasValue && endDate.HasValue)
            {
                container &= new QueryContainerDescriptor<Payment>()
                    .DateRange(qs => qs.Field(f => f.ScheduledDate)
                        .GreaterThanOrEquals(startDate)
                        .LessThanOrEquals(endDate)
                    );
            }
        }
    }
}