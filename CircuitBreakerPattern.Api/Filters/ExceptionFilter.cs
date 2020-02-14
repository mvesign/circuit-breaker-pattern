using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace CircuitBreakerPattern.Api.Filters
{
    /// <inheritdoc />
    public class ExceptionFilter : IExceptionFilter
    {
        private readonly ILogger<ExceptionFilter> _logger;

        /// <summary>
        /// Default ctor.
        /// </summary>
        /// <param name="logger">Logger.</param>
        public ExceptionFilter(ILogger<ExceptionFilter> logger)
        {
            _logger = logger;
        }

        /// <inheritdoc />
        public void OnException(ExceptionContext context)
        {
            _logger.LogWarning(context.Exception, context.Exception.Message);
            context.ExceptionHandled = true;
            context.Result = new BadRequestResult();
        }
    }
}