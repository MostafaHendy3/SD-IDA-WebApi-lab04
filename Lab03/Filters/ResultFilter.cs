using Microsoft.AspNetCore.Mvc.Filters;

namespace Lab03.Filters
{
    public class ResultFilter : IResultFilter
    {
        private readonly ILogger<ResultFilter> _logger;
        public ResultFilter(ILogger<ResultFilter> logger)
        {
            _logger = logger;
        }
        public void OnResultExecuting(ResultExecutingContext context)
        {
            // add header to the response
            context.HttpContext.Response.Headers.Append("server-time", DateTime.Now.ToString());
            _logger.LogInformation("OnResultExecuting: {Result}", $"Header: server-time: {context.HttpContext.Response.Headers["server-time"]}");
        }
        public void OnResultExecuted(ResultExecutedContext context)
        {

        }
    }
}