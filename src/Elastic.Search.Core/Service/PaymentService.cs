using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Elastic.Search.Core.Extensions;
using Elastic.Search.Core.Infrastructure.Abstract;
using Elastic.Search.Core.Models;
using Elastic.Search.Core.Service.Abstract;
using Nest;

namespace Elastic.Search.Core.Service
{
    /// <summary>
    /// ElasticPaymentModel service
    /// </summary>
    public class PaymentService : IPaymentService
    {
        private readonly IElasticClient _elastic;
        private readonly IIndexConfigProvider _indexConfigProvider;
        private readonly string _indexName;
        private readonly IHashingService _hashingService;
        private readonly ISecuritySettingsService _securitySettingsService;

        /// <summary>
        /// Initializes a new instance of the <see cref="PaymentService"/> class.
        /// </summary>
        public PaymentService(IElasticClient elastic,
            IIndexConfigProvider indexConfigProvider,
            IHashingService hashingService,
            ISecuritySettingsService securitySettingsService)
        {
            _elastic = elastic;
            _indexConfigProvider = indexConfigProvider;
            _indexName = indexConfigProvider.GetIndexName();
            _hashingService = hashingService;
            _securitySettingsService = securitySettingsService;
        }

        /// <summary>
        /// Bulk payments insert
        /// </summary>
        public async Task<IBulkResponse> BulkInsert(IEnumerable<ElasticPaymentModel> payments)
        {
            if (!await _indexConfigProvider.IndexExists())
            {
                await _indexConfigProvider.CreateIndex<ElasticPaymentModel>();
            }

            var request = new BulkDescriptor();

            payments = DoHash(ref payments);

            foreach (var entity in payments)
            {
                request.Index<ElasticPaymentModel>(op => op
                    .Index(_indexName)
                    .Document(entity)
                );
            }

            return await _elastic.BulkAsync(request);
        }

        /// <summary>
        /// Get entity by ID.
        /// </summary>
        public async Task<ElasticPaymentModel> GetById(string id)
        {
            var response = await _elastic.GetAsync(new DocumentPath<ElasticPaymentModel>(id), g => g
                .Index(_indexName)
                .Type(typeof(ElasticPaymentModel))
            );

            if (response.Source != null)
            {
                response.Source.Confirmation = id;
            }

            return response.Source;
        }

        /// <summary>
        /// Update elasticPaymentModel.
        /// </summary>
        public Task<IUpdateResponse<ElasticPaymentModel>> Update(ElasticPaymentModel elasticPaymentModel)
        {
            var response = _elastic.UpdateAsync<ElasticPaymentModel>(elasticPaymentModel.Confirmation, descriptor => descriptor
                .Index(_indexName)
                .Doc(elasticPaymentModel)
            );

            return response;
        }

        /// <summary>
        /// Gets total count of payments
        /// </summary>
        public async Task<long> Count()
        {
            var response = await _elastic.CountAsync<ElasticPaymentModel>(new CountRequest(_indexName));

            return response.Count;
        }

        /// <summary>
        /// Delete entity by ID
        /// </summary>
        public async Task<IDeleteResponse> Delete(string id)
        {
            var response = await _elastic.DeleteAsync(new DocumentPath<ElasticPaymentModel>(id), g => g
                .Index(_indexName)
                .Type(typeof(ElasticPaymentModel))
            );
            return response;
        }

        private IEnumerable<ElasticPaymentModel> DoHash(ref IEnumerable<ElasticPaymentModel> payments)
        {
            var settingsCollection = _securitySettingsService.GetSettings();

            return payments
                .Select(s => Hash(s, settingsCollection))
                .ToList();
        }

        private T Hash<T>(T payment, FiledSettingsCollection settingsCollection)
        {
            foreach (var propertyInfo in typeof(T).GetProperties())
            {
                if (!settingsCollection.IsCrypted(propertyInfo.Name))
                {
                    continue;
                }

                var value = propertyInfo.GetValue(payment);
                if (value != null)
                {
                    if (propertyInfo.PropertyType == typeof(string))
                    {
                        var str = (string)value;

                        if (string.IsNullOrWhiteSpace(str))
                        {
                            continue;
                        }

                        var valueHash = _hashingService.Encrypt(str);
                        propertyInfo.SetValue(payment, valueHash);
                    }
                }
            }

            return payment;
        }
    }
}