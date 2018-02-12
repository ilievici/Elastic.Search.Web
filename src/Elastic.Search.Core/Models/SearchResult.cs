using System.Collections.Generic;

namespace Elastic.Search.Core.Models
{
    public class SearchResult<T>
    {
        public long Total { get; set; }
        public int Page { get; set; }
        public int TotalOnPage { get; set; }
        public long ElapsedMilliseconds { get; set; }
        public IEnumerable<T> Results { get; set; }
    }
}