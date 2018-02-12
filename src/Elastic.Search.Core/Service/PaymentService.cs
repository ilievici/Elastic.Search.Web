using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Elastic.Search.Core.Infrastructure.Abstract;
using Elastic.Search.Core.Models;
using Elastic.Search.Core.Service.Abstract;
using Nest;

namespace Elastic.Search.Core.Service
{
    /// <summary>
    /// Payment service
    /// </summary>
    public class PaymentService : IPaymentService
    {
        private readonly IElasticClient _elastic;
        private readonly string _indexName;

        /// <summary>
        /// Initializes a new instance of the <see cref="PaymentService"/> class.
        /// </summary>
        public PaymentService(IElasticClient elastic, IIndexConfigProvider indexConfigProvider)
        {
            _elastic = elastic;
            _indexName = indexConfigProvider.GetIndexName();
        }

        /// <summary>
        /// Bulk payments insert
        /// </summary>
        public Task<IBulkResponse> BulkInsert(IEnumerable<Payment> payments)
        {
            var request = new BulkDescriptor();

            foreach (var entity in payments)
            {
                request.Index<Payment>(op => op
                    .Id(entity.Id.ToString())
                    .Index(_indexName)
                    .Document(entity)
                );
            }

            return _elastic.BulkAsync(request);
        }

        /// <summary>
        /// Get entity by ID.
        /// </summary>
        public async Task<Payment> GetById(Guid id)
        {
            var response = await _elastic.GetAsync(new DocumentPath<Payment>(id), g => g
                .Index(_indexName)
                .Type(typeof(Payment))
            );

            if (response.Source != null)
            {
                response.Source.Id = id;
            }

            return response.Source;
        }

        /// <summary>
        /// Update payment.
        /// </summary>
        public Task<IUpdateResponse<Payment>> Update(Payment payment)
        {
            var response = _elastic.UpdateAsync<Payment>(payment.Id, descriptor => descriptor
                .Index(_indexName)
                .Doc(payment)
            );

            return response;
        }

        /// <summary>
        /// Gets total count of payments
        /// </summary>
        public async Task<long> Count()
        {
            //_elastic.Refresh(_indexName);

            var response = await _elastic.CountAsync<Payment>(new CountRequest(_indexName));

            return response.Count;
        }

        /// <summary>
        /// Delete entity by ID
        /// </summary>
        public async Task<IDeleteResponse> Delete(Guid id)
        {
            var response = await _elastic.DeleteAsync(new DocumentPath<Payment>(id), g => g
                .Index(_indexName)
                .Type(typeof(Payment))
            );
            return response;
        }
    }
}