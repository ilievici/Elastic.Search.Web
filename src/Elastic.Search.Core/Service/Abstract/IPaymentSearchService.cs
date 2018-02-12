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
        /// Search payments by <see cref="AdvancedSearchCriteria"/> criterias.
        /// </summary>
        /// <param name="filter">The <see cref="AdvancedSearchCriteria"/>.</param>
        Task<SearchResult<Payment>> Search(AdvancedSearchCriteria filter);
    }
}