using System;
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
        Task<IBulkResponse> BulkInsert(IEnumerable<Payment> payments);

        /// <summary>
        /// Delete entity by ID
        /// </summary>
        Task<IDeleteResponse> Delete(Guid id);

        /// <summary>
        /// Get entity by ID.
        /// </summary>
        Task<Payment> GetById(Guid id);

        /// <summary>
        /// Update payment.
        /// </summary>
        Task<IUpdateResponse<Payment>> Update(Payment payment);

        /// <summary>
        /// Gets total count of payments
        /// </summary>
        Task<long> Count();
    }
}