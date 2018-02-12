using System.Threading.Tasks;
using Nest;

namespace Elastic.Search.Core.Infrastructure.Abstract
{
    /// <summary>
    /// Interface to index configuration
    /// </summary>
    public interface IIndexConfigProvider
    {
        /// <summary>
        /// Get curent client index name
        /// </summary>
        string GetIndexName();

        /// <summary>
        /// Create index
        /// </summary>
        Task<ICreateIndexResponse> CreateIndex<T>()
          where T : class;

        /// <summary>
        /// Delete index
        /// </summary>
        Task<IDeleteIndexResponse> DeleteIndex();

        /// <summary>
        /// Checks is index exists
        /// </summary>
        Task<bool> IndexExists();

        /// <summary>
        /// Refresh index
        /// </summary>
        Task<IRefreshResponse> RefreshIndex();
    }
}
