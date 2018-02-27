using System.Collections.Generic;
using System.Threading.Tasks;
using Elastic.Search.Core.Models;
using Nest;

namespace Elastic.Search.Core.Service.Abstract
{
    /// <summary>
    /// Interface to do payments 
    /// </summary>
    public interface IPaymentService
    {
        /// <summary>
        /// Bulk payments insert.
        /// </summary>
        Task<IBulkResponse> BulkInsert(IEnumerable<ElasticPaymentModel> payments);

        /// <summary>
        /// Delete entity by ID
        /// </summary>
        Task<IDeleteResponse> Delete(int id);

        /// <summary>
        /// Get entity by ID.
        /// </summary>
        Task<ElasticPaymentModel> GetById(int id);

        /// <summary>
        /// Update elasticPaymentModel.
        /// </summary>
        Task<IUpdateResponse<ElasticPaymentModel>> Update(ElasticPaymentModel elasticPaymentModel);

        /// <summary>
        /// Gets total count of payments
        /// </summary>
        Task<long> Count();

        /// <summary>
        /// Gets MAX payment ID
        /// </summary>
        Task<int> GetMaxPaymentId();
    }
}