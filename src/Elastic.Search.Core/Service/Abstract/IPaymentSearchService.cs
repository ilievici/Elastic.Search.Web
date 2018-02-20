using System.Threading.Tasks;
using Elastic.Search.Core.Models;

namespace Elastic.Search.Core.Service.Abstract
{
    /// <summary>
    /// Interface to do payments search 
    /// </summary>
    public interface IPaymentSearchService
    {
        /// <summary>
        /// Search payments by <see cref="PaymentFilterModel"/> criterias.
        /// </summary>
        /// <param name="filter">The <see cref="PaymentFilterModel"/>.</param>
        Task<SearchResult<ElasticPaymentModel>> Search(PaymentFilterModel filter);
    }
}