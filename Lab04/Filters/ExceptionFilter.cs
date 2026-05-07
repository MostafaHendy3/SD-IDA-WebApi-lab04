using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Lab04.Filters
{
    public class ExceptionFilter : IExceptionFilter 
    {
        private readonly ILogger<ExceptionFilter> _logger;
        public ExceptionFilter(ILogger<ExceptionFilter> logger)
        {
            _logger = logger;
        }
        public void OnException(ExceptionContext context)
        {
            _logger.LogError(context.Exception, "An error occurred while processing the request.");
            context.Result = new ContentResult() { Content = "An error occurred while processing the request.", StatusCode = 500 };
        }
    }
}