using System;
using System.Linq;
using System.Threading.Tasks;
using Elastic.Search.Core.Extensions;
using Elastic.Search.Core.Infrastructure.Abstract;
using Elastic.Search.Core.Models;
using Elastic.Search.Core.Service.Abstract;
using Nest;

namespace Elastic.Search.Core.Service
{
    /// <summary>
    /// Payments search service
    /// </summary>
    public class PaymentSearchService : IPaymentSearchService
    {
        private readonly IElasticClient _elastic;
        private readonly IHashingService _hashingService;
        private readonly ISecuritySettingsService _securitySettingsService;
        private readonly string _indexName;

        /// <summary>
        /// Initializes a new instance of the <see cref="PaymentSearchService"/> class.
        /// </summary>
        public PaymentSearchService(IElasticClient elastic,
            IIndexConfigProvider indexConfigProvider,
            IHashingService hashingService,
            ISecuritySettingsService securitySettingsService)
        {
            _elastic = elastic;
            _hashingService = hashingService;
            _securitySettingsService = securitySettingsService;
            _indexName = indexConfigProvider.GetIndexName();
        }

        /// <summary>
        /// Search payments by <see cref="PaymentFilterModel"/> criterias.
        /// </summary>
        /// <param name="filter">The <see cref="PaymentFilterModel"/>.</param>
        public async Task<SearchResult<ElasticPaymentModel>> Search(PaymentFilterModel filter)
        {
            ISearchResponse<ElasticPaymentModel> searchResults;

            var settingsCollection = _securitySettingsService.GetSettings();

            if (filter.HasAnySearchConditions())
            {
                var searchRequest = BuildSearchRequest(filter, settingsCollection);
                searchResults = await _elastic.SearchAsync<ElasticPaymentModel>(searchRequest);
            }
            else
            {
                searchResults = await _elastic.SearchAsync<ElasticPaymentModel>(s => s
                    .Index(_indexName)
                    .From(filter.Page)
                    .Size(filter.Take)
                    .Query(f => f
                        .MatchAll())
                );
            }

            var data = searchResults.Hits
                .Select(s => ConvertHitToCustumer(s, filter.DateType, settingsCollection))
                .AsEnumerable();

            return new SearchResult<ElasticPaymentModel>
            {
                Results = data,
                Total = searchResults.Total,
                Page = filter.Page,
                TotalOnPage = filter.Take,
                ElapsedMilliseconds = searchResults.Took
            };
        }
        
        /// <summary>
        /// Anonymous method to translate from a Hit to <see cref="ElasticPaymentModel"/> 
        /// </summary>
        private ElasticPaymentModel ConvertHitToCustumer(IHit<ElasticPaymentModel> hit, PaymentDateSearchType dateType, FiledSettingsCollection settingsCollection)
        {
            Func<IHit<ElasticPaymentModel>, ElasticPaymentModel> func = x =>
            {
                hit.Source.ElasticId = Guid.Parse(hit.Id);
                hit.Source.DateType = dateType;

                hit.Source.CreditAccount = _hashingService.Decrypt(hit.Source.CreditAccount);
                hit.Source.DebitAccount = _hashingService.Decrypt(hit.Source.DebitAccount);

                return hit.Source;
            };

            return func.Invoke(hit);
        }

        /// <summary>
        /// Builds the <see cref="SearchRequest{ElasticPaymentModel}"/>.
        /// </summary>
        private SearchRequest<ElasticPaymentModel> BuildSearchRequest(PaymentFilterModel filter, FiledSettingsCollection settingsCollection)
        {
            var searchRequest = new SearchRequest<ElasticPaymentModel>(Indices.Parse(_indexName))
            {
                From = filter.Page,
                Size = filter.Take,

                Sort = filter.BuildSort(settingsCollection),
                Query = filter.BuildQuery(_hashingService, settingsCollection)
            };

            return searchRequest;
        }
    }
}