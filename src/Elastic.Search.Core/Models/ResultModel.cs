using System.Collections.Generic;
using Newtonsoft.Json;

namespace Elastic.Search.Core.Models
{
    /// <summary>
    /// API Result Model
    /// </summary>
    public class ResultModel
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public ResultModel()
        {
            Errors = new List<string>();
        }

        /// <summary>
        /// Bool indicating that the request resulted with success.
        /// </summary>
        [JsonProperty("is_success")]
        public bool IsSuccess { get; set; }

        /// <summary>
        /// This property will contain error keys if any.
        /// </summary>
        [JsonProperty("errors")]
        public ICollection<string> Errors { get; set; }

        /// <summary>
        /// The result of the response, if there is no errors.
        /// </summary>
        [JsonProperty("result")]
        public object Result { get; set; }
    }
}
