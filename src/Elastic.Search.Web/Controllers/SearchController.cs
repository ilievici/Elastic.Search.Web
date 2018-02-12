using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Elastic.Search.Core.Models;
using Elastic.Search.Core.Service.Abstract;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Elastic.Search.Web.Controllers
{
    /// <summary>
    /// Payments search Controller
    /// </summary>
    [Route("api/[controller]")]
    public class SearchPaymentsController : Controller
    {
        private readonly IPaymentSearchService _paymentSearchService;
        private readonly ILogger<SearchPaymentsController> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="SearchPaymentsController"/> class.
        /// </summary>
        public SearchPaymentsController(IPaymentSearchService paymentSearchService, ILoggerFactory loggerFactory)
        {
            _paymentSearchService = paymentSearchService;
            _logger = loggerFactory.CreateLogger<SearchPaymentsController>();
        }

        /// <summary>
        /// Does search for payments by given criterias (from Query)
        /// </summary>
        [HttpGet("[action]")]
        [Produces("application/json", Type = typeof(ResultModel))]
        public Task<JsonResult> Search([FromQuery] AdvancedSearchCriteria criteria)
        {
            return DoSearch(criteria);
        }

        /// <summary>
        /// Does search for payments by given criterias (from JSON Body)
        /// </summary>
        [HttpPost("[action]")]
        [Produces("application/json", Type = typeof(ResultModel))]
        public Task<JsonResult> SearchByBody([FromBody] AdvancedSearchCriteria criteria)
        {
            return DoSearch(criteria);
        }

        private async Task<JsonResult> DoSearch(AdvancedSearchCriteria criteria)
        {
            var response = new ResultModel();

            try
            {
                if (criteria == null)
                {
                    throw new ArgumentNullException(nameof(criteria));
                }

                var data = await _paymentSearchService.Search(criteria);
                _logger.LogInformation("HasAnySearchCriteria", criteria, data);

                response.IsSuccess = true;
                response.Result = data;

                return Json(data);
            }
            catch (Exception exception)
            {
                _logger.LogError("Search", exception, criteria);
                response.IsSuccess = false;
                response.Errors = new List<string> { exception.Message };
                return Json(response);
            }
        }
    }
}
