using System.Collections.Generic;
using System.Net;
using Elastic.Search.Core.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Elastic.Search.Core.Filter
{
    /// <summary>
    /// A filter that runs after an action has thrown an <see cref="System.Exception"/>. It works at global level.
    /// </summary>
    public class HttpGlobalExceptionFilter : IExceptionFilter
    {
        private readonly JsonSerializerSettings _serializerSettings;
        private readonly ILogger<HttpGlobalExceptionFilter> _logger;

        /// <summary>
        /// The instance of <see cref="HttpGlobalExceptionFilter"/>
        /// </summary>
        public HttpGlobalExceptionFilter(ILogger<HttpGlobalExceptionFilter> logger)
        {
            _logger = logger;
            _serializerSettings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented
            };
        }

        /// <summary>
        ///  Called after an action has thrown an <see cref="System.Exception"/>.
        /// </summary>
        /// <param name="context"></param>
        public void OnException(ExceptionContext context)
        {
            _logger.LogError(new EventId(context.Exception.HResult),
                context.Exception,
                context.Exception.Message);

            var exception = context.Exception;
            var message = !string.IsNullOrEmpty(exception.Message)
                ? exception.Message
                : "An unhandled exception has occurred.";

            var result = new ResultModel
            {
                IsSuccess = false,
                Errors = new List<string> { message }
            };

            var serializeObject = JsonConvert.SerializeObject(result, _serializerSettings);

            context.Result = new BadRequestObjectResult(serializeObject);
            context.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            context.ExceptionHandled = true;
        }
    }
}
