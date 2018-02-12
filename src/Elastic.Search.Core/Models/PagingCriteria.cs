using System.ComponentModel.DataAnnotations;

namespace Elastic.Search.Core.Models
{
    public class PagingCriteria
    {
        public int Take { get; set; } = 10;

        public int Page { get; set; }
        
        [StringLength(50, ErrorMessage = "Value is too long")]
        public string SortingField { get; set; }

        [StringLength(50, ErrorMessage = "Value is too long")]
        public string SortingDirection { get; set; }
    }
}
