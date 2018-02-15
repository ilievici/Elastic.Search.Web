using System;
using System.Threading.Tasks;
using Elastic.Search.Core.Infrastructure.Abstract;
using Microsoft.AspNetCore.Http;
using Nest;

namespace Elastic.Search.Core.Infrastructure
{
    /// <summary>
    /// Index configuration provider
    /// </summary>
    public class IndexConfigProvider : IIndexConfigProvider
    {
        private readonly IElasticClient _elastic;

        private string IndexName => GetIndexName();

        /// <summary>
        /// Get curent client index name
        /// </summary>
        public string GetIndexName()
        {
            var clientIdFromHeader = _httpContextAccessor.HttpContext.Request.Headers[Constants.CLIENT_ID];

            if (string.IsNullOrWhiteSpace(clientIdFromHeader))
            {
                //TODO: test values
                string clientId = "cprofile_nextgen";
                return clientId.ToLower();
            }

            string value = clientIdFromHeader.ToString().ToLower();

            if (string.IsNullOrWhiteSpace(clientIdFromHeader))
            {
                throw new ArgumentNullException("CLIENT_ID is missing");
            }

            return value.ToLower();
        }

        private readonly IHttpContextAccessor _httpContextAccessor;

        /// <summary>
        /// Initializes a new instance of the <see cref="IndexConfigProvider"/> class.
        /// </summary>
        public IndexConfigProvider(IHttpContextAccessor httpContextAccessor, IElasticClient elastic)
        {
            _httpContextAccessor = httpContextAccessor;
            _elastic = elastic;
        }

        /// <summary>
        /// Create index
        /// </summary>
        public async Task<ICreateIndexResponse> CreateIndex<T>()
            where T : class
        {
            var response = await _elastic.IndexExistsAsync(new IndexExistsRequest(IndexName));
            if (response.Exists)
            {
                return null;
            }

            return await _elastic
                .CreateIndexAsync(IndexName, s => s
                    .Settings(t => t
                        .NumberOfShards(1)
                        .NumberOfReplicas(0)
                    )
                    .Mappings(m => m
                        .Map<T>(d => d
                            .AutoMap()
                        )
                    )
                );
        }

        /// <summary>
        /// Delete index
        /// </summary>
        public async Task<IDeleteIndexResponse> DeleteIndex()
        {
            var result = await _elastic.IndexExistsAsync(new IndexExistsRequest(IndexName));
            return result.Exists
                ? await _elastic.DeleteIndexAsync(IndexName)
                : null;
        }

        /// <summary>
        /// Checks is index exists
        /// </summary>
        public async Task<bool> IndexExists()
        {
            var result = await _elastic.IndexExistsAsync(new IndexExistsRequest(IndexName));
            return result.Exists;
        }

        /// <summary>
        /// Refresh index
        /// </summary>
        public Task<IRefreshResponse> RefreshIndex()
        {
            return _elastic.RefreshAsync(IndexName);
        }
    }
}