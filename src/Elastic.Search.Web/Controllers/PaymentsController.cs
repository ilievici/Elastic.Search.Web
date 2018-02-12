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
        /// Obtain payment by ID
        /// </summary>
        [HttpGet("[action]/{id:guid}")]
        [Produces("application/json", Type = typeof(ResultModel))]
        public async Task<JsonResult> GetById(Guid id)
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
        /// Delete the a specific payment.
        /// </summary>
        [HttpDelete("[action]/{id:guid}")]
        [Produces("application/json", Type = typeof(ResultModel))]
        public async Task<JsonResult> Delete(Guid id)
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
        /// Register a new payment.
        /// </summary>
        [HttpPost("[action]")]
        [Produces("application/json", Type = typeof(ResultModel))]
        public async Task<JsonResult> Create([FromBody, Required] Payment payment)
        {
            var response = new ResultModel();
            try
            {
                var model = await _paymentService.BulkInsert(new[] { payment });
                _logger.LogInformation("Create", payment, model);

                response.IsSuccess = true;
                response.Result = model;

                return Json(response);
            }
            catch (Exception exception)
            {
                _logger.LogError("Create", payment, exception);
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
        public async Task<JsonResult> CreateList([FromBody, Required] List<Payment> payments)
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
        /// Update an existing payment
        /// </summary>
        [HttpPut("[action]")]
        [Produces("application/json", Type = typeof(ResultModel))]
        public async Task<JsonResult> Update([FromBody, Required] Payment payment)
        {
            var response = new ResultModel();
            try
            {
                await _paymentService.Update(payment);
                _logger.LogInformation("Update", payment);

                response.IsSuccess = true;
                response.Result = null;

                return Json(response);
            }
            catch (Exception exception)
            {
                _logger.LogError("Update", payment, exception);
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
                    GenFu.GenFu.Configure<Payment>()
                        .Fill(s => s.PaymentType).WithRandom(new[] { "C", "A", "T", "B" })
                        .Fill(s => s.Color).WithRandom(new[] { "red", "black", "orange" })
                        .Fill(s => s.Channel).WithRandom(new[] { "API", "INTERNET", "EXTRANET" })
                        .Fill(s => s.CardIssuer).WithRandom(new[] { "AmericanExpress", "VISA", "MasterCard", "Other" })
                        .Fill(s => s.IsAudit).WithRandom(new[] { false, false, true })
                        .Fill(s => s.Emails).WithRandom(
                            new[]
                            {
                                GenFu.GenFu.ListOf<EmailEntity>(1)
                                    .Select(s => s.Email)
                                    .ToList(),
                                GenFu.GenFu.ListOf<EmailEntity>(3)
                                    .Select(s => s.Email)
                                    .ToList(),
                                GenFu.GenFu.ListOf<EmailEntity>(2)
                                    .Select(s => s.Email)
                                    .ToList(),
                                GenFu.GenFu.ListOf<EmailEntity>(2)
                                    .Select(s => s.Email)
                                    .ToList()
                            })
                        .Fill(s => s.Phones).WithRandom(
                            new[]
                            {
                                GenFu.GenFu.ListOf<PhoneEntity>(2)
                                    .Select(s => s.Phone)
                                    .ToList(),
                                GenFu.GenFu.ListOf<PhoneEntity>(3)
                                    .Select(s => s.Phone)
                                    .ToList(),
                                GenFu.GenFu.ListOf<PhoneEntity>(2)
                                    .Select(s => s.Phone)
                                    .ToList(),
                                GenFu.GenFu.ListOf<PhoneEntity>(2)
                                    .Select(s => s.Phone)
                                    .ToList(),
                                GenFu.GenFu.ListOf<PhoneEntity>(2)
                                    .Select(s => s.Phone)
                                    .ToList()
                            });

                    var payments = GenFu.GenFu.ListOf<Payment>(totalCount);

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

