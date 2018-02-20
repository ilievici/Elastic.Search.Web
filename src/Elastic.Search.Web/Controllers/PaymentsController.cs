using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Elastic.Search.Core.Models;
using Elastic.Search.Core.Service.Abstract;
using Elastic.Search.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Elastic.Search.Web.Controllers
{
    /// <summary>
    /// Payments Controller
    /// </summary>
    [Route("api/[controller]")]
    public class PaymentsController : Controller
    {
        private readonly IPaymentService _paymentService;
        private readonly ILogger<PaymentsController> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="PaymentsController"/> class.
        /// </summary>
        public PaymentsController(IPaymentService paymentService, ILoggerFactory loggerFactory)
        {
            _paymentService = paymentService;
            _logger = loggerFactory.CreateLogger<PaymentsController>();
        }

        /// <summary>
        /// Obtain elasticPaymentModel by ID
        /// </summary>
        [HttpGet("[action]/{id}")]
        [Produces("application/json", Type = typeof(ResultModel))]
        public async Task<JsonResult> GetById(string id)
        {
            var response = new ResultModel();
            try
            {
                var model = await _paymentService.GetById(id);
                _logger.LogInformation("GetById", id, model);

                response.IsSuccess = true;
                response.Result = model;

                return Json(response);
            }
            catch (Exception exception)
            {
                _logger.LogError("GetById", id, exception);
                response.IsSuccess = false;
                response.Errors = new List<string> { exception.Message };
                return Json(response);
            }
        }

        /// <summary>
        /// Delete the a specific elasticPaymentModel.
        /// </summary>
        [HttpDelete("[action]/{id}")]
        [Produces("application/json", Type = typeof(ResultModel))]
        public async Task<JsonResult> Delete(string id)
        {
            var response = new ResultModel();
            try
            {
                var model = await _paymentService.Delete(id);
                _logger.LogInformation("GetById", id, model);

                response.IsSuccess = true;
                response.Result = model;

                return Json(response);
            }
            catch (Exception exception)
            {
                _logger.LogError("Delete", id, exception);
                response.IsSuccess = false;
                response.Errors = new List<string> { exception.Message };
                return Json(response);
            }
        }

        /// <summary>
        /// Register a new elasticPaymentModel.
        /// </summary>
        [HttpPost("[action]")]
        [Produces("application/json", Type = typeof(ResultModel))]
        public async Task<JsonResult> Create([FromBody, Required] ElasticPaymentModel elasticPaymentModel)
        {
            var response = new ResultModel();
            try
            {
                var model = await _paymentService.BulkInsert(new[] { elasticPaymentModel });
                _logger.LogInformation("Create", elasticPaymentModel, model);

                response.IsSuccess = true;
                response.Result = model;

                return Json(response);
            }
            catch (Exception exception)
            {
                _logger.LogError("Create", elasticPaymentModel, exception);
                response.IsSuccess = false;
                response.Errors = new List<string> { exception.Message };
                return Json(response);
            }
        }

        /// <summary>
        /// Register a new list of payments.
        /// </summary>
        [HttpPost("[action]")]
        [Produces("application/json", Type = typeof(ResultModel))]
        public async Task<JsonResult> CreateList([FromBody, Required] List<ElasticPaymentModel> payments)
        {
            var response = new ResultModel();
            try
            {
                var model = await _paymentService.BulkInsert(payments);
                _logger.LogInformation("CreateList", payments, model);

                response.IsSuccess = true;
                response.Result = model;

                return Json(response);
            }
            catch (Exception exception)
            {
                _logger.LogError("CreateList", payments, exception);
                response.IsSuccess = false;
                response.Errors = new List<string> { exception.Message };
                return Json(response);
            }
        }

        /// <summary>
        /// Update an existing elasticPaymentModel
        /// </summary>
        [HttpPut("[action]")]
        [Produces("application/json", Type = typeof(ResultModel))]
        public async Task<JsonResult> Update([FromBody, Required] ElasticPaymentModel elasticPaymentModel)
        {
            var response = new ResultModel();
            try
            {
                await _paymentService.Update(elasticPaymentModel);
                _logger.LogInformation("Update", elasticPaymentModel);

                response.IsSuccess = true;
                response.Result = null;

                return Json(response);
            }
            catch (Exception exception)
            {
                _logger.LogError("Update", elasticPaymentModel, exception);
                response.IsSuccess = false;
                response.Errors = new List<string> { exception.Message };
                return Json(response);
            }
        }

        /// <summary>
        /// Register random list of payments.
        /// </summary>
        [HttpPost("[action]")]
        [Produces("application/json", Type = typeof(ResultModel))]
        public async Task<JsonResult> CreateRandom(int totalCount)
        {
            var response = new ResultModel();
            try
            {
                if (totalCount > 0)
                {
                    GenFu.GenFu.Configure<ElasticPaymentModel>()
                        .Fill(s => s.PaymentType).WithRandom(new[] { "C", "A", "T", "B" })
                        .Fill(s => s.Color).WithRandom(new[] { "red", "black", "orange" })
                        .Fill(s => s.Channel).WithRandom(new[] { "API", "INTERNET", "EXTRANET" })
                        .Fill(s => s.CollectorId).WithRandom(new[] { "API", "INTERNET", "EXTRANET" })
                        .Fill(s => s.CreditSiteId).WithRandom(new[] { "CUTX", "FLET", "CUNY" })
                        .Fill(s => s.Status).WithRandom(new[] { "Exported", "Refund - Pending", "Pending" , "Refund - Processed" , "Processed" })
                        .Fill(s => s.CardIssuer).WithRandom(new[] { "AmericanExpress", "VISA", "MasterCard", "Other" })
                        .Fill(s => s.HasAuditDetails).WithRandom(new[] { false, false, true })
                        .Fill(s => s.FeeAmount).WithinRange(0, 100)
                        .Fill(s => s.Amount).WithinRange(0, 9000);

                    var payments = GenFu.GenFu.ListOf<ElasticPaymentModel>(totalCount);

                    var model = await _paymentService.BulkInsert(payments);
                    _logger.LogInformation("CreateRandom", payments, model);
                }

                var totalInIndex = await _paymentService.Count();

                response.IsSuccess = true;
                response.Result = new
                {
                    TotalNewElements = totalCount,
                    TotalInIndex = totalInIndex
                };

                return Json(response);
            }
            catch (Exception exception)
            {
                _logger.LogError("CreateRandom", totalCount, exception);
                response.IsSuccess = false;
                response.Errors = new List<string>
                {
                    exception.Message
                };

                return Json(response);
            }
        }
    }
}

