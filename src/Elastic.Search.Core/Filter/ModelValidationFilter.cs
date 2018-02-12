using System.Linq;
using System.Net;
using Elastic.Search.Core.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;

namespace Elastic.Search.Core.Filter
{
    /// <summary>
    /// Fileter of <see cref="IActionFilter" /> type used to validate Controller MVC Model State
    /// </summary>
    public sealed class ModelValidationFilter : IActionFilter
    {
        private readonly JsonSerializerSettings _serializerSettings;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="ModelValidationFilter"/> class.
        /// </summary>
        public ModelValidationFilter()
        {
            _serializerSettings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented
            };
        }

        /// <summary>
        /// Trigger the filter OnActionExecuting
        /// </summary>
        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.ModelState.IsValid)
                return;

            var result = new ResultModel
            {
                IsSuccess = false,
                Errors = context.ModelState.Values
                    .SelectMany(x => x.Errors)
                    .Where(x => x != null)
                    .Select(s => s.ErrorMessage)
                    .ToList()
            };

            context.Result = new ContentResult
            {
                ContentType = "application/json",
                Content = JsonConvert.SerializeObject(result, _serializerSettings),
                StatusCode = (int)HttpStatusCode.OK
            };
        }

        /// <summary>
        /// Trigger the filter OnActionExecuted
        /// </summary>
        public void OnActionExecuted(ActionExecutedContext context)
        {
            // Do nothing
        }
    }
}