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
        private readonly IConnectionProvider _connectionProvider;
        private readonly IElasticClient _elastic;

        private string IndexName => GetIndexName();

        /// <summary>
        /// Get curent client index name
        /// </summary>
        public string GetIndexName()
        {
            var id = _connectionProvider.GetClientId();
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentNullException("CLIENT_ID is missing");
            }

            return id.ToLower();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IndexConfigProvider"/> class.
        /// </summary>
        public IndexConfigProvider(IConnectionProvider connectionProvider, IElasticClient elastic)
        {
            _connectionProvider = connectionProvider;
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

            var descriptor = new CreateIndexDescriptor(IndexName)
                .Settings(t => t
                    .NumberOfShards(1)
                    .NumberOfReplicas(0)
                )
                .Mappings(ms => ms
                    .Map<T>(m => m.AutoMap()
                    )
                );

            return await _elastic.CreateIndexAsync(IndexName, d => descriptor);

            /*
            return await _elastic
                .CreateIndexAsync(IndexName, s => s
                    .Settings(t => t
                        .NumberOfShards(1)
                        .NumberOfReplicas(0)

                        .Analysis(ar => ar
                            .Analyzers(a => a
                                .Custom("autocomplete", cc => cc
                                    .Filters("eng_stopwords", "trim", "lowercase")
                                    .Tokenizer("autocomplete")
                                )
                            )
                            .Tokenizers(tdesc => tdesc
                                .EdgeNGram("autocomplete", e => e
                                    .MinGram(3)
                                    .MaxGram(15)
                                    .TokenChars(TokenChar.Letter, TokenChar.Digit)
                                )
                            )
                            .TokenFilters(f => f
                                .Stop("eng_stopwords", lang => lang
                                    .StopWords("_english_")
                                )
                            )
                        )
                    )
                    //.Settings(st => st
                    //    .NumberOfShards(1)
                    //    .NumberOfReplicas(0)
                    //    .Analysis(an => an
                    //        .Analyzers(anz => anz
                    //            .Custom("ngram_analyzer", cc => cc
                    //                    .Tokenizer("ngram_tokenizer")
                    //                //.Filters(nGramFilters)
                    //            )
                    //        )
                    //        .Tokenizers(tz => tz
                    //            .NGram("ngram_tokenizer", td => td
                    //                .MinGram(2)
                    //                .MaxGram(20)
                    //                .TokenChars(
                    //                    TokenChar.Letter,
                    //                    TokenChar.Digit,
                    //                    TokenChar.Punctuation,
                    //                    TokenChar.Symbol
                    //                )
                    //            )
                    //        )
                    //    )
                    //)
                    .Mappings(m => m
                        .Map<T>(d => d
                            .AutoMap()
                        )
                    )
                );
            */
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